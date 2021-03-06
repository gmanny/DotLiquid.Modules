﻿using System.Collections.Generic;
using DotLiquid.Modules.Tags;

namespace DotLiquid.Modules
{
    public class ModulesContext
    {
        private readonly Dictionary<string, Module> _moduleIndex = new Dictionary<string, Module>();

        public Dictionary<string, Module> ModuleIndex { get { return _moduleIndex; } }

        private readonly List<Module> _dependencyOrder = new List<Module>();

        public List<Module> DependencyOrder { get { return _dependencyOrder; } }

        public string PathPrefix { get; set; }
    }
}
