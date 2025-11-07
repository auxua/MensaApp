using CommunityToolkit.Mvvm.Input;
using MensaApp2.Models;

namespace MensaApp2.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}