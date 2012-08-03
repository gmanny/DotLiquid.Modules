using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid.Modules.Tags;

namespace DotLiquid.Modules
{
    public static class DotLiquidModules
    {
        public static void Init()
        {
            Template.RegisterTag<Module>("module");
            Template.RegisterTag<DependsOn>("dependson");
            Template.RegisterTag<Section>("section");
            Template.RegisterTag<UseModule>("usemodule");
            Template.RegisterTag<WriteSection>("writesection");
            Template.RegisterTag<ModulePath>("modulepath");
        }
    }
}
