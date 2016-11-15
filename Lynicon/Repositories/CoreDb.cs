using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Attributes;
using Lynicon.Collation;
using Lynicon.DataSources;
using Lynicon.Linq;
using Lynicon.Membership;
using Lynicon.Models;
using Lynicon.Utility;

namespace Lynicon.Repositories
{
    /// <summary>
    /// The DbContext used for built in Data API requests, Basic and Content persistence models
    /// </summary>
    public class CoreDb : DbContext
    {
        static CoreDb()
        {
            Database.SetInitializer<CoreDb>(null);
        }

        public CoreDb()
            : base(ConfigurationManager.ConnectionStrings["LyniconContent"].ConnectionString)
        {
            this.Configuration.ProxyCreationEnabled = Repository.Instance.EFProxyCreationEnabled;
        }
        public CoreDb(string nameOrCs)
            : base(nameOrCs)
        {
            this.Configuration.ProxyCreationEnabled = Repository.Instance.EFProxyCreationEnabled;
        }

        /// <summary>
        /// Get a DbQuery (in the extended type) for a given base type
        /// </summary>
        /// <typeparam name="TBase">The base type</typeparam>
        /// <returns>A DbQuery against the underlying database</returns>
        public DbQuery CompositeSet<TBase>()
        {
            return CompositeSet(typeof(TBase));
        }
        /// <summary>
        /// Get a DbQuery (in the extended type) for a given base type
        /// </summary>
        /// <param name="tBase">The base type</param>
        /// <returns>A DbQuery against the underlying database</returns>
        public DbQuery CompositeSet(Type tBase)
        {
            if (!CompositeTypeManager.Instance.BaseTypes.Contains(tBase))
                throw new Exception("No composite of base type " + tBase.FullName);

            Type extType = CompositeTypeManager.Instance.ExtendedTypes[tBase];
            DbQuery q = Set(extType);
            foreach (var inclPi in extType.GetProperties().Where(pi => pi.GetCustomAttribute<AlwaysIncludeAttribute>() != null))
                q = q.Include(inclPi.Name);

            return q;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (!Collator.Instance.RepositoryBuilt)
                throw new Exception("In CoreDb.OnModelCreating because there was a use of CoreDb before repository was built");

            var requiredTypes = ContentTypeHierarchy.AllContentTypes
                .Select(ct => Collator.Instance.ContainerType(ct))
                .Distinct()
                .Where(crt => Repository.Instance.Registered(crt).DataSourceFactory is CoreDataSourceFactory)
                .ToList();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(ass => !ass.FullName.StartsWith("CompositeClasses"))
                .ToList();
            var ignoreTypes = assemblies
                .SelectMany(ass => ass.GetTypes())
                .Where(type => requiredTypes.Any(rt => type.IsSubclassOf(rt)));

            modelBuilder.Ignore(ignoreTypes);

            var entityMethod = typeof(DbModelBuilder).GetMethod("Entity");

            foreach (var extendedType in CompositeTypeManager.Instance.ExtendedTypes.Where(kvp => requiredTypes.Contains(kvp.Value)))
            {
                // below code runtime equivalent of
                // typeConfig = modelBuilder.Entity<extendedType.Value>();
                // typeConfig.HasKey<Guid>(x => x.Id);

                var typeConfig = entityMethod.MakeGenericMethod(extendedType.Value)
                    .Invoke(modelBuilder, new object[] { });
                //var hasKey = typeConfig.GetType().GetMethod("HasKey");
                
                //ParameterExpression x = Expression.Parameter(extendedType.Value, "x");
                //PropertyInfo piId = extendedType.Value.GetProperty("Id");
                //LambdaExpression le = System.Linq.Dynamic.DynamicExpression.ParseLambda(
                //    new ParameterExpression[] { x }, piId.PropertyType, "x.Id");

                //hasKey.MakeGenericMethod(piId.PropertyType).Invoke(typeConfig, new object[] { le });

                string tableName = null;
                var tableNamePi = extendedType.Key.GetProperty("TableName", BindingFlags.Static | BindingFlags.Public);
                if (tableNamePi != null)
                    tableName = (string)tableNamePi.GetValue(null, null);
                if (tableName == null)
                {
                    var tableAttr = extendedType.Key.GetCustomAttribute<TableAttribute>();
                    if (tableAttr != null)
                        tableName = tableAttr.Name;
                    else
                    {
                        tableName = extendedType.Key.Name;
                        if (!tableName.EndsWith("s")) tableName += "s";
                    }
                }

                var toTable = typeConfig.GetType().GetMethod("ToTable", new Type[] { typeof(string) });
                toTable.Invoke(typeConfig, new object[] { tableName });
            }
        }
    }
}
