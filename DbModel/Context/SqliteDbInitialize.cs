using DbModel.DomainClasses.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
//using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbModel.Context
{
    public class SqliteDbInitialize// : SQLite.CodeFirst.SqliteCreateDatabaseIfNotExists<MyDbContext>
    {
        //public SqliteDbInitialize(DbModelBuilder modelBuilder) : base(modelBuilder)
        //{

        //}

        //protected override void Seed(MyDbContext context)
        //{
        //    context.Options.AddOrUpdate(op => new { op.Name, op.Value },
        //        new Option { Name = "FileUrl", Value = "c:" });

        //    context.SaveChanges();

        //    base.Seed(context);
        //}
    }
}
