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
    [Table("Video")]
    public partial class Video
    {
        // [Index(IsUnique = true)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int video_id { get; set; }

        public virtual string KinnectFilePath { get; set; }

        public virtual string LeapFilePath { get; set; }

        public virtual LeapKinnectType LeapKinnectType { get; set; }

        public virtual User User { get; set; }
        public virtual int? User_id { get; set; }

        public virtual Words Words { get; set; }
        public virtual int? word_id { get; set; }
    }
}
