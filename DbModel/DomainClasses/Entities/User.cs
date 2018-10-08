using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbModel.DomainClasses.Entities
{
    [Table("User")]
    public partial class User
    {
        // [Index(IsUnique = true)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int User_id { get; set; }

        //[Required(ErrorMessage = "{0} is required.")]
        public virtual string Name { get; set; }

        public virtual string Age { get; set; }

        public virtual string Phone { get; set; }

        //public virtual ICollection<Words> Words { get; set; }

        public virtual ICollection<Video> Videos { get; set; }
        //public int? word_id { get; set; }
    }
}
