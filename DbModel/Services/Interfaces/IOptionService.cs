

using DbModel.ViewModel.OptionVM;

namespace DbModel.Services.Interfaces
{
    public interface IOptionService
    {
        //bool ModeratingComment { get; }
        AppConfig GetAll();
        bool Update(UpdateOptionModel model);
    }
}