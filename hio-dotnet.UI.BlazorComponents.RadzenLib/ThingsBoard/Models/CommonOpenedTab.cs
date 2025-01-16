using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.UI.BlazorComponents.RadzenLib.ThingsBoard.Models
{
    public abstract class CommonOpenedTab : IOpenedTab
    {
        public OpenedTabType Type { get; set; }
        public string Title { get; set; }
        public RenderFragment? Content { get; set; }
        public Guid Id { get; set; }
        public bool Visible { get; set; } = true;
        public bool IsSelected { get; set; }
        public object? Data { get; set; }
    }
}
