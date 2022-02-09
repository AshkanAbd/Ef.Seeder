using System;

namespace Ef.Seeder.Attributes
{
    /// <summary>
    /// Identifies the seeder method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class SeederAttribute : Attribute
    {
        /// <summary>
        /// Creates a seed
        /// </summary>
        /// <param name="priority">Determines seed priority</param>
        /// <param name="type">Determines the entity that should seed. If this parameter is null then seeder hasn't specific model or entity</param>
        /// <param name="production">Determines entity should seed in Production environment</param>
        /// <param name="force">Force to seed even if entity already has data. This parameter is useful where <see cref="type"/> is not null</param>
        public SeederAttribute(int priority, Type type = null, bool production = false, bool force = false)
        {
            Priority = priority;
            Type = type;
            Production = production;
            Force = force;
        }

        /// <summary>
        /// Determines the entity that should seed. If its null then seeder hasn't specific model or entity.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Determines seed priority
        /// </summary>
        public int Priority { get; }

        /// <summary>
        /// Determines entity should seed in Production environment
        /// </summary>
        public bool Production { get; }

        /// <summary>
        /// Force to seed even if entity already has data, this is useful when <see cref="Type"/> is not null
        /// </summary>
        public bool Force { get; }
    }
}