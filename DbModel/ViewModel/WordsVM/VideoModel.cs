using DbModel.DomainClasses.Entities;
using DbModel.DomainClasses.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbModel.ViewModel.WordsVM
{
    public class VideoModel
    {
        public int video_id { get; set; }

        public string KinnectFilePath { get; set; }

        public string LeapFilePath { get; set; }

        public LeapKinnectType LeapKinnectType { get; set; }

        public User User { get; set; }
        public int? User_id { get; set; }

        public Words Words { get; set; }
        public int? word_id { get; set; }
    }
}
