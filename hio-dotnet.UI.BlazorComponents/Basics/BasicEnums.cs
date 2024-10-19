using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.UI.BlazorComponents.Basics
{
    public enum Color
    {
        Default,
        Primary,
        Secondary,
        Success,
        Danger,
        Warning,
        Info,
        Light,
        Dark,
        Link
    }

    public enum TextSize
    {
        ExtraSmall,
        Small,
        Medium,
        Large
    }

    public enum ContainerType
    {
        Default,    // Corresponds to Bootstrap's .container class
        Fluid,      // Corresponds to Bootstrap's .container-fluid class
        Custom      // Allows for a custom container class
    }


    #region RowSpecific
    public enum RowWidth
    {
        Full = 100,
        Half = 50,
        Quarter = 25,
        Auto,
        Custom
    }
    #endregion

    #region ColumnSpecific
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
    #endregion

    #region ModalSpecific
    public enum ModalSize
    {
        Small,
        Medium,
        Large,
        ExtraLarge
    }

    #endregion

    #region TextSpecific
    public enum TextEditWidthPercentage
    {
        Full = 100,
        Half = 50,
        Quarter = 25,
        Auto,
        Custom
    }

    #endregion

    #region TablesSpecific
    public enum TableWidth
    {
        Auto,
        Full,
        Half,
        Quarter,
        Custom
    }

    public enum TableVariant
    {
        Default,
        Striped,
        Bordered,
        Hover,
        Condensed,
        Custom
    }

    #endregion
}
