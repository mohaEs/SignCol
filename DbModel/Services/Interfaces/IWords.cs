using DbModel.DomainClasses.Entities;
using DbModel.DomainClasses.Enum;
using DbModel.ViewModel;
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
    public interface IWords
    {
        WordsModel GetWordsById(int Id);
        Words GetWordsEntityById(int id);
        //WordsModel GetWordsByIdAndUser(int id);
        bool CheckIsVideoInWord(int wordid);
        int WordCount(WordType wt);

        int VideoCount(WordType wt, LeapKinnectType lk);

        List<listcustomechart> PerVideoCount(WordType wt, LeapKinnectType lk);

        ObservableCollection<WordsModel> GetAll();

        ObservableCollection<Words> Search(int occupation, int maritalStatus, string operand);

        bool Create(WordsModel Words);

        bool Update(WordsModel Words);
        //bool UpdateVideo(WordsModel Words);

        bool Delete(int id);
        //bool DeleteKinnectVideo(WordsModel Words, AppConfig app);
        //bool DeleteLeapVideo(WordsModel Words, AppConfig app);
        //bool DeleteByUser(int userid, AppConfig app);
        bool DeleteByLanguage(int langid, AppConfig app);

        ObservableCollection<WordsModel> GetWords(int start, int
            itemCount, string sortColumn, bool ascending, out int totalItems, string SearchWordNme);
        ObservableCollection<WordsModel> GetWords(int start, int
            itemCount, string sortColumn, bool ascending, out int totalItems, WordType? wt, string SearchWordNme);
    }
}
