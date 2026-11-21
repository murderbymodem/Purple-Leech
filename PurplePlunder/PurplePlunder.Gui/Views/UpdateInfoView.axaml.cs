using Avalonia.Controls;
using PurplePlunder.Shared.Extensions;
using PurplePlunder.Shared.Reflection;

namespace PurplePlunder.Gui.Views
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