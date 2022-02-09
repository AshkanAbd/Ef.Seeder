using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Ef.Seeder.Attributes;

namespace Ef.Seeder
{
    /// <summary>
    /// Holds seeders information
    /// </summary>
    public class SeederInfo
    {
        /// <summary>
        /// Creates a seeder information object
        /// </summary>
        /// <param name="type">The class that has some seeder methods.</param>
        /// <param name="methodInfo">The method that seeds the entity.</param>
        /// <param name="seederAttribute">The attribute that defines in seeder method.</param>
        public SeederInfo(Type type, MethodInfo methodInfo, SeederAttribute seederAttribute)
        {
            Type = type;
            MethodInfo = methodInfo;
            SeederAttribute = seederAttribute;
        }

        internal Type Type { get; }
        internal MethodInfo MethodInfo { get; }
        internal SeederAttribute SeederAttribute { get; }

        /// <summary>
        /// Determine a seeder is async or not
        /// </summary>
        /// <returns>true if seeder is an async method. false is seeder isn't an async method.</returns>
        internal bool IsAsync()
        {
            return MethodInfo.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) != null;
        }

        /// <summary>
        /// Determines that the seeder and application environment are compatible   
        /// </summary>
        /// <param name="isProduction">application environment</param>
        /// <returns>Returns true if the seeder and application environment are compatible</returns>
        protected internal virtual bool IsValidEnvironment(bool isProduction)
        {
            return !(isProduction && !SeederAttribute.Production);
        }

        /// <summary>
        /// Checks that the seeder should run with given entity count in database.
        /// </summary>
        /// <param name="count">Seeder entity count in database</param>
        /// <returns>Returns true if Checks the seeder should run with given entity count in database.</returns>
        protected internal virtual bool ShouldRun(long count)
        {
            return !(count != 0 && !SeederAttribute.Force);
        }

        /// <summary>
        /// Determines that seeder type is null or not.
        /// </summary>
        /// <returns>Returns true if seeder type is null.</returns>
        protected internal virtual bool IsNullTypeSeeder()
        {
            return SeederAttribute.Type == null;
        }

        /// <summary>
        /// Gets seeder name based on seeder type.
        /// </summary>
        /// <returns>Returns <see cref="MethodInfo"/> name if seeder <see cref="IsNullTypeSeeder"/>, otherwise returns seeder entity name</returns>
        protected internal virtual string GetSeederName()
        {
            return IsNullTypeSeeder() ? MethodInfo.Name : SeederAttribute.Type.Name;
        }
    }
}