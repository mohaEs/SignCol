using DbModel.DomainClasses.Entities;
using DbModel.ViewModel.UserVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbModel.Services.Interfaces
{
    public interface IUser
    {
        UserModel GetUserById(int Id);
        User GetUserEntityById(int id);
        bool CheckUserHaveVideo(int userid);

        ObservableCollection<UserModel> GetAll();

        ObservableCollection<User> Search(int occupation, int maritalStatus, string operand);

        bool Create(UserModel User);

        bool Update(UserModel User);

        bool Delete(int id);

        ObservableCollection<UserModel> GetUsers(int start, int
            itemCount, string sortColumn, bool ascending, out int totalItems);
    }
}
