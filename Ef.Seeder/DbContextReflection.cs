using System.Linq;
using System.Reflection;

namespace Ef.Seeder
{
    /// <summary>
    /// Some helper methods to access DbContext methods and fields using reflection. 
    /// </summary>
    public class DbContextReflection
    {
        /// <summary>
        /// Gets DbSet method of seeder entity from DbContext.
        /// </summary>
        /// <param name="dbContext">DbContext which entities are defined in it</param>
        /// <param name="seederInfo">The seeder method information.</param>
        /// <returns>DbSet method of seeder entity from DbContext.</returns>
        protected internal virtual MethodInfo GetDbSetMethod(object dbContext, SeederInfo seederInfo)
        {
            return dbContext.GetType().GetMethods()
                .FirstOrDefault(x => x.Name == "Set" && x.GetParameters().Length == 0)?
                .MakeGenericMethod(seederInfo.SeederAttribute.Type);
        }

        /// <summary>
        /// Gets Count method from Queryable that should run with DbSet method to get count of a entity in database.
        /// </summary>
        /// <param name="seederInfo">The seeder method information.</param>
        /// <returns>Count method from Queryable for the entity that should seed.</returns>
        protected internal virtual MethodInfo GetCountMethod(SeederInfo seederInfo)
        {
            return typeof(Queryable)
                .GetMethods()
                .FirstOrDefault(x => x.Name == "Count" && x.GetParameters().Length == 1)?
                .MakeGenericMethod(seederInfo.SeederAttribute.Type);
        }
    }
}