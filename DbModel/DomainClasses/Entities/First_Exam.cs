using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DbModel.Entities
{
    [Table("First_Exam")]
    public class First_Exam //: PropertyValidateModel
    {
        //[Required(ErrorMessage = "{0} is required.")]
        [Index(IsUnique = true)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int feid { get; set; }

        //[Required(ErrorMessage = "{0} is required.")]
        public int exam_date { get; set; }

        //[Required(ErrorMessage = "{0} is required.")]
        public string baseline { get; set; }

        //[Required(ErrorMessage = "{0} is required.")]
        public string medications { get; set; }

        //[Required(ErrorMessage = "{0} is required.")]
        public string diagnosis { get; set; }

        //[Required(ErrorMessage = "{0} is required.")]
        public string surgeries { get; set; }

        //[Required(ErrorMessage = "{0} is required.")]
        public string iris { get; set; }

        //[Required(ErrorMessage = "{0} is required.")]
        public string bleb { get; set; }

        //[Required(ErrorMessage = "{0} is required.")]
        public string tube { get; set; }

        //[Required(ErrorMessage = "{0} is required.")]
        public string lens { get; set; }

        //[Required(ErrorMessage = "{0} is required.")]
        public string corneal_clarity { get; set; }

        //[Required(ErrorMessage = "{0} is required.")]
        public string corneal_diameter { get; set; }

        //[Required(ErrorMessage = "{0} is required.")]
        public string ac { get; set; }

        //[Required(ErrorMessage = "{0} is required.")]
        public string cct { get; set; }

        //[Required(ErrorMessage = "{0} is required.")]
        public string fundus { get; set; }

        //[Required(ErrorMessage = "{0} is required.")]
        public string cd { get; set; }

        //[Required(ErrorMessage = "{0} is required.")]
        public string al { get; set; }

        //[Required(ErrorMessage = "{0} is required.")]
        public string reflection { get; set; }

        //[Required(AllowEmptyStrings = true)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string plan { get; set; }


        public int patient_id { get; set; }


        //public virtual Patient patient { get; set; }

        public virtual ICollection<Result> results { get; set; }

        public virtual ICollection<Documents> docs { get; set; }

    }
}

