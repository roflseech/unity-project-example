using System;
using UnityEngine;
using VContainer;

namespace Game.Common.VContainer
{
    public static class VContainerExtensions
    {
        public static void BindConfigSO<T>(this IContainerBuilder builder, string path) where T : ScriptableObject
        {
            var config = Resources.Load<T>(path);
            if (config == null)
            {
                throw new Exception($"Unable to load config file: {path}");
            }
        
            builder.RegisterInstance<T>(config);
        }

        public static void BindSingleton<T>(this IContainerBuilder builder)
        {
            builder.Register<T>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        }

        public static void BindTransient<T>(this IContainerBuilder builder)
        {
            builder.Register<T>(Lifetime.Transient).AsImplementedInterfaces().AsSelf();
        }
        
        public static void BindComponent<T>(this IContainerBuilder builder, T component)
        {
            builder.RegisterInstance<T>(component).AsImplementedInterfaces().AsSelf();
            builder.RegisterBuildCallback(resolver => resolver.Inject(component));
        }
    }
}