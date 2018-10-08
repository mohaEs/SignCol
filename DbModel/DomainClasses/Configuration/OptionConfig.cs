using DbModel.DomainClasses.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbModel.DomainClasses.Configuration
{
    public class OptionConfig : EntityTypeConfiguration<Option>
    {
        public OptionConfig()
        {

        }
    }
}