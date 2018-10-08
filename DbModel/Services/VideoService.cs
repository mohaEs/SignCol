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
    public class VideoService : IVideo
    {

        IUnitOfWork _uow;
        IDbSet<Words> _word;
        IDbSet<Video> _video;
        IDbSet<Languages> _language;
        IDbSet<User> _user;
        IOptionService option;
        AppConfig app;
        public VideoService(IUnitOfWork uow)
        {
            _uow = uow;
            _word = _uow.Set<Words>();
            _video = _uow.Set<Video>();
            _language = _uow.Set<Languages>();
            _user = _uow.Set<User>();
            option = new OptionService(uow);
            app = option.GetAll();
        }
        
        public VideoModel GetVideosPrevById(int videoid)
        {
            var word = (from c in _video
                        join u in _user
                        on c.User_id equals u.User_id
                        join w in _word
                        on c.word_id equals w.word_id
                        where c.video_id == videoid
                        select new VideoModel
                        {
                            video_id = c.video_id,
                            KinnectFilePath = c.KinnectFilePath,
                            LeapFilePath = c.LeapFilePath,
                            User_id = c.User_id.Value,
                            User = c.User,
                            Words = c.Words,
                            word_id = c.word_id.Value,
                            LeapKinnectType = c.LeapKinnectType
                        }).FirstOrDefault();
            return word;
            //return _word.Where(i => i.word_id == id).SingleOrDefault();
        }
        public VideoModel GetVideoById(int videoid)
        {
            return _video.Include(x => x.User)
                .Include(x => x.Words)
                .Where(x => x.video_id == videoid)
                .Select(c => new VideoModel
                {
                    video_id = c.video_id,
                    KinnectFilePath = c.KinnectFilePath,
                    LeapFilePath = c.LeapFilePath,
                    User_id = (c.User_id.HasValue ? c.User_id.Value : 0),
                    User = c.User,
                    Words = c.Words,
                    word_id = (c.word_id.HasValue ? c.word_id.Value : 0),
                    LeapKinnectType = c.LeapKinnectType
                }).FirstOrDefault();
            //return _word.Where(i => i.word_id == id).SingleOrDefault();
        }
        public VideoModel GetVideoByWord(int wordid, int ty)
        {
            if (ty == 1)
            {
                return _video.Include(x => x.User)
                    .Include(x => x.Words)
                    .Where(x => x.word_id == wordid && !string.IsNullOrEmpty(x.KinnectFilePath))
                    .Select(c => new VideoModel
                    {
                        video_id = c.video_id,
                        KinnectFilePath = c.KinnectFilePath,
                        LeapFilePath = c.LeapFilePath,
                        User_id = (c.User_id.HasValue ? c.User_id.Value : 0),
                        User = c.User,
                        Words = c.Words,
                        word_id = (c.word_id.HasValue ? c.word_id.Value : 0),
                        LeapKinnectType = c.LeapKinnectType
                    }).FirstOrDefault();
            }
            else
                return _video.Include(x => x.User)
                    .Include(x => x.Words)
                    .Where(x => x.word_id == wordid && !string.IsNullOrEmpty(x.LeapFilePath))
                    .Select(c => new VideoModel
                    {
                        video_id = c.video_id,
                        KinnectFilePath = c.KinnectFilePath,
                        LeapFilePath = c.LeapFilePath,
                        User_id = (c.User_id.HasValue ? c.User_id.Value : 0),
                        User = c.User,
                        Words = c.Words,
                        word_id = (c.word_id.HasValue ? c.word_id.Value : 0),
                        LeapKinnectType = c.LeapKinnectType
                    }).FirstOrDefault();
            //return _word.Where(i => i.word_id == id).SingleOrDefault();
        }
        public ObservableCollection<VideoModel> GetAll()
        {
            return new ObservableCollection<VideoModel>(from c in _video
                                                        join u in _user
                                                        on c.User_id equals u.User_id
                                                        join w in _word
                                                        on c.word_id equals w.word_id
                                                        select new VideoModel
                                                        {
                                                            video_id = c.video_id,
                                                            KinnectFilePath = c.KinnectFilePath,
                                                            LeapFilePath = c.LeapFilePath,
                                                            User_id = c.User_id.Value,
                                                            User = c.User,
                                                            Words = c.Words,
                                                            word_id = c.word_id.Value,
                                                            LeapKinnectType = c.LeapKinnectType
                                                        });
            //return _word.ToList().ToObservableCollection();
        }
        public ObservableCollection<VideoModel> GetAllUserVideos(int user_id)
        {
            return new ObservableCollection<VideoModel>(from c in _video
                                                        join u in _user
                                                        on c.User_id equals u.User_id
                                                        join w in _word
                                                        on c.word_id equals w.word_id
                                                        where c.User_id != null && c.User_id == user_id
                                                        select new VideoModel
                                                        {
                                                            video_id = c.video_id,
                                                            KinnectFilePath = c.KinnectFilePath,
                                                            LeapFilePath = c.LeapFilePath,
                                                            User_id = c.User_id.Value,
                                                            User = c.User,
                                                            Words = c.Words,
                                                            word_id = c.word_id.Value,
                                                            LeapKinnectType = c.LeapKinnectType
                                                        });
            //return _word.ToList().ToObservableCollection();
        }
        public ObservableCollection<Video> Search(int occupation, int maritalStatus, string operand)
        {
            return _video.ToList().ToObservableCollection();
        }
        public bool Create(VideoModel Videos)
        {
            var wo = new Video
            {
                KinnectFilePath = Videos.KinnectFilePath,
                LeapFilePath = Videos.LeapFilePath,
                video_id = Videos.video_id,
                User = Videos.User,
                User_id = Videos.User_id,
                Words = Videos.Words,
                word_id = Videos.word_id,
                LeapKinnectType = Videos.LeapKinnectType
            };
            _video.Add(wo);
            bool b = _uow.SaveChanges() > 0;
            _uow.Entry(wo).State = EntityState.Detached;

            return b;
        }
        public bool Update(VideoModel Videos)
        {
            bool b = false;
            var entity = new Video { video_id = Videos.video_id };
          
            entity.KinnectFilePath = Videos.KinnectFilePath;
            entity.LeapFilePath = Videos.LeapFilePath;
            entity.User = Videos.User;
            entity.User_id = Videos.User_id;
            entity.Words = Videos.Words;
            entity.word_id = Videos.word_id;
            entity.LeapKinnectType = Videos.LeapKinnectType;

            _video.Attach(entity);
            _uow.Entry(entity).State = EntityState.Modified;
            b = _uow.SaveChanges() > 0;
            _uow.Entry(entity).State = EntityState.Detached;

            return b;
        }
        public bool UpdateVideo(VideoModel Videos)
        {
            bool b = false;
            var entity = new Video { video_id = Videos.video_id };
           
            entity.User = Videos.User;
            entity.User_id = Videos.User_id;
            entity.KinnectFilePath = Videos.KinnectFilePath;
            entity.LeapFilePath = Videos.LeapFilePath;
            entity.LeapKinnectType = Videos.LeapKinnectType;

            _video.Attach(entity);
            _uow.Entry(entity).State = EntityState.Modified;
            b = _uow.SaveChanges() > 0;
            _uow.Entry(entity).State = EntityState.Detached;

            return b;
        }
        public bool Delete(int id)
        {
            var entity = new Video { video_id = id };
            _uow.Entry(entity).State = EntityState.Deleted;
            return _uow.SaveChanges() > 0;
        }
        public bool DeleteKinnectVideo(VideoModel Videos)
        {
            bool b = false;
            var entity = new Video { video_id = Videos.video_id };
            entity.KinnectFilePath = string.Empty;
            entity.User = null;
            entity.User_id = null;
            _video.Attach(entity);
            _uow.Entry(entity).State = EntityState.Modified;
            b = _uow.SaveChanges() > 0;
            _uow.Entry(entity).State = EntityState.Detached;

            return b;
        }
        public bool DeleteLeapVideo(VideoModel Videos)
        {
            bool b = false;
            var entity = new Video { video_id = Videos.video_id };
            entity.KinnectFilePath = Videos.KinnectFilePath;
            entity.LeapFilePath = string.Empty;
            entity.User = null;
            entity.User_id = null;
            
            _video.Attach(entity);
            _uow.Entry(entity).State = EntityState.Modified;
            b = _uow.SaveChanges() > 0;
            _uow.Entry(entity).State = EntityState.Detached;

            return b;
        }
        public bool DeleteByUser(int userid)
        {
            bool b = false;
            //var entity = new Words { User_id = userid };
            var entity = _video.Where(x => x.User_id == userid).ToList();
            var useren = _user.Where(x => x.User_id == userid).FirstOrDefault();
            if (entity != null)
            {
                foreach (var item in entity)
                {
                    _uow.Entry(item).State = EntityState.Deleted;

                    b = _uow.SaveChanges() > 0;
                    _uow.Entry(item).State = EntityState.Detached;
                }
            }
            _uow.Entry(useren).State = EntityState.Deleted;
            b = _uow.SaveChanges() > 0;
            return b;
        }
        public bool DeleteByLanguage(int langid)
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

        public ObservableCollection<VideoModel> GetWords(int start, int
            itemCount, string sortColumn, bool ascending, out int totalItems, int? wordid, LeapKinnectType lk)
        {
            //using (var context = new MyDbContext())
            //{
            IList<VideoModel> allpats = ((wordid.HasValue)?_video.Include(x => x.User)
                                .Include(x => x.Words)
                                .Where(x=>x.word_id==wordid.Value && x.LeapKinnectType==lk)
                                .Select(c => new VideoModel
                                {
                                    video_id = c.video_id,
                                    KinnectFilePath = c.KinnectFilePath,
                                    LeapFilePath = c.LeapFilePath,
                                    User_id = c.User_id.Value,
                                    User = c.User,
                                    Words = c.Words,
                                    word_id = c.word_id
                                }).ToList():
                                _video.Include(x => x.User)
                                .Include(x => x.Words)
                                .Where(x => x.LeapKinnectType == lk)
                                .Select(c => new VideoModel
                                {
                                    video_id = c.video_id,
                                    KinnectFilePath = c.KinnectFilePath,
                                    LeapFilePath = c.LeapFilePath,
                                    User_id = c.User_id.Value,
                                    User = c.User,
                                    Words = c.Words,
                                    word_id = c.word_id
                                }).ToList());
            
            totalItems = allpats.Count;
            ObservableCollection<VideoModel> sortedProducts = new
                    ObservableCollection<VideoModel>();
            // Sort the products. In reality, the items should be stored in a  database and
            // use SQL statements for sorting and querying items.
            switch (sortColumn)
            {
                case ("video_id"):
                    sortedProducts = new ObservableCollection<VideoModel>
                    (
                        from p in allpats
                        orderby p.video_id
                        select p
                    );
                    break;
            }
            sortedProducts = ascending ? sortedProducts : new
            ObservableCollection<VideoModel>(sortedProducts.Reverse());
            ObservableCollection<VideoModel> filteredProducts = new
                ObservableCollection<VideoModel>();
            for (int i = start; i < start + itemCount && i < totalItems; i++)
            {
                filteredProducts.Add(sortedProducts[i]);
            }
            return filteredProducts;
            //}
        }

    }
}