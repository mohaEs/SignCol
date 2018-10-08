using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel;

namespace DbModel.Entities
{
    [Table("MemberShip")]
    //[PropertyChanged.ImplementPropertyChanged]
    public class MemberShip //: INotifyPropertyChanged// PropertyValidateModel
    {
        //[Required(ErrorMessage = "{0} is required.")]
        [Index(IsUnique = true)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid mid { get; set; }
        [Required(ErrorMessage = "{0} is required.")]
        public int mdate { get; set; }
        [Required(ErrorMessage = "{0} is required.")]
        public string user { get; set; }
        [Required(ErrorMessage = "{0} is required.")]
        public string pass { get; set; }
        [Required(ErrorMessage = "{0} is required.")]
        public string vdate { get; set; }
        
    }
}
