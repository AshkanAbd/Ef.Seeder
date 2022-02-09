using System;
using System.Linq;
using System.Reflection;

namespace Ef.Seeder
{
    /// <summary>
    /// Internal static class that used for creating instance from seeder classes.
    /// </summary>
    internal static class InstanceCreator
    {
        /// <summary>
        /// Creates an instance from the class which seeder is defined in it.
        /// </summary>
        /// <param name="seederType">The seeder class that should create an instance from it</param>
        /// <param name="serviceProvider">A service provider that will helps to create an instance</param>
        /// <returns>An instance of the class which seeder is defined in it.</returns>
        /// <exception cref="Exception">Throw an exception if can't create an instance from the seeder class</exception>
        internal static object GetInstance(Type seederType, IServiceProvider serviceProvider)
        {
            var constructorParameters = GetConstructorInfo(seederType);
            if (constructorParameters == null) {
                throw new Exception("Can't find suitable constructor for seeder class");
            }

            var constructorArgs = CreateConstructorParameters(constructorParameters, serviceProvider);
            return Activator.CreateInstance(seederType, constructorArgs);
        }

        /// <summary>
        /// Gets ConstructorInfo of the class which seeder method is defined in it.
        /// </summary>
        /// <param name="seederType">The class which seeder method is defined in it.</param>
        /// <returns>ConstructorInfo of the class which seeder method is defined in it.</returns>
        private static ConstructorInfo GetConstructorInfo(Type seederType)
        {
            return seederType.GetConstructors().Length < 1 ? null : seederType.GetConstructors()[0];
        }

        /// <summary>
        /// Creates parameters from Dependency Injection for constructor of the class which seeder method is defined in it.
        /// </summary>
        /// <param name="constructor">Constructor of the class which seeder method is defined in it.</param>
        /// <param name="serviceProvider">A service provider that will helps to create an instance</param>
        /// <returns>Parameters for constructor of the class which seeder method is defined in it.</returns>
        private static object[] CreateConstructorParameters(MethodBase constructor, IServiceProvider serviceProvider)
        {
            return constructor.GetParameters()
                .Select(parameter => serviceProvider.GetService(parameter.ParameterType))
                .ToArray();
        }
    }
}