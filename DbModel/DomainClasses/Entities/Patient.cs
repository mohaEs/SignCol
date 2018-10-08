
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DbModel.Entities
{
    [Table("Patient")]
    //[PropertyChanged.ImplementPropertyChanged]
    public class Patient //: PropertyValidateModel
    {
        //[Required(ErrorMessage = "{0} is required.")]
        [Index(IsUnique = true)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int pid { get; set; }

        //[Required(ErrorMessage = "{0} is required.")]
        public string name { get; set; }

        //[Required(ErrorMessage = "{0} is required.")]
        public int pdate { get; set; }

        //[Required(ErrorMessage = "{0} is required.")]
        //[Range(0, 1, ErrorMessage = "{0} is required.")]
        public byte? gender { get; set; }

        //[Required(ErrorMessage = "{0} is required.")]
        //[Range(0, 1, ErrorMessage = "{0} is required.")]
        public byte? family_history { get; set; }

        //[Required(ErrorMessage = "{0} is required.")]
        //[Range(1, 200, ErrorMessage = "{0} is required.")]
        public int? age_d { get; set; }

        //[Required(ErrorMessage = "{0} is required.")]
        //[Range(0, 1, ErrorMessage = "{0} is required.")]
        public byte? cansanguinity { get; set; }

        public virtual ICollection<First_Exam> FirstExams { get; set; }
        public virtual ICollection<Last_Exam> LastExams { get; set; }

    }
}
