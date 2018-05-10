using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace WebHost.Infrastructure.Mapping
{
    public class CustomerMap : EntityTypeConfiguration<Customer>
    {
        public CustomerMap()
        {
            this.ToTable("Customers");

            this.HasKey(p => p.Id);
            this.Property(p => p.Id).HasColumnName("ID");

        }
    }
}