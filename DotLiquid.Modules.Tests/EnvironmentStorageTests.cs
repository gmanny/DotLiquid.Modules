using DotLiquid.Modules.ContextExtractor;
using Xunit;

namespace DotLiquid.Modules.Tests
{
    public class EnvironmentStorageTests : AbstractTests
    {
        public EnvironmentStorageTests() : base(ContextStorageType.Environment) { }

        [Fact]
        public void HasRightContextExtractor()
        {
            Assert.IsType<EnvironmentContextExtractor>(DotLiquidModules.ContextExtractor);
        }
    }
}