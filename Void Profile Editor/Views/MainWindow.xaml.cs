using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using Void_Profile_Editor.ViewModels;

namespace Void_Profile_Editor.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static MainWindow _instance;
        public MainWindow(MainWindowViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }

        public static void ShowOrActive(IServiceProvider serviceProvider)
        {
            if (_instance == null)
            {
                // если окно не создано - создаем новое окно
                var viewModel = serviceProvider.GetRequiredService<MainWindowViewModel>();
                _instance = new MainWindow(viewModel);
                // при закрытии окна очищаем поле _instance
                _instance.Closed += (s, e) =>
                {
                    _instance = null;
                };
                // показываем окно
                _instance.Show();
            }
            else
            {
                // если окно уже существует - показываем его
                _instance.Show();
            }
        }
    }
}
