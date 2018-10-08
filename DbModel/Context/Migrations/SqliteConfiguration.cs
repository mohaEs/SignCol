using DbModel.DomainClasses.Entities;
using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SQLite.EF6.Migrations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbModel.Context.Migrations
{
    // SQLite Configuration
    //public sealed class ContextMigrationConfiguration// : DbMigrationsConfiguration<MyDbContext>
    //{
        /*public ContextMigrationConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            SetSqlGenerator("System.Data.SQLite", new SQLiteMigrationSqlGenerator());
        }*/
    //}
    public sealed class SqliteConfiguration : SqliteCreateDatabaseIfNotExists<DbModel.Context.MyDbContext>
    {

        public SqliteConfiguration(DbModelBuilder modelBuilder) : base(modelBuilder)
        {

        }
        protected override void Seed(DbModel.Context.MyDbContext context)
        {
            string assemblyPath = AppDomain.CurrentDomain.BaseDirectory+@"Files";
            context.Options.AddOrUpdate(op => new { op.Name, op.Value },
                new Option { Name = "FileUrl", Value = assemblyPath/*"c:"*/ });

            context.SaveChanges();

            base.Seed(context);
        }
    }

}
