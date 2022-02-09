using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Ef.Seeder.Attributes;

namespace Ef.Seeder
{
    /// <summary>
    /// Seeds database.
    /// </summary>
    public class DatabaseSeeder
    {
        /// <summary>
        /// Creates a DatabaseSeeder that seeds database.
        /// </summary>
        /// <param name="serviceProvider">IServiceCollection which DbContext is registered in it</param>
        /// <param name="dbContext">DbContext which entities are defined in it, this parameter is optional and only for EFCore</param>
        public DatabaseSeeder(IServiceProvider serviceProvider, object dbContext = null)
        {
            ServiceProvider = serviceProvider;
            DbContext = dbContext;
            IsProduction = false;
            DbContextReflection = new DbContextReflection();
        }

        private object DbContext { get; }
        private bool IsProduction { get; set; }
        private IServiceProvider ServiceProvider { get; }
        public DbContextReflection DbContextReflection { get; set; }

        /// <summary>
        /// Sets environment is production for seeder.
        /// </summary>
        /// <param name="isProduction">Is seeder environment is production or not</param>
        public DatabaseSeeder IsProductionEnvironment(bool isProduction)
        {
            IsProduction = isProduction;
            return this;
        }


        /// <summary>
        /// Runs seeders.
        /// NOTE: Seeders SHOULD NOT run on a dotnet-ef process.
        /// </summary>
        /// <param name="isNotEfProcess">Determines the process is dotnet-ef process or not.</param>
        public void EnsureSeeded(bool isNotEfProcess)
        {
            if (!isNotEfProcess) return;

            var seeders = GetSeeders();


            var stopWatch = new Stopwatch();
            seeders.ForEach(seederInfo => {
                if (!seederInfo.IsValidEnvironment(IsProduction)) {
                    return;
                }

                var seederInstance = InstanceCreator.GetInstance(seederInfo.Type, ServiceProvider);
                if (seederInstance == null) {
                    return;
                }

                if (!seederInfo.IsNullTypeSeeder() && DbContext != null) {
                    var dataCount = GetEntityCountFromDatabase(seederInfo);

                    if (!seederInfo.ShouldRun(dataCount)) {
                        return;
                    }
                }

                Console.WriteLine($"Seeding {seederInfo.GetSeederName()} from {seederInfo.MethodInfo.Name}...");
                stopWatch.Restart();

                if (seederInfo.IsAsync()) {
                    Console.WriteLine($"{seederInfo.GetSeederName()}: Async seeders are not supported");
                }
                else {
                    InvokeSeeder(seederInfo, seederInstance);
                }

                Console.WriteLine(seederInfo.GetSeederName() + " from " +
                                  seederInfo.MethodInfo.Name + " in " +
                                  seederInfo.Type.Name + " class seeded in " +
                                  stopWatch.ElapsedMilliseconds + "ms.");
                stopWatch.Stop();
            });
        }

        /// <summary>
        /// Invokes a non-async seeder method.
        /// </summary>
        /// <param name="seederInfo">The seeder method information.</param>
        /// <param name="seederInstance">An instance of the class which seeder is defined in it.</param>
        protected virtual void InvokeSeeder(SeederInfo seederInfo, object seederInstance)
        {
            seederInfo.MethodInfo.Invoke(seederInstance, null);
        }

        /// <summary>
        /// Gets entity count from database.
        /// </summary>
        /// <param name="seederInfo">The seeder method information.</param>
        /// <returns>Returns entity count</returns>
        /// <exception cref="Exception">Throws exception if can't retrieve entity count</exception>
        protected virtual int GetEntityCountFromDatabase(SeederInfo seederInfo)
        {
            try {
                var dbSetMethod = DbContextReflection.GetDbSetMethod(DbContext, seederInfo);
                if (dbSetMethod == null) throw new Exception("Can't access to DbSet.");

                var dbSet = dbSetMethod.Invoke(DbContext, null);
                if (dbSet == null) throw new Exception("Can't invoke DbSet.");

                var countMethod = DbContextReflection.GetCountMethod(seederInfo);
                if (countMethod == null) throw new Exception("Can't invoke Count() method in DbSet");

                var dataCount = (int) countMethod.Invoke(dbSet, new[] {dbSet});

                return dataCount;
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                Console.WriteLine(
                    $"Can't invoke Count() method for {seederInfo.SeederAttribute.Type} in {seederInfo.Type} from {DbContext}"
                );
                throw;
            }
        }

        /// <summary>
        /// Gets list of all seeders methods in AppDomain.CurrentDomain assembly
        /// </summary>
        /// <returns>List of seeders which are define in AppDomain.CurrentDomain assembly</returns>
        protected virtual List<SeederInfo> GetSeeders()
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass)
                .SelectMany(x => x.GetMethods())
                .Where(x => x.GetCustomAttributes(typeof(SeederAttribute), false).FirstOrDefault() != null)
                .Select(x => new SeederInfo(
                    x.DeclaringType,
                    x,
                    x.GetCustomAttribute(typeof(SeederAttribute)) as SeederAttribute
                )).OrderBy(x => x.SeederAttribute.Priority)
                .ToList();
        }
    }
}