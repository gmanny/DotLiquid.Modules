using System;
using System.IO;
using System.Text.RegularExpressions;
using DotLiquid.FileSystems;
using DotLiquid.Util;

namespace DotLiquid.Modules.TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            DotLiquidModules.Init();

            string fileName = "Resources/Test/test.liquid";
            string tpl = File.ReadAllText(fileName);

            Template compiled = Template.Parse(tpl);
            Context ctx = new Context();
            ctx.Registers["file_system"] = new LocalFileSystem(Path.GetFullPath(Path.GetDirectoryName(fileName)));
            
            compiled.Render(Console.Out, new RenderParameters {Context = ctx});
        }
    }
}
