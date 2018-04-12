﻿using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Nop.Core;
using Nop.Core.Infrastructure;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Data.Tests
{
    [TestFixture]
    public abstract class PersistenceTest
    {
        protected NopObjectContext context;

        [SetUp]
        public virtual void SetUp()
        {
            //TODO fix compilation warning (below)
            #pragma warning disable 0618
            Database.DefaultConnectionFactory = new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0");
            context = new NopObjectContext(GetTestDbName());
            context.Database.Delete();
            context.Database.Create();
        }

        protected string GetTestDbName()
        {
            var hostingEnvironment = MockRepository.GenerateMock<IHostingEnvironment>();
            hostingEnvironment.Expect(x => x.ContentRootPath).Return(System.Reflection.Assembly.GetExecutingAssembly().Location);
            hostingEnvironment.Expect(x => x.WebRootPath).Return(System.IO.Directory.GetCurrentDirectory());
            var fileProvider = new NopFileProvider(hostingEnvironment);
            
            var testDbName = "Data Source=" + fileProvider.GetAbsolutePath() + @"\\Nop.Data.Tests.Db.sdf;Persist Security Info=False";
            return testDbName;
        }        
        
        /// <summary>
        /// Persistance test helper
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="disposeContext">A value indicating whether to dispose context</param>
        protected T SaveAndLoadEntity<T>(T entity, bool disposeContext = true) where T : BaseEntity
        {
            context.Set<T>().Add(entity);
            context.SaveChanges();

            object id = entity.Id;

            if (disposeContext)
            {
                context.Dispose();
                context = new NopObjectContext(GetTestDbName());
            }

            var fromDb = context.Set<T>().Find(id);
            return fromDb;
        }
    }
}
