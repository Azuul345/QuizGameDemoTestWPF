using System.Windows;
using System.Windows.Controls;

namespace QuizGame.Views
{
    /// <summary>
    /// Interaction logic for EditView.xaml
    /// </summary>
    public partial class EditView : UserControl
    {
        public EditView()
        {
            InitializeComponent();
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this)!.Content = new MenuView();
        }
    }
}
