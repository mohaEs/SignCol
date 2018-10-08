using DbModel.DomainClasses.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbModel.DomainClasses.Entities
{
    [Table("Words")]
    public partial class Words
    {
       // [Index(IsUnique = true)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int word_id { get; set; }

        //[Required(ErrorMessage = "{0} is required.")]
        public virtual string Name { get; set; }

        //public virtual string KinnectFilePath { get; set; }

        public virtual WordType WordType { get; set; }

        public virtual Languages Languages { get; set; }
        public virtual int? lang_id { get; set; }

        public virtual ICollection<Video> Video { get; set; }
        //public virtual int? Video_id { get; set; }

        //public virtual User User { get; set; }
        //public virtual int? User_id { get; set; }
    }
}
