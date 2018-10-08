using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DbModel.Context;
using DbModel.DomainClasses.Entities;
using DbModel.ViewModel.OptionVM;
using DbModel.Services.Interfaces;

namespace DbModel.Services
{
    public class OptionService : IOptionService
    {
        IUnitOfWork _uow;
        private readonly IDbSet<Option> _options;

        public OptionService(IUnitOfWork uow)
        {
            _uow = uow;
            _options = uow.Set<Option>();
        }

        public bool Update(UpdateOptionModel model)
        {
            bool b = false;
            //List<Option> options = _options.ToList();
            //options.Where(op => op.Name.Equals("FileUrl")).FirstOrDefault().Value = model.FileUrl;
            string ff = model.FileUrl;
            if(!string.IsNullOrEmpty(ff))
            {
                ff = (ff.EndsWith(@"\") ? ff.Substring(0, ff.LastIndexOf(@"\")):ff);
            }
            var entity = _options.Where(x => x.Name.Equals("FileUrl")).FirstOrDefault();
            if (entity != null)
            {
                entity.Value = ff;// model.FileUrl;

                _options.Attach(entity);
                _uow.Entry(entity).State = EntityState.Modified;
                b = _uow.SaveChanges() > 0;
            }
            return b;
        }

        public AppConfig GetAll()
        {
            List<Option> options = _options.ToList();
            var model = new AppConfig
            {
                FileUrl = options.Where(op => op.Name.Equals("FileUrl")).FirstOrDefault().Value               
            };
            return model;
        }

        
    }
}