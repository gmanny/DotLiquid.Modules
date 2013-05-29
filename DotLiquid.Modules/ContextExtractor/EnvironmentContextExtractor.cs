namespace DotLiquid.Modules.ContextExtractor
{
    public class EnvironmentContextExtractor : IContextExtractor
    {
        private const string EnvironmentKey = "modules.Context";

        public ModulesContext GetModulesContext(Context context)
        {
            // check that we've got any environments
            if (context.Environments.Count == 0)
            {
                return null;
            }

            // scan environments
            for (int i = 0; i < context.Environments.Count; i++)
            {
                // get current environment
                Hash currentEnv = context.Environments[i];

                // check that it has our context
                object modContext;
                if (!currentEnv.TryGetValue(EnvironmentKey, out modContext))
                {
                    continue;
                }

                // check if it's the right type
                ModulesContext casted = modContext as ModulesContext;
                if (casted == null)
                {
                    continue;
                }

                return casted;
            }

            return null;
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

            // create new environment, and add it to the DotLiquid context
            context.Environments.Add(new Hash { { EnvironmentKey, (currentContext = new ModulesContext()) } });

            // return newly created context
            return currentContext;
        }
    }
}