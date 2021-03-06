using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace Microsoft.VisualStudio.ProjectSystem.VS.Implementation.PropertyPages.Designer
{
    public static class FileBrowseEditor
    {
        public static ICommand BrowseCommand { get; } = new DelegateCommand<PropertyValue>(
            value =>
            {
                // TODO allow editor metadata to control things like file/directory path, file extensions, starting directory (project default), relative/absolute

                var dialog = new OpenFileDialog
                {
                    FileName = value.EvaluatedValue is string s ? s : ""
                };

                var result = dialog.ShowDialog(Application.Current.MainWindow);

                if (result == true)
                {
                    value.EvaluatedValue = dialog.FileName;
                }
            });
    }
}