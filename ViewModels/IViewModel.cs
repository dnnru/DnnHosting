#region Usings

using R7.Dnn.Extensions.ViewModels;

#endregion

namespace Italliance.Modules.DnnHosting.ViewModels
{
    public interface IViewModel<TSettngs> where TSettngs : class, new()
    {
        void SetContext(ViewModelContext<TSettngs> context);
    }
}
