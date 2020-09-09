using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

#nullable enable

namespace SettingsProject
{
    internal sealed class LaunchProfilesWindowViewModel
    {
        public ObservableCollection<LaunchProfileViewModel> Profiles { get; }

        public LaunchProfileViewModel? SelectedProfile { get; set; }

        public ICommand CloneCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RenameCommand { get; }

        public LaunchProfilesWindowViewModel(ObservableCollection<LaunchProfileViewModel> profiles)
        {
            Profiles = profiles;
            SelectedProfile = profiles.First();

            CloneCommand = new DelegateCommand(() =>
            {
                var index = Profiles.IndexOf(SelectedProfile);

                var clone = SelectedProfile.Clone();

                Profiles.Insert(index + 1, clone);
            });

            DeleteCommand = new DelegateCommand(() => Profiles.Remove(SelectedProfile));
            
            RenameCommand = new DelegateCommand(
                () =>
                {
                    if (SelectedProfile != null)
                    {
                        SelectedProfile.IsRenaming = true;
                    }
                });
        }
    }
}
