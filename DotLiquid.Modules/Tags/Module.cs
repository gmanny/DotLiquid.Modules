using System.Collections.Generic;
using System.IO;

namespace DotLiquid.Modules.Tags
{
    public class Module : Block
    {
        private string _name; // token from script

        public string ModuleName { get { return _name; } }

        private readonly Dictionary<string, List<Section>> _sections = new Dictionary<string, List<Section>>();

        // being filled in by UseModule tag while rendering
        public Dictionary<string, List<Section>> Sections { get { return _sections; } }

        public override void Initialize(string tagName, string markup, List<string> tokens)
        {
            _name = markup.Trim(); // remember the name

            base.Initialize(tagName, markup, tokens);
        }

        public override void Render(Context context, TextWriter result)
        {
            // do nothing, it's parse-only class

            // NodeList is attached to the real template and rendered by WriteSection block
        }
    }
}

