using DbModel.Context;
using DbModel.DomainClasses.Entities;
using DbModel.DomainClasses.Enum;
using DbModel.Extensions;
using DbModel.Services.Interfaces;
using DbModel.ViewModel;
using DbModel.ViewModel.OptionVM;
using DbModel.ViewModel.WordsVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbModel.Services
{
    public class listcustomechart
    {
        public string wordname { get; set; }
        public int count { get; set; }
    }
    public class WordsService : IWords
    {
        IUnitOfWork _uow;
        IDbSet<Words> _word;
        IDbSet<Languages> _language;
        IDbSet<User> _user;
        IDbSet<Video> _video;
        public WordsService(IUnitOfWork uow)
        {
            _uow = uow;
            _word = _uow.Set<Words>();
            _language = _uow.Set<Languages>();
            _user = _uow.Set<User>();
            _video = _uow.Set<Video>();
        }
        public int WordCount(WordType wt)
        {
            return (from n in _word where n.WordType == wt select n).Count();

            //return _word.Count(x => x.WordType == wt);
        }
        public int VideoCount(WordType wt, LeapKinnectType lk)
        {
            return (from v in _video
                    join w in _word on v.Words.word_id equals w.word_id
                    where v.Words.WordType == wt && v.LeapKinnectType == lk select v).Count();

            //return _word.Count(x => x.WordType == wt);
        }
        public int VideoCountForList(int wordid)
        {
            return (from v in _video
                    join w in _word on v.Words.word_id equals w.word_id
                    where v.Words.word_id == wordid select v).Count();

            //return _word.Count(x => x.WordType == wt);
        }
        public List<listcustomechart> PerVideoCount(WordType wt, LeapKinnectType lk)
        {
            return _video.Include(x => x.Words)//.Include(x => x.User)
                                .Where(x => x.Words.WordType == wt && x.LeapKinnectType == lk)
                                .GroupBy(x => x.Words.Name)
                                .Select(c => new listcustomechart
                                {
                                    wordname = c.Key,
                                    count = c.Count()//g.Select(m => m.Name).Count()
                                }).ToList();
            /*return _word.Include(x => x.Languages)//.Include(x => x.User)
                                .Where(x => x.WordType == wt)
                                .GroupBy(x => x.Name)
                                .Select(c => new listcustomechart
                                {
                                    wordname = c.Key,
                                    count = c.Count()//g.Select(m => m.Name).Count()
                                }).ToList();*/
            //var results = (from v in _video
            //              join w in _word on v.Words.word_id equals w.word_id
            //              where v.Words.WordType == wt
            //              group w by w.Name into g
            //              select new listcustomechart
            //              {
            //                  wordname = g.Key,
            //                  count = g.Select(m => m.Name).Count()
            //              }).ToList();

            //return results;
            //return (from v in _video join w in _word on v.Words.word_id equals w.word_id where v.Words.WordType == wt select v).Count();

            //return _word.Count(x => x.WordType == wt);
        }
        public bool CheckIsVideoInWord(int wordid)
        {
            return _video.Any(x => x.word_id == wordid);//.Find(id);
        }
        public Words GetWordsEntityById(int id)
        {
            /*var word = (from c in _word
                        join p in _language
                        on c.lang_id equals p.lang_id
                        where c.word_id == id
                        select new Words
                        {
                            Name = c.Name,
                            WordType = c.WordType,
                            word_id = c.word_id,
                            lang_id = c.lang_id.Value,
                            Languages = c.Languages
                        }).FirstOrDefault();
            return word;*/
            return _word.Where(i => i.word_id == id).SingleOrDefault();
        }
        public WordsModel GetWordsById(int id)
        {
            var word = (from c in _word
                        join p in _language
                        on c.lang_id equals p.lang_id
                        //join u in _user
                        //on c.User_id equals u.User_id
                        where c.word_id == id
                        select new WordsModel
                        {
                            Name = c.Name,
                            WordType = c.WordType,
                            word_id = c.word_id,
                            lang_id = c.lang_id.Value,
                            Languages = c.Languages,
                            //KinnectFilePath = c.KinnectFilePath,
                            //LeapFilePath = c.LeapFilePath
                            //User_id = c.User_id.Value,
                            //User = c.User
                        }).FirstOrDefault();
            return word;
            //return _word.Where(i => i.word_id == id).SingleOrDefault();
        }
        public WordsModel GetWordsByIdAndUser(int id)
        {
            /*var word = (from c in _word
                        join p in _language
                        on c.lang_id equals p.lang_id
                        join u in _user
                        on c.User_id equals u.User_id
                        where c.word_id == id
                        select new WordsModel
                        {
                            Name = c.Name,
                            WordType = c.WordType,
                            word_id = c.word_id,
                            lang_id = c.lang_id.Value,
                            Languages = c.Languages,
                            FilePath = c.FilePath,
                            User_id = c.User_id.Value,
                            User = c.User
                        }).FirstOrDefault();
            return word;*/
            return _word.Include(x => x.Languages)//.Include(x => x.User)
                .Where(x => x.word_id == id)
                .Select(c => new WordsModel
                {
                    Name = c.Name,
                    WordType = c.WordType,
                    word_id = c.word_id,
                    lang_id = (c.lang_id.HasValue ? c.lang_id.Value : 0),
                    Languages = c.Languages,
                    //KinnectFilePath = c.KinnectFilePath,
                    //LeapFilePath = c.LeapFilePath,
                    //User_id = (c.User_id.HasValue ? c.User_id.Value : 0),
                    //User = c.User
                }).FirstOrDefault();
            //return _word.Where(i => i.word_id == id).SingleOrDefault();
        }
        public ObservableCollection<WordsModel> GetAll()
        {
            return new ObservableCollection<WordsModel>(from c in _word
                                                        join p in _language
                                                        on c.lang_id equals p.lang_id
                                                        //join u in _user
                                                        //on c.User_id equals u.User_id
                                                        select new WordsModel
                                                        {
                                                            Name = c.Name,
                                                            WordType = c.WordType,
                                                            word_id = c.word_id,
                                                            lang_id = c.lang_id.Value,
                                                            Languages = c.Languages,
                                                            //KinnectFilePath = c.KinnectFilePath,
                                                            //LeapFilePath = c.LeapFilePath
                                                            //User_id = c.User_id.Value,
                                                            //User = c.User
                                                        });
            //return _word.ToList().ToObservableCollection();
        }
        public ObservableCollection<Words> Search(int occupation, int maritalStatus, string operand)
        {
            return _word.ToList().ToObservableCollection();
        }
        public bool Create(WordsModel Words)
        {
            var wo = new Words
            {
                Name = Words.Name,
                WordType = Words.WordType,
                Languages = Words.Languages,
                lang_id = Words.lang_id,
                //KinnectFilePath = Words.KinnectFilePath,
                //LeapFilePath = Words.LeapFilePath
                //User = Words.User,
                //User_id = Words.User_id,
            };
            _word.Add(wo);
            bool b = _uow.SaveChanges() > 0;
            _uow.Entry(wo).State = EntityState.Detached;

            return b;
        }
        public bool Update(WordsModel Words)
        {
            bool b = false;
            //var entity = new Words { word_id = Words.word_id };
            var entity = _word.AsNoTracking().FirstOrDefault(x => x.word_id == Words.word_id);
            entity.Name = Words.Name;
            entity.WordType = Words.WordType;
            //entity.Languages = Words.Languages;
            entity.lang_id = Words.lang_id;
            //entity.KinnectFilePath = Words.KinnectFilePath;
            //entity.LeapFilePath = Words.LeapFilePath;
            //entity.User = Words.User;
            //entity.User_id = Words.User_id;

            _word.Attach(entity);
            _uow.Entry(entity).State = EntityState.Modified;
            b = _uow.SaveChanges() > 0;
            _uow.Entry(entity).State = EntityState.Detached;

            return b;
        }
       
        public bool Delete(int id)
        {
            var entity = new Words { word_id = id };
            _uow.Entry(entity).State = EntityState.Deleted;
            return _uow.SaveChanges() > 0;
        }
        public bool DeleteByLanguage(int langid, AppConfig app)
        {
            bool b = false;
            //var entity = new Words { lang_id = langid };
            var entity = _word.Where(x => x.lang_id == langid).FirstOrDefault();
            if (entity != null)
            {
                _uow.Entry(entity).State = EntityState.Deleted;
                b = _uow.SaveChanges() > 0;
            }
            return b;
        }

        public ObservableCollection<WordsModel> GetWords(int start, int
            itemCount, string sortColumn, bool ascending, out int totalItems,
            string SearchWordNme/*, LeapKinnectType lk*/)
        {
            //using (var context = new MyDbContext())
            //{
            List<WordsModel> allpats = new List<WordsModel>();
            allpats = _word.Include(x => x.Languages)
                            .Include(x => x.Video)
                            .Select(c => new WordsModel
                            {
                                Name = c.Name,
                                WordType = c.WordType,
                                word_id = c.word_id,
                                lang_id = c.lang_id.Value,
                                Languages = c.Languages,

                                videoCount = (c.Video.Where(x => x.word_id == c.word_id).Count())
                            }).ToList();

            if (!string.IsNullOrEmpty(SearchWordNme))
            {
                allpats = _word.Include(x => x.Languages)
                                .Include(x => x.Video)
                                   .Where(x => x.Name.Contains(SearchWordNme))
                                .Select(c => new WordsModel
                                {
                                    Name = c.Name,
                                    WordType = c.WordType,
                                    word_id = c.word_id,
                                    lang_id = c.lang_id.Value,
                                    Languages = c.Languages,

                                    videoCount = (c.Video.Where(x => x.word_id == c.word_id).Count())
                                }).ToList();
            }
            totalItems = allpats.Count;
            ObservableCollection<WordsModel> sortedProducts = new
                    ObservableCollection<WordsModel>();
            
            // Sort the products. In reality, the items should be stored in a  database and
            // use SQL statements for sorting and querying items.
            switch (sortColumn)
            {
                case ("word_id"):
                    sortedProducts = new ObservableCollection<WordsModel>
                    (
                        from p in allpats
                        orderby p.word_id
                        select p
                    );
                    break;
            }
            sortedProducts = ascending ? sortedProducts : new
            ObservableCollection<WordsModel>(sortedProducts.Reverse());
            ObservableCollection<WordsModel> filteredProducts = new
                ObservableCollection<WordsModel>();
            for (int i = start; i < start + itemCount && i < totalItems; i++)
            {
                filteredProducts.Add(sortedProducts[i]);
            }
            return filteredProducts;
            //}
        }

        public ObservableCollection<WordsModel> GetWords(int start, int
            itemCount, string sortColumn, bool ascending, out int totalItems,
            WordType? wt, string SearchWordNme/*, LeapKinnectType lk*/)
        {
            //using (var context = new MyDbContext())
            //{
            List<WordsModel> allpats = new List<WordsModel>();
             allpats = _word.Include(x => x.Languages)
                                .Include(x => x.Video)
                                .Where(x => x.WordType == wt.Value)
                                .Select(c => new WordsModel
                                {
                                    Name = c.Name,
                                    WordType = c.WordType,
                                    word_id = c.word_id,
                                    lang_id = c.lang_id.Value,
                                    Languages = c.Languages,

                                    videoCount = (c.Video.Where(x => x.word_id == c.word_id).Count())
                                }).ToList();
            if (!string.IsNullOrEmpty(SearchWordNme))
            {
                allpats = _word.Include(x => x.Languages)
                                   .Include(x => x.Video)
                                   .Where(x => x.WordType == wt.Value && x.Name.Contains(SearchWordNme))
                                   .Select(c => new WordsModel
                                   {
                                       Name = c.Name,
                                       WordType = c.WordType,
                                       word_id = c.word_id,
                                       lang_id = c.lang_id.Value,
                                       Languages = c.Languages,

                                       videoCount = (c.Video.Where(x => x.word_id == c.word_id).Count())
                                   }).ToList();
            }
            totalItems = allpats.Count;
            ObservableCollection<WordsModel> sortedProducts = new
                    ObservableCollection<WordsModel>();

            // Sort the products. In reality, the items should be stored in a  database and
            // use SQL statements for sorting and querying items.
            switch (sortColumn)
            {
                case ("word_id"):
                    sortedProducts = new ObservableCollection<WordsModel>
                    (
                        from p in allpats
                        orderby p.word_id
                        select p
                    );
                    break;
            }
            sortedProducts = ascending ? sortedProducts : new
            ObservableCollection<WordsModel>(sortedProducts.Reverse());
            ObservableCollection<WordsModel> filteredProducts = new
                ObservableCollection<WordsModel>();
            for (int i = start; i < start + itemCount && i < totalItems; i++)
            {
                filteredProducts.Add(sortedProducts[i]);
            }
            return filteredProducts;
            //}
        }
    }
}
