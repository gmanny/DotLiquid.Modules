using DotLiquid.Modules.ContextExtractor;
using Xunit;

namespace DotLiquid.Modules.Tests
{
    public class RegisterStorageTests : AbstractTests
    {
        public RegisterStorageTests() : base(ContextStorageType.Registers) { }

        [Fact]
        public void HasRightContextExtractor()
        {
            Assert.IsType<RegistersContextExtractor>(DotLiquidModules.ContextExtractor);
        }
    }
}