using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DotLiquid.Modules.Tags
{
    public class TokenInsertion
    {
        private readonly string _tokenName;

        public string TokenName { get { return _tokenName; } }

        public TokenInsertion(string tokenName)
        {
            _tokenName = tokenName;
        }
    }

    public class WriteSection : Block
    {
        private string _sectionName;
        private List<object> _metaNodeList = new List<object>(); // here we've got our node lists and occasionally insertion points for the tokens

        public override void Initialize(string tagName, string markup, List<string> tokens)
        {
            _sectionName = markup.Trim(); // remember the name

            base.Initialize(tagName, markup, tokens);
        }

        public override void UnknownTag(string tag, string markup, List<string> tokens)
        {
            PushToken(tag);
        }

        public override void Render(Context context, TextWriter result)
        {
            // get modules context
            ModulesContext mod = context.Registers["modules"] as ModulesContext;

            // alow section names to be variables too
            object evalName = context[_sectionName];
            string secName = evalName != null ? Convert.ToString(evalName) : _sectionName;

            // there was no modules loaded, so section is empty
            if (mod == null)
            {
                return;
            }

            // walk the dependency graph
            foreach (Module module in mod.DependencyOrder)
            {
                if (module.Sections.ContainsKey(secName))
                {
                    foreach (Section section in module.Sections[secName])
                    {
                        if (section.Condition != null && !section.Condition.Evaluate(context))
                        {
                            continue;
                        }

                        foreach (object o in _metaNodeList)
                        {
                            if (o is TokenInsertion)
                            {
                                TokenInsertion tok = o as TokenInsertion;

                                if (!section.Tokens.ContainsKey(tok.TokenName))
                                {
                                    result.Write("[Section '" + section.SectionName + "' in module '" + module.ModuleName + "' doesn't have token named '" + tok.TokenName + "']");
                                }
                                else
                                {
                                    MemoryStream buf = new MemoryStream(); // we trim sections (to allow normal src values and such)
                                    TextWriter bufWriter = new StreamWriter(buf); // so we need to capture output in the buffer first

                                    RenderAll(section.Tokens[tok.TokenName].NodeList, context, bufWriter);

                                    bufWriter.Flush(); // it has buffering, yeah
                                    result.Write(Encoding.UTF8.GetString(buf.ToArray()).Trim());
                                }
                            }
                            else if (o is List<object>)
                            {
                                RenderAll(o as List<object>, context, result);
                            }
                        }

                        // then render the ending that's stored in NodeList
                        RenderAll(NodeList, context, result);
                    }
                }
            }
        }

        public void PushToken(string tokenName)
        {
            _metaNodeList.Add(NodeList);
            _metaNodeList.Add(new TokenInsertion(tokenName));

            NodeList = new List<object>();
        }
    }
}
