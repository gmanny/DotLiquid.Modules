using System;
using Xunit;

namespace DotLiquid.Modules.Tests
{
    public class Tests
    {
        [Fact]
        public void DoesntAllowMultipleInitializations()
        {
            // init for the first time
            DotLiquidModules.Init();

            // assert
            Assert.Throws<InvalidOperationException>(() => DotLiquidModules.Init());
        }
    }
}