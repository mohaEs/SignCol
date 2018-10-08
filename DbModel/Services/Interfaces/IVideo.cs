using DbModel.DomainClasses.Entities;
using DbModel.DomainClasses.Enum;
using DbModel.ViewModel.OptionVM;
using DbModel.ViewModel.WordsVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbModel.Services.Interfaces
{
    public interface IVideo
    {
        VideoModel GetVideosPrevById(int videoid);
        VideoModel GetVideoById(int videoid);
        VideoModel GetVideoByWord(int wordid, int ty);


        ObservableCollection<VideoModel> GetAll();

        ObservableCollection<Video> Search(int occupation, int maritalStatus, string operand);

        bool Create(VideoModel Words);

        bool Update(VideoModel Words);
        bool UpdateVideo(VideoModel Words);

        bool Delete(int id);
        bool DeleteKinnectVideo(VideoModel Words);
        bool DeleteLeapVideo(VideoModel Words);
        bool DeleteByUser(int userid);
        bool DeleteByLanguage(int langid);

        ObservableCollection<VideoModel> GetWords(int start, int
            itemCount, string sortColumn, bool ascending, out int totalItems, int? wordid, LeapKinnectType lk);

        ObservableCollection<VideoModel> GetAllUserVideos(int user_id);
    }
}
