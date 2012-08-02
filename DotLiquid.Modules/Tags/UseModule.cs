using System;
using System.Collections.Generic;
using System.IO;
using DotLiquid.FileSystems;
using QuickGraph;
using QuickGraph.Algorithms;

namespace DotLiquid.Modules.Tags
{
    public class UseModule : Tag
    {
        private string _moduleName;

        public override void Initialize(string tagName, string markup, List<string> tokens)
        {
            _moduleName = markup.Trim(); // remember the name

            base.Initialize(tagName, markup, tokens);
        }

        public override void Render(Context context, TextWriter result)
        {
            // init modules context
            context.Registers["modules"] = context.Registers["modules"] ?? new ModulesContext();
            ModulesContext modules = context.Registers.Get<ModulesContext>("modules");

            // allow module name to be supplied as a variable, makes sense when you supply modules in the Model
            object evalName = context[_moduleName];
            string modName = evalName != null ? Convert.ToString(evalName) : _moduleName;
            
            // remember modules that were already loaded
            Dictionary<string, bool> alreadyLoaded = new Dictionary<string, bool>();
            foreach (string moduleName in modules.ModuleIndex.Keys)
            {
                alreadyLoaded[moduleName] = true;
            }

            // add module to context, get its dependencies in graph
            AdjacencyGraph<Module, Edge<Module>> dependencyGraph = new AdjacencyGraph<Module, Edge<Module>>(true);
            AddModuleToContextByName(modName, modules, context, dependencyGraph);

            // add dependency tree into context's dependency list
            foreach (Module module in dependencyGraph.TopologicalSort())
            {
                if (!alreadyLoaded.ContainsKey(module.ModuleName))
                {
                    modules.DependencyOrder.Add(module);
                }
            }
        }

        public static Module AddModuleToContextByName(string moduleName, ModulesContext ctx, Context global, AdjacencyGraph<Module, Edge<Module>> dependencyGraph)
        {
            if (ctx.ModuleIndex.ContainsKey(moduleName))
            { // we've got that module already
                return ctx.ModuleIndex[moduleName];
            }

            // freaking file system tries to evaluate it for us
            string moduleNameQuoted = "\"" + moduleName + "\"";

            // load module file and parse it.
            IFileSystem fileSystem = global.Registers["file_system"] as IFileSystem ?? Template.FileSystem;
            string source = fileSystem.ReadTemplateFile(global, moduleNameQuoted);
            Template template = Template.Parse(source);

            Module rootMod = null;

            // run all the modules from file
            template.Root.NodeList.ForEach(o =>
                                               {
                                                   Module m = o as Module;

                                                   if (m != null)
                                                   {
                                                       AddModuleToContext(m, ctx, global, dependencyGraph);

                                                       if (m.ModuleName == moduleName)
                                                       {
                                                           rootMod = m;
                                                       }
                                                   }
                                               });

            return rootMod;
        }

        public static void AddModuleToContext(Module module, ModulesContext ctx, Context global, AdjacencyGraph<Module, Edge<Module>> dependencyGraph)
        {
            // add module to index
            Module storedModule;
            if (ctx.ModuleIndex.ContainsKey(module.ModuleName))
            {
                storedModule = ctx.ModuleIndex[module.ModuleName];
            }
            else
            {
                ctx.ModuleIndex[module.ModuleName] = module;
                dependencyGraph.AddVertex(module);
                storedModule = module;
            }

            // fill in module's sections first
            module.NodeList.ForEach(o =>
                                        {
                                            Section s = o as Section;

                                            if (s != null)
                                            {
                                                List<Section> secList;

                                                if (storedModule.Sections.ContainsKey(s.SectionName))
                                                {
                                                    secList = storedModule.Sections[s.SectionName];
                                                } 
                                                else
                                                {
                                                    secList = new List<Section>();
                                                    storedModule.Sections[s.SectionName] = secList;
                                                }

                                                secList.Add(s);
                                            }
                                        });

            // then parse dependencies and upgrade dependency graph
            module.NodeList.ForEach(o =>
                                        {
                                            DependsOn d = o as DependsOn;

                                            if (d != null)
                                            {
                                                // add this module to context
                                                Module dependsOn = AddModuleToContextByName(d.ModuleName, ctx, global, dependencyGraph);

                                                // upgrade graph
                                                dependencyGraph.AddVerticesAndEdge(new Edge<Module>(dependsOn, storedModule));
                                            }
                                        });
        }
    }
}
