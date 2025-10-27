using System.Windows;
using System.Windows.Controls;

namespace QuizGame.Views
{
    /// <summary>
    /// Interaction logic for MenuView.xaml
    /// </summary>
    public partial class MenuView : UserControl
    {
        public MenuView()
        {
            InitializeComponent();
        }
        private void Play_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Content = new QuizSelectWindow();   // go to Play. 
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Content = new EditView();   // go to Edit
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Content = new CreateView(); // go to Create
        }
    }
}
