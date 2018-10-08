using DbModel.DomainClasses.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbModel.ViewModel.UserVM
{
    public class UserModel
    {
        public int User_id { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        public string Age { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        public string Phone { get; set; }

        public ICollection<Video> Videos { get; set; }
        //public int? word_id { get; set; }
    }
}
