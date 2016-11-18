using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Linq;
using Lynicon.Membership;
using Lynicon.Models;
using Lynicon.Utility;

namespace Lynicon.Repositories
{
    /// <summary>
    /// Simple db context to access DbChanges table before data api initialisation
    /// </summary>
    public class PreloadDb : DbContext
    {
        static PreloadDb()
        {
            Database.SetInitializer<PreloadDb>(null);
        }

        public PreloadDb()
            : base(ConfigurationManager.ConnectionStrings["LyniconContent"].ConnectionString)
        { }

        /// <summary>
        /// The records in the DbChanges table
        /// </summary>
        public DbSet<DbChange> DbChanges { get; set; }

        /// <summary>
        /// Build a core Lynicon database
        /// </summary>
        public void EnsureCoreDb()
        {
            bool dbChangesExists = Database
                     .SqlQuery<int?>(@"
                         SELECT 1 FROM sys.tables AS T
                         INNER JOIN sys.schemas AS S ON T.schema_id = S.schema_id
                         WHERE S.Name = 'dbo' AND T.Name = 'DbChanges'")
                     .SingleOrDefault() != null;

            if (!dbChangesExists)
                Database.ExecuteSqlCommand(
                    @"CREATE TABLE [dbo].[DbChanges](
	                    [Id] [int] IDENTITY(1,1) NOT NULL,
	                    [Change] [nvarchar](100) NOT NULL,
	                    [ChangedWhen] [datetime] NOT NULL,
                        CONSTRAINT [PK_DbChanges] PRIMARY KEY CLUSTERED (Id))");
            else if (this.DbChanges.Any(dbc => dbc.Change.StartsWith("LyniconInit ")))
                return;

            Database.ExecuteSqlCommand(
                @"CREATE TABLE [dbo].[ContentItems](
	                [Id] [uniqueidentifier] NOT NULL,
	                [Identity] [uniqueidentifier] NOT NULL,
	                [DataType] [varchar](250) NOT NULL,
	                [Path] [nvarchar](250) NULL,
	                [Locale] [varchar](10) NULL,
	                [Summary] [nvarchar](max) NULL,
	                [Content] [nvarchar](max) NULL,
	                [Title] [nvarchar](250) NULL,
	                [Created] [datetime] NOT NULL,
	                [UserCreated] [varchar](40) NULL,
	                [Updated] [datetime] NOT NULL,
	                [UserUpdated] [varchar](40) NULL,
                 CONSTRAINT [PK_ContentItems] PRIMARY KEY CLUSTERED (Id))");

            DbChanges.Add(new DbChange { Change = "LyniconInit 0.0", ChangedWhen = DateTime.Now });
            SaveChanges();
        }
    }
}
