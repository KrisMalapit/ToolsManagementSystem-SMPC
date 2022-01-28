using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using ToolsManagementSystem.Models;

namespace ToolsManagementSystem.DAL
{
    public class ToolManagementContext : DbContext
    {
        public ToolManagementContext()
            : base("TMSContext_SMPC")
        {
            //Database.SetInitializer<ToolManagementContext>(new DropCreateDatabaseIfModelChanges<ToolManagementContext>());
            
        }


        public DbSet<User> Users { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemDetail> ItemDetails { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<NoSeries> NoSeries { get; set; }
        public DbSet<Contractor> Contractors { get; set; }
        public DbSet<AFEmployeeIssue> AFEmployeeIssues { get; set; }
        public DbSet<AFEmployee> AFEmployees { get; set; }
        public DbSet<AFEmployeeReturn> AFEmployeeReturns { get; set; }
        public DbSet<AFBorrower> AFBorrowers { get; set; }
        public DbSet<AFBorrowerIssue> AFBorrowerIssues { get; set; }
        public DbSet<AFBorrowerReturn> AFBorrowerReturns { get; set; }
        public DbSet<AFFA> AFFAs { get; set; }
        public DbSet<AFFAIssue> AFFAIssues { get; set; }
        public DbSet<AFFAReturn> AFFAReturns { get; set; }
        public DbSet <Log> Logs { get; set; }
        public DbSet<UnitOfMeasure> UnitOfMeasures { get; set; }
        public DbSet<Signatory> Signatories { get; set; }
        public DbSet<ItemLog> ItemLogs { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserDept> UserDepts { get; set; }
        public DbSet<EquipmentType> EquipmentTypes { get; set; }
        public DbSet<GroupAccountability> GroupAccountabilities { get; set; }
        public DbSet<GroupAccountabilityMember> GroupAccountabilityMembers { get; set; }
        public DbSet<VGroupMember> VGroupMembers { get; set; }
        //public DbSet<TempTable> TempTables { get    ; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

        }

        public System.Data.Entity.DbSet<ToolsManagementSystem.Models.View_Model.ItemViewModel> ItemViewModels { get; set; }
       
        //public System.Data.Entity.DbSet<ToolsManagementSystem.Models.AFEmployee> AFEmployees { get; set; }

        //public System.Data.Entity.DbSet<ToolsManagementSystem.Models.Item> Items { get; set; }
    }
}