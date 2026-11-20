using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PurpleLeech.Gui.Views
{
    public partial class AuthView : UserControl
    {
        public AuthView()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}