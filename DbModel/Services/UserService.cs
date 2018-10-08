using DbModel.Context;
using DbModel.DomainClasses.Entities;
using DbModel.Extensions;
using DbModel.Services.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using DbModel.ViewModel.UserVM;

namespace DbModel.Services
{
    public class UserService : IUser
    {
        IUnitOfWork _uow;
        IDbSet<User> _user;
        IDbSet<Words> _word;
        IDbSet<Video> _video;
        public UserService(IUnitOfWork uow)
        {
            _uow = uow;
            _user = _uow.Set<User>();
            _word = _uow.Set<Words>();
            _video = _uow.Set<Video>();
        }

        public User GetUserEntityById(int id)
        {
            return _user.Find(id);
        }
        public bool CheckUserHaveVideo(int userid)
        {
            return _video.Any(x => x.User_id == userid);//.Find(id);
        }
        public UserModel GetUserById(int id)
        {
            var user = (from c in _user
                        where c.User_id == id
                        select new UserModel
                        {
                            Name = c.Name,
                            //Age = (c.Age.HasValue ? c.Age.Value : 0),
                            Age = c.Age,
                            Phone = c.Phone,
                            User_id = c.User_id                            
                        }).FirstOrDefault();
            return user;
            //return _user.Where(i => i.User_id == id).SingleOrDefault();
        }
        public ObservableCollection<UserModel> GetAll()
        {
            return new ObservableCollection<UserModel>(from c in _user
                                                       select new UserModel
                                                       {
                                                           Name = c.Name,
                                                           //Age = (c.Age.HasValue ? c.Age.Value : 0),
                                                           Age = c.Age,
                                                           Phone = c.Phone,
                                                           User_id = c.User_id
                                                       });
            //return _user.ToList().ToObservableCollection();
        }
        public ObservableCollection<User> Search(int occupation, int maritalStatus, string operand)
        {
            return _user.ToList().ToObservableCollection();
        }
        public bool Create(UserModel User)
        {
            var us = new User
            {
                Name = User.Name,
                Age = User.Age,
                Phone = User.Phone                
            };
            _user.Add(us);
            bool b = _uow.SaveChanges() > 0;
            _uow.Entry(us).State = EntityState.Detached;

            return b;
        }
        public bool Update(UserModel User)
        {
            bool b = false;
            var entity = new User { User_id = User.User_id };
            entity.Name = User.Name;
            entity.Phone = User.Phone;
            entity.Age = User.Age;

            _user.Attach(entity);
            _uow.Entry(entity).State = EntityState.Modified;
            b = _uow.SaveChanges() > 0;
            //_uow.Entry(entity).State = EntityState.Detached;

            return b;
        }
        public bool Delete(int id)
        {
            bool b = false;
            //var entity = new User { User_id = id };
            var entity = _user.AsNoTracking().Where(x => x.User_id == id).FirstOrDefault();
            if (entity != null)
            {
                _uow.Entry(entity).State = EntityState.Detached;
            }
            _uow.Entry(entity).State = EntityState.Deleted;
            b = _uow.SaveAllChanges() > 0;
            return b;
        }

        public ObservableCollection<UserModel> GetUsers(int start, int
            itemCount, string sortColumn, bool ascending, out int totalItems)
        {
            IList<UserModel> allpats = (from c in _user
                                       select new UserModel
                                       {
                                           User_id = c.User_id
                                           ,Name = c.Name
                            //Age = (c.Age.HasValue ? c.Age.Value : 0),
                            ,Age = c.Age
                                           ,
                                           Phone = c.Phone
                                       }).ToList();
               
                totalItems = allpats.Count;
                ObservableCollection<UserModel> sortedProducts = new
                        ObservableCollection<UserModel>();
                

                switch (sortColumn)
                {
                    case ("User_id"):
                        sortedProducts = new ObservableCollection<UserModel>
                        (
                            from p in allpats
                            orderby p.User_id
                            select p
                        );
                        break;
                }
                sortedProducts = ascending ? sortedProducts : new
                ObservableCollection<UserModel>(sortedProducts.Reverse());
                ObservableCollection<UserModel> filteredProducts = new
                    ObservableCollection<UserModel>();
                for (int i = start; i < start + itemCount && i < totalItems; i++)
                {
                    filteredProducts.Add(sortedProducts[i]);
                }
                return filteredProducts;
            //}
        }

    }
}