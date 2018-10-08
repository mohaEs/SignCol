using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using EFSecondLevelCache;
using System.Threading.Tasks;
using System.Windows;
using SQLite;
using System.Data.SQLite;
using DbModel.DomainClasses.Entities;
using System.Data.Entity.ModelConfiguration.Conventions;
using DbModel.DomainClasses.Configuration;
using DbModel.Context.Migrations;
using SQLite.CodeFirst;

namespace DbModel.Context
{
    public class MyDbContext : DbContext, IUnitOfWork
    {
        // SQLite
        public MyDbContext() : base("constr")
        {
            
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new Migrations.SqliteConfiguration(modelBuilder));
        }


        // LocalDB
        /*public MyDbContext() : base(nameOrConnectionString: BuildConnectionString)
        {
            // برای اعمال غیر متصل
            this.Configuration.ProxyCreationEnabled = false;
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.AutoDetectChangesEnabled = false;


            this.Configuration.LazyLoadingEnabled = false;

            //Database.SetInitializer<MyDbContext>(null);
            //Database.SetInitializer<MyDbContext>(new CreateDatabaseIfNotExists<MyDbContext>());
            Database.SetInitializer<MyDbContext>(new Migrations.Configuration());

        }
        //public MyDbContext DbContext = new MyDbContext(entityConnectionString);
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new LanguageConfig());
            modelBuilder.Configurations.Add(new WordsConfig());
            modelBuilder.Configurations.Add(new VideoConfig());
            modelBuilder.Configurations.Add(new OptionConfig());
            modelBuilder.Configurations.Add(new UserConfig());
            base.OnModelCreating(modelBuilder);
        }*/





        public DbSet<Languages> Languages { get; set; }
        public DbSet<Words> Words { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<User> Users { get; set; }

        private static string BuildConnectionString
        {
            get
            {
                string ConnectionString = "";
                string assemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"App_Data\Database.mdf");
                string sqlitepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"App_Data\Database.sqlite");


                ConnectionString = LocalDbConnection(@"(LocalDb)\MSSQLLocalDB", assemblyPath, "", "");
                //ConnectionString = SQLExpressConnection(@".\SQLEXPRESS", assemblyPath, "", "");
                //ConnectionString = SQLiteConnection(sqlitepath, "", "");

                return ConnectionString;
            }
        }
        private static string LocalDbConnection(string serverName, string AttachDBFilename, string UserID, string Password)
        {
            // Specify the provider name, server and database.
            //string providerName = "System.Data.SqlClient";
            //string serverName = @"(LocalDb)\MSSQLLocalDB";
            //string databaseName = "Database";
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();

            // Set the properties for the data source.
            sqlBuilder.DataSource = serverName;
            //sqlBuilder.InitialCatalog = databaseName;
            sqlBuilder.AttachDBFilename = AttachDBFilename;/* @"|DataDirectory|\App_Data\Database.mdf";*/
            sqlBuilder.UserID = UserID;// "";
            sqlBuilder.Password = Password;// "";
            sqlBuilder.IntegratedSecurity = true;

            return sqlBuilder.ToString();
        }

        private static string SQLExpressConnection(string serverName, string AttachDBFilename, string UserID, string Password)
        {
            // Specify the provider name, server and database.
            //string providerName = "System.Data.SqlClient";
            //string serverName = @".\SQLEXPRESS";
            //string databaseName = "Database";
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();

            // Set the properties for the data source.
            sqlBuilder.DataSource = serverName;
            sqlBuilder.AttachDBFilename = AttachDBFilename;// @"|DataDirectory|\Database.mdf"; // or sqlBuilder.InitialCatalog = databaseName;
            sqlBuilder.MultipleActiveResultSets = true;
            sqlBuilder.UserInstance = true;
            sqlBuilder.UserID = UserID;
            sqlBuilder.Password = Password;
            sqlBuilder.IntegratedSecurity = true;

            return sqlBuilder.ToString();
        }

        private static string SQLServerConnection()
        {
            string serverName = "ServerName";
            string instanceName = "InstanceName";
            string databaseName = "Database";
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();

            // Set the properties for the data source.
            /*sqlBuilder.DataSource = serverName;
            sqlBuilder.InitialCatalog = databaseName;
            sqlBuilder.IntegratedSecurity = false;
            sqlBuilder.UserID = "";
            sqlBuilder.Password = "";
            sqlBuilder.MultipleActiveResultSets = true;*/

            // or
            sqlBuilder.DataSource = serverName + @"\" + instanceName;
            sqlBuilder.InitialCatalog = databaseName;
            sqlBuilder.IntegratedSecurity = true;
            sqlBuilder.MultipleActiveResultSets = true;

            return sqlBuilder.ToString();
        }

        private static string SQLServerCompactConnection()
        {
            // Specify the provider name, server and database.
            //string providerName = "System.Data.SqlClient";
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
            sqlBuilder.DataSource = @"|DataDirectory|\Database.sdf";
            sqlBuilder.PersistSecurityInfo = false;
            return sqlBuilder.ToString();
        }

        private static string SQLiteConnection(string AttachDBFilename, string UserID, string Password)
        {
            // Specify the provider name, server and database.
            string providerName = "System.Data.SQLite";
            SQLiteConnectionStringBuilder conString = new SQLiteConnectionStringBuilder();
            conString.DataSource = @".\phonebook.sqlite";// AttachDBFilename;
        //    conString.ForeignKeys = false;
        //    conString.DefaultTimeout = 5000;
        //    conString.SyncMode = SynchronizationModes.Off;
        //    conString.JournalMode = SQLiteJournalModeEnum.Memory;
        //    conString.PageSize = 65536;
        //    conString.CacheSize = 16777216;
        //    conString.FailIfMissing = false;
        //    conString.ReadOnly = false;
            return conString.ToString();
        }





        #region IUnitOfWork Members
        public new IDbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        public int SaveAllChanges(bool invalidateCacheDependencies)
        {
            return SaveChanges(invalidateCacheDependencies);
        }

        public async Task<int> SaveAllChangesAsync(bool invalidateCacheDependencies)
        {
            return await SaveChangesAsync(invalidateCacheDependencies);
        }

        public int SaveChanges(bool invalidateCacheDependencies)
        {
            var changedEntityNames = this.GetChangedEntityNames();
            var result = base.SaveChanges();
            if (invalidateCacheDependencies)
            {
                new EFCacheServiceProvider().InvalidateCacheDependencies(changedEntityNames);
            }
            return result;
        }

        public async Task<int> SaveChangesAsync(bool invalidateCacheDependencies)
        {
            var changedEntityNames = this.GetChangedEntityNames();
            var result = await base.SaveChangesAsync();
            if (invalidateCacheDependencies)
            {
                new EFCacheServiceProvider().InvalidateCacheDependencies(changedEntityNames);
            }
            return result;
        }

        public void MarkAsAdded<TEntity>(TEntity entity) where TEntity : class
        {
            Entry(entity).State = EntityState.Added;
        }
        public void MarkAsDeleted<TEntity>(TEntity entity) where TEntity : class
        {
            Entry(entity).State = EntityState.Deleted;
        }

        public IEnumerable<TEntity> AddThisRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            return ((DbSet<TEntity>)this.Set<TEntity>()).AddRange(entities);
        }

        public void MarkAsChanged<TEntity>(TEntity entity) where TEntity : class
        {
            Entry(entity).State = EntityState.Modified;
        }

        public IList<T> GetRows<T>(string sql, params object[] parameters) where T : class
        {
            return Database.SqlQuery<T>(sql, parameters).ToList();
        }

        public void ForceDatabaseInitialize()
        {
            this.Database.Initialize(force: true);
        }
        #endregion

    }

}
