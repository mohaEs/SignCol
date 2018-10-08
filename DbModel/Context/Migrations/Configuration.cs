namespace DbModel.Context.Migrations
{
    using DomainClasses.Entities;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    // LocalDB Configuration
    public sealed class Configuration : CreateDatabaseIfNotExists<DbModel.Context.MyDbContext>
    {
        //public Configuration()
        //{
        //    AutomaticMigrationsEnabled = true;
        //    AutomaticMigrationDataLossAllowed = true;
        //}

        protected override void Seed(DbModel.Context.MyDbContext context)
        {
            context.Options.AddOrUpdate(op => new { op.Name, op.Value },
                new Option { Name = "FileUrl", Value = "c:" });

            context.SaveChanges();

            base.Seed(context);
        }
    }
}
