namespace Power_BI_Embedded_Explorer.ViewModels
{
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using Microsoft.PowerBI.Api.Beta;
  using Models;
  using System.Threading.Tasks;
  using Microsoft.PowerBI.Security;
  using Microsoft.Rest;
  using System.IO;
  using Microsoft.PowerBI.Api.Beta.Models;
  using System.Threading;

  public class MainWindowViewModel : INotifyPropertyChanged
  {
    public ObservableCollection<Models.Workspace> Workspaces { get; private set; }

    private bool _isLoading;
    public bool IsLoading
    {
      get { return _isLoading; }
      private set
      {
        if (value != _isLoading)
        {
          _isLoading = value;
          NotifyPropertyChanged("IsLoading");
        }
      }
    }

    private bool _isLoaded;
    public bool IsLoaded
    {
      get { return _isLoaded; }
      private set
      {
        if (value != _isLoaded)
        {
          _isLoaded = value;
          NotifyPropertyChanged("IsLoaded");
        }
      }
    }

    private string _status;
    public string Status
    {
      get { return _status; }
      private set
      {
        if (value != _status)
        {
          _status = value;
          NotifyPropertyChanged("Status");
        }
      }
    }

    public MainWindowViewModel()
    {
      Workspaces = new ObservableCollection<Models.Workspace>();

      Status = "Ready";
      IsLoading = false;
      IsLoaded = false;
    }

    /// <summary>
    /// Load workspaces for the given Workspace Collection Name
    /// </summary>
    /// <param name="workspaceCollectionName"></param>
    /// <param name="accessKey"></param>
    /// <returns></returns>
    public async Task LoadWorkspacesAsync(string workspaceCollectionName, string accessKey)
    {
      Status = "Loading...";
      IsLoading = true;
      IsLoaded = false;

      Workspaces.Clear();

      var provisionToken = PowerBIToken.CreateProvisionToken(workspaceCollectionName);      
      using (var client = CreateClient(provisionToken, accessKey))
      {
        var response = await client.Workspaces.GetWorkspacesByCollectionNameAsync(workspaceCollectionName);
        if (response.Value != null)
        {
          foreach(var workspace in response.Value)
          {
            Workspaces.Add(new Models.Workspace() { WorkspaceId = workspace.WorkspaceId });
          }

          Status = "Loaded.";
          IsLoaded = true;
        }
        else
        {
          Status = "Failed. Cannot get workspaces";
        }
      }

      IsLoading = false;
    }


    public async Task<string> CreateWorkspaceAsync(string workspaceCollectionName, string accessKey)
    {
      string ret = null;

      Status = "Loading...";
      IsLoading = true;
      IsLoaded = false;

      var provisionToken = PowerBIToken.CreateProvisionToken(workspaceCollectionName);
      using (var client = CreateClient(provisionToken, accessKey))
      {
        var workspace = await client.Workspaces.PostWorkspaceAsync(workspaceCollectionName);
        if (workspace != null)
        {
          ret = workspace.WorkspaceId;
          Workspaces.Add(new Models.Workspace() { WorkspaceId = ret });
        }
      }

      IsLoading = false;
      IsLoaded = true;
      return ret;
    }

    public async Task<Import> ImportPbixAsync(string workspaceCollectionName, string accessKey, string workspaceId, string filePath, string datasetName)
    {
      Status = "Importing...";
      IsLoading = true;
      IsLoaded = false;

      var devToken = PowerBIToken.CreateDevToken(workspaceCollectionName, workspaceId);
      using (var fileStream = File.OpenRead(filePath))
      {
        using (var client = CreateClient(devToken, accessKey))
        {
          var import = await client.Imports.PostImportWithFileAsync(workspaceCollectionName, workspaceId, fileStream, datasetName);

          while (import.ImportState != "Succeeded" && import.ImportState != "Failed")
          {
            import = await client.Imports.GetImportByIdAsync(workspaceCollectionName, workspaceId, import.Id);
            Status = $"Checking import state... {import.ImportState}";
            Thread.Sleep(1000);
          }

          IsLoading = false;
          IsLoaded = true;
          Status = "Importing task is completed.";
          return import;
        }
      }
    }


    public event PropertyChangedEventHandler PropertyChanged;
    private void NotifyPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    static IPowerBIClient CreateClient(PowerBIToken provisionToken, string accessKey)
    {
      var jwt = provisionToken.Generate(accessKey);
      var credentials = new TokenCredentials(jwt, "AppToken");
      var client = new PowerBIClient(credentials);
      client.BaseUri = new System.Uri("https://api.powerbi.com");

      return client;
    }
  }
}
