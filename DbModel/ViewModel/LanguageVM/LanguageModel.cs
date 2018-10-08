using DbModel.DomainClasses.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbModel.ViewModel.LanguageVM
{
    public class LanguageModel
    {
        public int lang_id { get; set; }
        
        public string Name { get; set; }

        public int WordsCount { get; set; }
        //public int? word_id { get; set; }
    }
}
