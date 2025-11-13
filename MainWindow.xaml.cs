using L046_Labb3_Code_Along.Models;
using L046_Labb3_Code_Along.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace L046_Labb3_Code_Along;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    MainWindowViewModel viewModel;
    public MainWindow()
    {
        InitializeComponent();

        viewModel = new MainWindowViewModel();
        DataContext = viewModel;
        Closing += MainWindow_Closing;

    }
    
    private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        //   throw new NotImplementedException();
        viewModel.ConfigurationViewModel?.SaveQuestions("questions.json");
    }

}