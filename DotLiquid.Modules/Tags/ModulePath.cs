using System;
using System.Collections.Generic;
using System.IO;

namespace DotLiquid.Modules.Tags
{
    public class ModulePath : Tag
    {
        private string _pathPrefix; // all UseModule inclusions will be prefixed with this path in the future

        public override void Initialize(string tagName, string markup, List<string> tokens)
        {
            _pathPrefix = markup.Trim(); // remember the prefix

            base.Initialize(tagName, markup, tokens);
        }

        public override void Render(Context context, TextWriter result)
        {
            // init modules context
            ModulesContext modules = DotLiquidModules.ContextExtractor.GetOrAddModulesContext(context);

            // we change prefix on render, to make multiple usage work
            // also we can therefore evaluate markup as an expression
            object evalPath = context[_pathPrefix];
            string modPath = evalPath != null ? Convert.ToString(evalPath) : _pathPrefix;

            // set the prefix
            modules.PathPrefix = modPath;
        }
    }
}
