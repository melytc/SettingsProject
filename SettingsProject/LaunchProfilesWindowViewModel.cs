using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
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

        public ICommand NewCommand { get; }

        public ImmutableArray<LaunchProfileKind> ProfileKinds { get; }

        public LaunchProfilesWindowViewModel(ObservableCollection<LaunchProfileViewModel> profiles, ImmutableArray<LaunchProfileKind> profileKinds)
        {
            Profiles = profiles;
            ProfileKinds = profileKinds;
            SelectedProfile = profiles.First();

            CloneCommand = new DelegateCommand<LaunchProfileViewModel>(profile =>
            {
                var index = Profiles.IndexOf(profile);

                var clone = profile.Clone();

                Profiles.Insert(index + 1, clone);
            });

            DeleteCommand = new DelegateCommand<LaunchProfileViewModel>(profile => Profiles.Remove(profile));
            
            RenameCommand = new DelegateCommand<LaunchProfileViewModel>(profile => profile.IsRenaming = true);

            NewCommand = new DelegateCommand<LaunchProfileKind>(kind =>
            {
                //TODO: use real data
                var context = new SettingContext(SettingsLoader.DefaultConfigurationDictionary, LaunchProfilesWindow.Conditions, false, kind.Metadata.Select(md => new Setting(md, new SettingValue(ImmutableDictionary<string, string>.Empty, ""))).ToImmutableArray());

                var newProfile = new LaunchProfileViewModel("New profile", kind, context);
                
                Profiles.Add(newProfile);
                
                // TODO: select the profile after creating it, implement a notification.
                //SelectedProfile = newProfile;
            });
        }
    }
}
