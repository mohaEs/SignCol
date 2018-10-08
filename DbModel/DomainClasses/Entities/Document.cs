using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System;

namespace DbModel.Entities
{
    [Table("Documents")]
    public partial class Documents
    {
        [Index(IsUnique = true)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid doc_id { get; set; }

        //[Required(ErrorMessage = "{0} is required.")]
        public string FName { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        public int Fdate { get; set; }
        //public string FPath { get; set; }

        //[Required(ErrorMessage = "{0} is required.")]
        //public int? FSize { get; set; }
        //public byte? Ftype { get; set; }

        //public byte[] Fcontent { get; set; }


        public int? First_Exam_id { get; set; }
        public int? Last_Exam_id { get; set; }


        //public virtual First_Exam fexam { get; set; }
        //public virtual Last_Exam lexam { get; set; }
    }
}
