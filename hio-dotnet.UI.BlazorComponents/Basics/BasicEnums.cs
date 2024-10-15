using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.UI.BlazorComponents.Basics
{
    public enum RowWidth 
    { 
        Full = 100, 
        Half = 50, 
        Quarter = 25, 
        Auto, 
        Custom 
    }

    public enum ColumnSize 
    { 
        Sm, 
        Md, 
        Lg, 
        Xl, 
        Xxl 
    }

    public enum ColumnWidth 
    { 
        Auto = 0,
        One = 1, 
        Two, 
        Three, 
        Four, 
        Five, 
        Six, 
        Seven, 
        Eight, 
        Nine, 
        Ten, 
        Eleven, 
        Twelve 
    }

    public enum ColumnWidthPercentage 
    { 
        Full = 100, 
        Half = 50, 
        Quarter = 25, 
        Auto, 
        Custom 
    }

}
