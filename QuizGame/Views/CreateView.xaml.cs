using System.Windows;
using System.Windows.Controls;

namespace QuizGame.Views
{
    /// <summary>
    /// Interaction logic for CreateView.xaml
    /// </summary>
    public partial class CreateView : UserControl
    {
        public CreateView()
        {
            InitializeComponent();
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this)!.Content = new MenuView();
        }
    }
}
