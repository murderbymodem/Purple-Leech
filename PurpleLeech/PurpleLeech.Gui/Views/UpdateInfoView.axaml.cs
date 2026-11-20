using Avalonia.Controls;
using PurpleLeech.Shared.Extensions;
using PurpleLeech.Shared.Reflection;

namespace PurpleLeech.Gui.Views
{
    public partial class UpdateInfoView : Window
    {
        public UpdateInfoView()
        {
            AssemblyUtil au = AssemblyUtil.Get;

            Title = au.GetProductName() + " " + au.GetAssemblyVersion().Trim();
        }
    }
}