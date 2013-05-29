namespace DotLiquid.Modules.ContextExtractor
{
    public interface IContextExtractor
    {
        /// <summary>
        /// Retrieves modules context from the DotLiquid context.
        /// </summary>
        /// <param name="context">DotLiquid context to get the modules context from</param>
        /// <returns>current modules context, or null, if none was found</returns>
        ModulesContext GetModulesContext(Context context);

        /// <summary>
        /// Retrieves modules context from the DotLiquid context, or creates new one, of no context existed before. 
        /// </summary>
        /// <param name="context">DotLiquid context to get the modules context from</param>
        /// <returns>current modules context</returns>
        ModulesContext GetOrAddModulesContext(Context context);
    }
}