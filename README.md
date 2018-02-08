# Introduction
This small library is targeted EntityFramework Core. It enables you to specify which relations are to be eagerly fetched where you specify your relations, rather than having to specify this during fetching of data. 

This is particularly useful when you are doing DDD style programming or needing to fetch large object graphs using EF. Since in these situations, for every relation, you want it fetched eagerly.

I use it for my MassTransit saga implementations.



# Usage

Using the framework is a two-step process
  1. During configuration of the model, define which relations are to be eagerly fetched
  2. During a fetch, apply the eager fetch configuration

The configuration is managed by `EagerFetchConfigurator` which hold the eager fetch configuration for an aggregate root.
  
## Usage step 1. Configuring the model
 
During model building, you prefix an `Eager()` to your `HasOne()` and `HasMany()` relations. This is possible due to an extension method acquired by th using-statement. 
Notice one or several configurations must passed around to `Eager()`. This is what allows us to apply fetch configurations in stage 2.

```
using EFCoreEagerFetching.Builders;

public class MyContext : DbContext
{
    private readonly EagerFetchConfigurator cfg;
 
    public MyContext(EagerFetchConfigurator cfg, DbContextOptions options) : base(options)
    {
        this.cfg = cfg;
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ...
        modelBuilder.Entity<Address>().Eager().HasOne(x => x.SomeAreaCode);
    }
}
```

you can also use it with Direct implementations of `IEntityTypeConfiguration`. An example

```
using EFCoreEagerFetching.Builders;

public class SomeMapping : IEntityTypeConfiguration<Person>
{
    private readonly EagerFetchConfigurator cfg;

    public SomeMapping (EagerFetchConfigurator cfg)
    {
        this.cfg = cfg;
    }

    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Eager(cfg).HasMany(x => x.Address4);
    }
}
```

### Recursive data structures
The `HasOne()` and `HasMany()` are overloaded with a max recursion depth parameter. This allows eager fetch of recursive structures down to a finite level.


## Usage step 2. Applying the eagerness
For each query you make, you have to choose which configuration to use or if you want to use eager fetching at all. 
Simply apply it when you have a `IQueryable<T>` by calling 
```
var eagerQuery = cfg.ApplyEagerConfiguration(query)
```


### Usage step 2. Applying the eagerness to MassTransit
The eager fetcher holds a configuration of functions of `IQueryable<T> -> IQueryable<T>`. This means you can simply call them in your `DBContext` class' fetch methods.

Alternatively, using it in MassTransit as follows. When fetching MassTransit state using `EntityFrameworkSagaRepository` you can define a `queryCustomization`. An example

```
var efSagaRepository =
    new EntityFrameworkSagaRepository<MySagaState>(
        sagaDbContextFactory,
        queryCustomization: q => cfg.ApplyEagerConfiguration(q));
```


happy hacking
 Kasper Graversen