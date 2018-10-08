using DbModel.DomainClasses.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbModel.DomainClasses.Configuration
{
    public class UserConfig : EntityTypeConfiguration<User>
    {
        public UserConfig()
        {
            //HasMany(a => a.Words).WithRequired(a => a.User).HasForeignKey(a => a.User_id)
            //    .WillCascadeOnDelete(true);
        }
    }
}