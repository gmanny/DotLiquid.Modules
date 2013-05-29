namespace DotLiquid.Modules.ContextExtractor
{
    /// <summary>
    /// Defines the types of storage available for the DotLiquid.Modules context.
    /// </summary>
    public enum ContextStorageType
    {
        /// <summary>
        /// Store DotLiquid.Modules context in the registers of the DotLiquid's context.
        /// 
        /// Registers may (or may not) be cached between executions, this caching may
        /// or may not benefit in your particular case.
        /// </summary>
        Registers,

        /// <summary>
        /// Store DotLiquid.Modules context in the Environments of the DotLiquid's context.
        /// 
        /// Environments are flushed for each render.
        /// </summary>
        Environment
    }
}