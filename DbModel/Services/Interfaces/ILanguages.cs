using DbModel.DomainClasses.Entities;
using DbModel.ViewModel.LanguageVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbModel.Services.Interfaces
{
    public interface ILanguages
    {
        bool CheckIsLanguageInWord(int langid);
        LanguageModel GetLanguagesById(int id);

        ObservableCollection<LanguageModel> GetAll();

        ObservableCollection<Languages> Search(int occupation, int maritalStatus, string operand);

        bool Create(LanguageModel Languages);

        bool Update(LanguageModel Languages);

        bool Delete(int id);

        ObservableCollection<LanguageModel> GetLanguages(int start, int
            itemCount, string sortColumn, bool ascending, out int totalItems);
    }
}
