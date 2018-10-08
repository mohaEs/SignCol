using DbModel.Context;
using DbModel.DomainClasses.Entities;
using DbModel.Extensions;
using DbModel.Services.Interfaces;
using DbModel.ViewModel.LanguageVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbModel.Services
{
    public class LanguageServise : ILanguages
    {

        IUnitOfWork _uow;
        IDbSet<Languages> _language;
        IDbSet<Words> _word;
        IDbSet<Video> _video;
        public LanguageServise(IUnitOfWork uow)
        {
            _uow = uow;
            _language = _uow.Set<Languages>();
            _word = _uow.Set<Words>();
            _video = _uow.Set<Video>();
        }

        public LanguageModel GetLanguagesById(int id)
        {
            var lang = (from c in _language
                        //join p in _word 
                        //on c.word_id equals p.word_id
                        where c.lang_id == id
                        select new LanguageModel
                        {
                            Name = c.Name,
                            lang_id = c.lang_id,
                            WordsCount = 0//c.Words.Count
                        }).FirstOrDefault();
            return lang;
            //return _language.Where(i => i.lang_id == id).SingleOrDefault();
        }
        public ObservableCollection<LanguageModel> GetAll()
        {
            return new ObservableCollection<LanguageModel>(from c in _language
                     //join p in _word on c.Words.word_id equals p.word_id
                     select new LanguageModel
                     {
                         Name = c.Name,
                         lang_id = c.lang_id,
                         WordsCount = 0//c.Words.Count
                     });
            //return _language.ToList().ToObservableCollection();
        }
        public ObservableCollection<Languages> Search(int occupation, int maritalStatus, string operand)
        {
            return _language.ToList().ToObservableCollection();
        }
        public bool Create(LanguageModel Languages)
        {
            var la = new Languages
            {
                Name = Languages.Name
            };
            _language.Add(la);
            bool b = _uow.SaveChanges() > 0;
            _uow.Entry(la).State = EntityState.Detached;

            return b;
        }
        public bool Update(LanguageModel Languages)
        {
            bool b = false;
            var entity = new Languages { lang_id = Languages.lang_id };
            entity.Name = Languages.Name;

            _language.Attach(entity);
            _uow.Entry(entity).State = EntityState.Modified;
            b = _uow.SaveChanges() > 0;
            _uow.Entry(entity).State = EntityState.Detached;

            return b;
        }
        public bool CheckIsLanguageInWord(int langid)
        {
            return _word.Any(x => x.lang_id == langid);//.Find(id);
        }
        public bool Delete(int id)
        {
            bool b = false;
            //var entity = new Languages { lang_id = id };
            var entity = _language.Where(x => x.lang_id == id).FirstOrDefault();
            //var wordentity = _word.Where(x => x.lang_id == id).ToList();
            //if (wordentity != null)
            //{
            //    foreach (var item in wordentity)
            //    {
            //        var videoentity = _video.Where(x => x.word_id == item.word_id).ToList();
            //        foreach(var videoitem in videoentity)
            //        {
            //            _uow.Entry(videoitem).State = EntityState.Deleted;

            //            b = _uow.SaveChanges() > 0;
            //            _uow.Entry(videoitem).State = EntityState.Detached;
            //        }


            //        _uow.Entry(item).State = EntityState.Deleted;

            //        b = _uow.SaveChanges() > 0;
            //        _uow.Entry(item).State = EntityState.Detached;

            //    }
            //}
            _uow.Entry(entity).State = EntityState.Deleted;
            return _uow.SaveChanges() > 0;
        }

        public ObservableCollection<LanguageModel> GetLanguages(int start, int
            itemCount, string sortColumn, bool ascending, out int totalItems)
        {
            IList<LanguageModel> allpats = (from c in _language
                                            //join p in _word on c.Words.word_id equals p.word_id
                                            select new LanguageModel
                                            {
                                                lang_id = c.lang_id,
                                                Name = c.Name
                                                //,WordsCount = 0//c.Words.Count
                                            }).ToList();
            totalItems = allpats.Count;
            ObservableCollection<LanguageModel> sortedProducts = new
                    ObservableCollection<LanguageModel>();
            

            switch (sortColumn)
            {
                case ("lang_id"):
                    sortedProducts = new ObservableCollection<LanguageModel>
                    (
                        from p in allpats
                        orderby p.lang_id
                        select p
                    );
                    break;
            }
            sortedProducts = ascending ? sortedProducts : new
            ObservableCollection<LanguageModel>(sortedProducts.Reverse());
            ObservableCollection<LanguageModel> filteredProducts = new
                ObservableCollection<LanguageModel>();
            for (int i = start; i < start + itemCount && i < totalItems; i++)
            {
                filteredProducts.Add(sortedProducts[i]);
            }
            return filteredProducts;
           
        }

    }
}