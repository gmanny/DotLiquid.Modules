using System;
using DotLiquid.Modules.ContextExtractor;
using DotLiquid.Modules.Tags;

namespace DotLiquid.Modules
{
    public static class DotLiquidModules
    {
        private static IContextExtractor contextExtractor;
        public static IContextExtractor ContextExtractor
        {
            get { return contextExtractor; }
        }

        public static void Init(ContextStorageType contextStorage = ContextStorageType.Registers)
        {
            // check that we're initializing for the first time
            if (contextExtractor != null)
            {
                throw new InvalidOperationException("You can't initialize DotLiquid.Modules more than once.");
            }

            // init the context extractor
            ChangeContextStorage(contextStorage);

            // register our tags in the DotLiquid
            Template.RegisterTag<Module>("module");
            Template.RegisterTag<DependsOn>("dependson");
            Template.RegisterTag<Section>("section");
            Template.RegisterTag<UseModule>("usemodule");
            Template.RegisterTag<WriteSection>("writesection");
            Template.RegisterTag<ModulePath>("modulepath");
        }

        public static void ChangeContextStorage(ContextStorageType contextStorage)
        {
            switch (contextStorage)
            {
                case ContextStorageType.Registers:
                    contextExtractor = new RegistersContextExtractor();
                    break;

                case ContextStorageType.Environment:
                    contextExtractor = new EnvironmentContextExtractor();
                    break;

                default:
                    throw new ArgumentException("Unknown context storage type: " + contextStorage, "contextStorage");
            }
        }
    }
}
