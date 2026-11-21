using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PurplePlunder.Gui.Views
{
    public partial class AuthView : UserControl
    {
        public AuthView()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}