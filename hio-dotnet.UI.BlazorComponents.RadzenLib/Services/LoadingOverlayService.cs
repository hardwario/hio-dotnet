using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.UI.BlazorComponents.RadzenLib.Services
{
    public class LoadingOverlayService
    {
        public bool IsBusy { get; private set; } = false;

        public event EventHandler<bool> OnChange;

        public LoadingOverlayService() { }

        public void Show()
        {
            IsBusy = true;
            OnChange?.Invoke(this, IsBusy);
        }

        public void Hide()
        {
            IsBusy = false;
            OnChange?.Invoke(this, IsBusy);
        }
    }
}
