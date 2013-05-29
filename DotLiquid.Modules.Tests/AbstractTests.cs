using System;
using System.IO;
using System.Reflection;
using System.Text;
using DotLiquid.Modules.ContextExtractor;
using DotLiquid.Modules.Tests.Util;
using Xunit;

namespace DotLiquid.Modules.Tests
{
    public abstract class AbstractTests
    {
        protected AbstractTests(ContextStorageType contextStorage)
        {
            try
            {
                // init modules
                DotLiquidModules.Init(contextStorage);
            }
            catch (InvalidOperationException)
            {
                // change context storage scheme, if modules were already initialized
                DotLiquidModules.ChangeContextStorage(contextStorage);
            }
        }

        private Context GetResourceContext()
        {
            Context ctx = new Context();
            ctx.Registers["file_system"] = new ResourceFileSystem("DotLiquid.Modules.Tests.Resources", Assembly.GetExecutingAssembly());

            return ctx;
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
            Assert.Equal(
                    "te-st",
                    ParseAndRun("{% usemodule test %}{% writesection test %}{% test %}{% endwritesection %}")
                );
        }

        [Fact]
        public void TwoSimilarSections()
        {
            Assert.Equal(
                    "te-stte-st",
                    ParseAndRun("{% usemodule test %}{% writesection test %}{% test %}{% endwritesection %}{% writesection test %}{% test %}{% endwritesection %}")
                );
        }

        [Fact]
        public void BasicDependencies()
        {
            Assert.Equal(
                    "masterslave",
                    ParseAndRun("{% usemodule slave %}{% writesection test %}{% test %}{% endwritesection %}")
                );
        }

        [Fact]
        public void OrderingGuaranteed()
        {
            Assert.Equal(
                    "zerofirstsecondthird",
                    ParseAndRun("{% usemodule first %}{% usemodule second %}{% usemodule third %}{% writesection test %}{% test %}{% endwritesection %}")
                );
        }

        [Fact]
        public void ConditionalFalse()
        {
            Assert.Equal(
                    "nonconditional",
                    ParseAndRun("{% usemodule conditional %}{% assign cond = 0 %}{% writesection test %}{% test %}{% endwritesection %}")
                );
        }

        [Fact]
        public void ConditionalTrue()
        {
            Assert.Equal(
                    "nonconditionalconditional",
                    ParseAndRun("{% usemodule conditional %}{% assign cond = 1 %}{% writesection test %}{% test %}{% endwritesection %}")
                );
        }
    }
}
