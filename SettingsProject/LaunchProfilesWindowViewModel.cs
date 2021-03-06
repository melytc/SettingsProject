﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

#nullable enable

namespace Microsoft.VisualStudio.ProjectSystem.VS.Implementation.PropertyPages.Designer
{
    internal sealed class LaunchProfilesWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private LaunchProfileViewModel? _selectedProfile;

        public ObservableCollection<LaunchProfileViewModel> Profiles { get; }

        public ICommand CloneCommand { get; }

        public ICommand DeleteCommand { get; }

        public ICommand RenameCommand { get; }

        public ICommand NewCommand { get; }

        public ImmutableArray<LaunchProfileKind> ProfileKinds { get; }

        public LaunchProfileViewModel? SelectedProfile
        {
            get => _selectedProfile;
            set
            {
                if (!ReferenceEquals(value, _selectedProfile))
                {
                    _selectedProfile = value;
                    OnPropertyChanged();
                }
            }
        }

        public LaunchProfilesWindowViewModel(ObservableCollection<LaunchProfileViewModel> profiles, ImmutableArray<LaunchProfileKind> profileKinds)
        {
            Profiles = profiles;
            ProfileKinds = profileKinds;
            SelectedProfile = profiles.FirstOrDefault();

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
                //TODO: find appropriate default value for each property somehow
                var properties = kind.Metadata.Select(md => new Property(md, new PropertyValue("", ""))).ToImmutableArray();

                var context = new PropertyContext(Array.Empty<KeyValuePair<string, ImmutableArray<string>>>(), kind.Conditions, properties);

                var newProfile = new LaunchProfileViewModel(Resources.LaunchProfileNewProfileName, kind, context) { IsRenaming = true };

                Profiles.Add(newProfile);
                
                SelectedProfile = newProfile;
            });
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
