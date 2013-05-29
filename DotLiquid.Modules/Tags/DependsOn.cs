using System.Collections.Generic;
using System.IO;

namespace DotLiquid.Modules.Tags
{
    public class DependsOn : Tag
    {
        private string _name; // token from script

        public string ModuleName { get { return _name; } }

        public override void Initialize(string tagName, string markup, List<string> tokens)
        {
            _name = markup.Trim(); // remember the name

            base.Initialize(tagName, markup, tokens);
        }

        public override void Render(Context context, TextWriter result)
        {
            // purely semantic tag, it has no output
        }
    }
}
