# EF.Seeder sample

### Seeder for EFCore and other ORMs.

A new seeder mainly for EFCore and also for other ORMs to easily seeding database.     
This repo contains a sample for usage for [Ef.Seeder](https://www.nuget.org/packages/Ef.Seeder)

## Previous EFCore seeder:

EFCore has a seeder that can be found in [this link](https://docs.microsoft.com/en-us/ef/core/modeling/data-seeding). You have to write your seeds in ModelBuilder and seeds will be a part of your migrations what is not an interesting way for seeding
the database. Also that seeds won't increase the primary key and indexes of your tables so you have to increase them manually!!!

## New Seeder that implemented in this package:

After my experience while using EFCore seeder, I decided to write a new implementation for EFCore seeder.

#### New seeder approach:

##### Define your seeders:

```c#
using System.Linq;
using efCoreSeederSample.Models;
using efCoreSeederSample.Seeder.Attributes;
using Microsoft.EntityFrameworkCore;

namespace efCoreSeederSample.Seed
{
    public class DatabaseSeed
    {
        public SeederSampleDbContext DbContext { get; set; }

        public DatabaseSeed(SeederSampleDbContext dbContext)
        {
            DbContext = dbContext;
        }

        [Seeder(1, typeof(Category)/* this parameter is optional*/)]
        public void CategorySeeder()
        {
            for (var i = 1; i <= 3; i++) {
                DbContext.Categories.Add(new Category {
                    Name = $"Category {i}"
                });
            }

            DbContext.SaveChanges();
        }
    }
}
```

As you can see, In this new approach we can easily define a seeder method. Also there is a priority in `SeederAttribute` that determines order of seeder methods, this can helps you to seed your database as you defined your relations.

##### Run your seeders:

1) If you want to seed EFCore:

```c#
new DatabaseSeeder(ServiceProvider, YourDbContext)
        .IsProductionEnvironment(true) // For seed that are needed only in development.
        .EnsureSeeded(isNotEfProcess); // For ef-tool processes
```

2) If you want to seed Other ORMs or don't want to pass DbContext:

```c#
new DatabaseSeeder(ServiceProvider)
        .IsProductionEnvironment(true) // For seed that are needed only in development.
        .EnsureSeeded(isNotEfProcess); // For ef-tool processes
```

You can put above lines in any part of your application, but note that you need to a not disposed `ServiceProvider`.   
In this sample, I put it in `Main` method of `Program` class.    
Also you can create a new `Command` using [AppCommand package](https://www.nuget.org/packages/AppCommand)
to seed you database without source code. Also with [AppCommand package](https://www.nuget.org/packages/AppCommand), you can easily access `ServiceProvider`

### `SeederAttribute` Parameters:

#### `Priority`:

The priority of the seeder. Assume that because of relations you defined in your database, you can't insert data to table `A` before table `B`. Now for seeding your database you can set `Priority` of `BSeeder` to `1` and `Priority` of `ASeeder`
to `2`. Now seeder will run `BSeeder` before `ASeeder` and you have seed in your both tables.

#### `Type` (Optional):

This is type of the entity you want seed. The model should has a `DBSet<>` in your `DbContext`.

##### Note: For other ORMs or other purposes, this parameter is optional and can be null.

##### `Production` (Optional):

When this parameter is `true` means this seeder should only run on your production for seeding your database in production environment. And if the parameter is `false`, the seeder will only run on development environment.

#### `Force` (Optional):

Seeder automatically checks your database's tables. If the model's table has some data in it, seeder won't run to prevent duplicating data. But with setting `Force` Parameter to `true`, you can force seeder to insert data again.

##### Note 1: This parameter is only useful for EFCore orm.

##### Note 2: This parameter only works when you pass `DbContext` object to seeder and `Type` is not null.

## Purpose:

I think this implementation is good start point for a new seeder in EFCore and it's better than previous seeder of EFCore. I hope you like it.

### Donation:

If you like it, you can support me with `USDT`:

1) `TJ57yPBVwwK8rjWDxogkGJH1nF3TGPVq98` for `USDT TRC20`
2) `0x743379201B80dA1CB680aC08F54b058Ac01346F1` for `USDT ERC20`