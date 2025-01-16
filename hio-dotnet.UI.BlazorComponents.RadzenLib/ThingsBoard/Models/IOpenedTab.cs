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
    public interface IOpenedTab
    {
        OpenedTabType Type { get; set; }
        string Title { get; set; }
        RenderFragment? Content { get; set; }
        Guid Id { get; set; }
        bool Visible { get; set; }
        bool IsSelected { get; set; }

        object? Data { get; set; }
    }
}
