using System;
using System.Collections.Generic;
using System.Reflection;

namespace MUT
{
    public static class MyPlugins<T>
    {
        public static ICollection<T> GetPlugins(string[] dllFileNames)
        {
            ICollection<Assembly> assemblies = new List<Assembly>(dllFileNames.Length);
            foreach (string dllFile in dllFileNames)
            {
                AssemblyName an = AssemblyName.GetAssemblyName(dllFile);
                Assembly assembly = Assembly.Load(an);
                assemblies.Add(assembly);
            }

            ICollection<Type> pluginTypes = new List<Type>();
            foreach (Assembly assembly in assemblies)
            {
                if (assembly != null)
                {
                    Type[] types = assembly.GetTypes();
                    foreach (Type type in types)
                    {
                        if (type.IsInterface || type.IsAbstract)
                        {
                            continue;
                        }
                        else
                        {
                            if (type.GetInterface(typeof(T).FullName) != null)
                            {
                                pluginTypes.Add(type);
                            }
                        }
                    }
                }
            }

            ICollection<T> plugins = new List<T>(pluginTypes.Count);
            foreach (Type type in pluginTypes)
            {
                T plugin = (T)Activator.CreateInstance(type);
                plugins.Add(plugin);
            }
            return plugins;
        }
    }
}
