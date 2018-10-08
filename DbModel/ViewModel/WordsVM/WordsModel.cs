using DbModel.DomainClasses.Entities;
using DbModel.DomainClasses.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbModel.ViewModel.WordsVM
{
    public class WordsModel
    {
        public int word_id { get; set; }

        //[Required(ErrorMessage = "{0} is required.")]
        public string Name { get; set; }

        //public string KinnectFilePath { get; set; }

        //public string LeapFilePath { get; set; }

        public WordType WordType { get; set; }

        public Languages Languages { get; set; }
        public int lang_id { get; set; }

        public int? videoCount { get; set; }

        public ICollection<Video> Videos { get; set; }
        //public int Video_id { get; set; }
    }
}
