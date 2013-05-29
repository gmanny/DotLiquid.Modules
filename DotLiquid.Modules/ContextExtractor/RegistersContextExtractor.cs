namespace DotLiquid.Modules.ContextExtractor
{
    public class RegistersContextExtractor : IContextExtractor
    {
        private const string RegisterName = "modules";

        public ModulesContext GetModulesContext(Context context)
        {
            return context.Registers[RegisterName] as ModulesContext;
        }

        public ModulesContext GetOrAddModulesContext(Context context)
        {
            // try to get the context
            ModulesContext currentContext = GetModulesContext(context);

            // check that we've found it
            if (currentContext != null)
            {
                return currentContext;
            }

            // create new context, and store it to the DotLiquid context
            context.Registers[RegisterName] = currentContext = new ModulesContext();

            // return newly created context
            return currentContext;
        }
    }
}