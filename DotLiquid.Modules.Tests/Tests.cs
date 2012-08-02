using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using DotLiquid.Modules.Tests.Util;
using Xunit;

namespace DotLiquid.Modules.Tests
{
    public class Tests
    {
        private Context GetResourceContext()
        {
            Context ctx = new Context();
            ctx.Registers["file_system"] = new ResourceFileSystem("DotLiquid.Modules.Tests.Resources", Assembly.GetExecutingAssembly());

            return ctx;
        }
        private void Init()
        {
            DotLiquidModules.Init();
        }
        private string ParseAndRun(string tpl)
        {
            Template compiled = Template.Parse(tpl);
            MemoryStream buf = new MemoryStream();
            TextWriter tw = new StreamWriter(buf);

            compiled.Render(tw, new RenderParameters { Context = GetResourceContext() });

            tw.Flush();
            return Encoding.UTF8.GetString(buf.ToArray());
        }

        [Fact]
        public void IsFunctional()
        {
            Init();

            Assert.Equal(
                    "te-st",
                    ParseAndRun("{% usemodule test %}{% writesection test %}{% test %}{% endwritesection %}")
                );
        }

        [Fact]
        public void TwoSimilarSections()
        {
            Init();

            Assert.Equal(
                    "te-stte-st",
                    ParseAndRun("{% usemodule test %}{% writesection test %}{% test %}{% endwritesection %}{% writesection test %}{% test %}{% endwritesection %}")
                );
        }

        [Fact]
        public void BasicDependencies()
        {
            Init();

            Assert.Equal(
                    "masterslave",
                    ParseAndRun("{% usemodule slave %}{% writesection test %}{% test %}{% endwritesection %}")
                );
        }

        [Fact]
        public void OrderingGuaranteed()
        {
            Init();

            Assert.Equal(
                    "zerofirstsecondthird",
                    ParseAndRun("{% usemodule first %}{% usemodule second %}{% usemodule third %}{% writesection test %}{% test %}{% endwritesection %}")
                );
        }

        [Fact]
        public void ConditionalFalse()
        {
            Init();

            Assert.Equal(
                    "nonconditional",
                    ParseAndRun("{% usemodule conditional %}{% assign cond = 0 %}{% writesection test %}{% test %}{% endwritesection %}")
                );
        }

        [Fact]
        public void ConditionalTrue()
        {
            Init();

            Assert.Equal(
                    "nonconditionalconditional",
                    ParseAndRun("{% usemodule conditional %}{% assign cond = 1 %}{% writesection test %}{% test %}{% endwritesection %}")
                );
        }
    }
}
