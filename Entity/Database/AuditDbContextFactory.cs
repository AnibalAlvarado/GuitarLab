using Entity.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Database
{
    public class AuditDbContextFactory
    {
        private readonly DatabaseFactoryProvider _provider;
        private readonly IConfiguration _configuration;

        public AuditDbContextFactory(DatabaseFactoryProvider provider, IConfiguration configuration)
        {
            _provider = provider;
            _configuration = configuration;
        }

        public AuditDbContext Create()
        {
            var providerName = _configuration["AuditDatabaseProvider"] ?? _configuration["DatabaseProvider"];
            var connectionString = _configuration.GetConnectionString(providerName);

            var optionsBuilder = new DbContextOptionsBuilder<AuditDbContext>();
            var factory = _provider.GetFactory(providerName);
            factory.Configure(optionsBuilder, connectionString);

            return new AuditDbContext(optionsBuilder.Options);
        }
    }
}
