using Microsoft.EntityFrameworkCore;

namespace TSSLogParser.EFCore
{
    public partial class TSSParserContext : DbContext
    {
        private string connectionString;

        public TSSParserContext(string ConnectionString)
        {
            connectionString = ConnectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(connectionString, options => options.CommandTimeout(600));
            }
        }
    }
}
