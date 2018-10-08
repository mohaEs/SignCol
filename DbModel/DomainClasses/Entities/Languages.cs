using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbModel.DomainClasses.Entities
{
    [Table("Languages")]
    public partial class Languages
    {
        // [Index(IsUnique = true)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int lang_id { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        public virtual string Name { get; set; }

        public virtual ICollection<Words> Words { get; set; }
        //public int? word_id { get; set; }

    }
}
