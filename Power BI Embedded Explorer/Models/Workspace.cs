namespace Power_BI_Embedded_Explorer.Models
{
  public class Workspace : BaseModel
  {
    private string _workspaceId;
    public string WorkspaceId
    {
      get { return _workspaceId; }
      set
      {
        if (!value.Equals(_workspaceId))
        {
          _workspaceId = value;
          NotifyPropertyChanged("WorkspaceId");
        }
      }
    }
  }
}
