namespace Power_BI_Embedded_Explorer
{
  using Microsoft.Win32;
  using Models;
  using System.Diagnostics;
  using System.IO;
  using System.Windows;
  using System.Windows.Controls;
  using ViewModels;

  public partial class MainWindow : Window
  {
    private MainWindowViewModel _viewModel = new MainWindowViewModel();

    private Workspace selectedWorkspace;
    
    public MainWindow()
    {
      InitializeComponent();

      DataContext = _viewModel;
    }

    private async void OnLoadButtonClicked(object sender, RoutedEventArgs e)
    {
      string wrkColTxt = WorkspaceCollection.Text.Trim(),
             accessKeyTxt = AccessKey.Text.Trim();

      if (string.IsNullOrEmpty(wrkColTxt) || string.IsNullOrEmpty(accessKeyTxt))
      {
        MessageBox.Show("Please enter Workspace Collection name and Access key.");
      }
      else
      {
        await _viewModel.LoadWorkspacesAsync(wrkColTxt, accessKeyTxt);
        if (_viewModel.Workspaces.Count > 0)
        {
          WorkspaceList.SelectedIndex = 0;
        }
      }
    }

    private async void OnAddWorkspaceButtonClicked(object sender, RoutedEventArgs e)
    {
      string wrkColTxt = WorkspaceCollection.Text.Trim(),
             accessKeyTxt = AccessKey.Text.Trim();

      await _viewModel.CreateWorkspaceAsync(wrkColTxt, accessKeyTxt);
    }

    private async void OnImportButtonClicked(object sender, RoutedEventArgs e)
    {
      string wrkColTxt = WorkspaceCollection.Text.Trim(),
             accessKeyTxt = AccessKey.Text.Trim();

      var ofd = new OpenFileDialog();
      ofd.Filter = "Power BI files (*.pbix)|*.pbix";

      if (ofd.ShowDialog() == true)
      {
        string datasetName = DatasetName.Text.Trim();
        if (string.IsNullOrEmpty(datasetName))
        {
          datasetName = null;
        }
        await _viewModel.ImportPbixAsync(wrkColTxt, accessKeyTxt, selectedWorkspace.WorkspaceId, ofd.FileName, datasetName);
      }

    }

    private void WorkspaceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      selectedWorkspace = WorkspaceList.SelectedItem as Workspace;
      DatasetName.IsEnabled = true;
      ImportButton.IsEnabled = true;
    }
  }
}
