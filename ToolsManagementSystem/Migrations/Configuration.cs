namespace ToolsManagementSystem.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using ToolsManagementSystem.DAL;

    internal sealed class Configuration : DbMigrationsConfiguration<ToolsManagementSystem.DAL.ToolManagementContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ToolsManagementSystem.DAL.ToolManagementContext context)
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            var baseDir = Path.GetDirectoryName(path) + "\\Migrations\\GroupMemberView.sql";

            context.Database.ExecuteSqlCommand(File.ReadAllText(baseDir));
        }
        
        
    }
}
