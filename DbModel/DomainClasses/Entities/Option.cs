using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbModel.DomainClasses.Entities
{
    [Table("Option")]
    public partial class Option
    {
        // [Index(IsUnique = true)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int option_Id { get; set; }

        public virtual string Name { get; set; }
        public virtual string Value { get; set; }
        //public virtual DateTime Date { get; set; }
    }
}
