using System.ComponentModel;

namespace Power_BI_Embedded_Explorer.Models
{
  public class BaseModel : INotifyPropertyChanged
  {
    public BaseModel()
    {

    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void NotifyPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
