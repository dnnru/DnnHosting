﻿#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#endregion

namespace Italliance.Modules.DnnHosting.Components.Resource
{
    public class AssemblyResourceLocator : ILocateResources
    {
        private readonly IDictionary<string, Assembly> _resNames;

        public readonly Func<string, string, bool> MatchingStrategy;
        public readonly Func<string, Assembly, string> NameExtractor;

        public AssemblyResourceLocator(params Assembly[] assemblies)
        {
            _resNames = new Dictionary<string, Assembly>();

            foreach (Assembly assembly in assemblies)
            {
                foreach (string name in assembly.GetManifestResourceNames())
                {
                    _resNames[name] = assembly;
                }
            }

            MatchingStrategy = DefaultMatchingStrategy;
            NameExtractor = DefaultNameExtractor;
        }

        public ResourceReference Locate(string name)
        {
            string key = _resNames.Keys.FirstOrDefault(k => MatchingStrategy(k, name));

            return key == null ? null : new ResourceReference(_resNames[key], key);
        }

        public IEnumerable<ResourceEntry> EnumerateResourceEntries()
        {
            return _resNames.Select(pair => new ResourceEntry
                                            {
                                                ManifestResourceName = pair.Key,
                                                Assembly = pair.Value,
                                                Name = NameExtractor(pair.Key, pair.Value)
                                            });
        }

        public static bool DefaultMatchingStrategy(string key, string name)
        {
            return key.Contains(name);
        }

        public static string DefaultNameExtractor(string manifestResourceName, Assembly assembly)
        {
            var dotIndices = new Stack<int>();

            for (int i = 0; i < manifestResourceName.Length; i++)
            {
                if (manifestResourceName[i] == '.')
                {
                    dotIndices.Push(i);
                }
            }

            if (dotIndices.Count < 2)
            {
                throw new ArgumentException(@"Expected more than one dot in manifest resource name.", nameof(manifestResourceName));
            }

            int extPos = dotIndices.Pop();
            int namePos = dotIndices.Pop();

            return manifestResourceName.Substring(namePos + 1);
        }
    }
}