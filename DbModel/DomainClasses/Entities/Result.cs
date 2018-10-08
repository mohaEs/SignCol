using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DbModel.Entities
{
    [Table("Result")]
    public class Result
    {
        //[Required(ErrorMessage = "{0} is required.")]
        [Index(IsUnique = true)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int rid { get; set; }

        public byte? sur { get; set; }

        public string value1 { get; set; }

        public string value2 { get; set; }

        public string value3 { get; set; }

        public string value4 { get; set; }


        public int? First_Exam_id { get; set; }
        public int? Last_Exam_id { get; set; }


        //public virtual First_Exam FirstExam { get; set; }
        //public virtual Last_Exam LastExam { get; set; }


    }
}

