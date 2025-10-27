using QuizGame.Views;
using System.Windows;

namespace QuizGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //this.Content = new QuizGame.Views.MenuView();
            this.Content = new MenuView();
        }


    }
}