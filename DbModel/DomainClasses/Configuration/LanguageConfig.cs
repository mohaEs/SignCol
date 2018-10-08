using DbModel.DomainClasses.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbModel.DomainClasses.Configuration
{
    public class LanguageConfig : EntityTypeConfiguration<Languages>
    {
        public LanguageConfig()
        {
            HasMany(a => a.Words).WithRequired(a => a.Languages).HasForeignKey(a => a.lang_id)
                .WillCascadeOnDelete(true);

        }
    }
}
