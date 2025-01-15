using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.UI.BlazorComponents.RadzenLib.ThingsBoard.Models
{
    public enum OpenedTabType
    {
        None,
        Device
    }

    public class OpenedTab
    {
        public OpenedTabType Type { get; set; } = OpenedTabType.None;
        public string Title { get; set; } = "Tab";
        public RenderFragment? Content { get; set; }
        public Guid Id { get; set; } = Guid.NewGuid();
        public bool Visible { get; set; } = true;
        public bool IsSelected { get; set; } = true;

        public object? Data { get; set; }
    }
}
