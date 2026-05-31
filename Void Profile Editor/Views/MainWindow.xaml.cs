using Autodesk.Revit.UI;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
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
        private static Mutex _mutex;
        private const string MutexName = @"Global\Void_Profile_Editor_Mutex";
        public MainWindow(MainWindowViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }

        public static void ShowOrActive(IServiceProvider serviceProvider)
        {
            if (_instance != null)
            {
                try
                {
                    _instance.Dispatcher.Invoke(() =>
                    {
                        {
                            if (_instance.IsLoaded)
                            {
                                _instance.Activate();                                
                            }
                        }
                    });
                    return;
                }
                catch
                {
                    _instance = null;
                    if (_mutex != null)
                    {
                        try { _mutex.ReleaseMutex(); } catch { }
                        _mutex.Dispose();
                        _mutex = null;
                    }
                }
                if (_instance != null)
                    return;
            }

            bool createdNew;
            _mutex=new Mutex(true, MutexName, out createdNew);
            if (!createdNew) 
            {
                bool acquired = false;
                try
                {
                    acquired = _mutex.WaitOne(TimeSpan.FromSeconds(1));
                }
                catch (AbandonedMutexException)
                {
                    // Мьютекс был брошен предыдущей аварией, теперь он наш
                    acquired = true;
                }
                if (!acquired)
                {
                    TaskDialog.Show("Внимание", "Плагин уже запущен");
                    _mutex.Dispose();
                    _mutex = null;
                    return;
                }
            }

            var viewModel=serviceProvider.GetService<MainWindowViewModel>();
            _instance=new MainWindow(viewModel);

            _instance.Closed += (s, e) =>
            {
                _instance = null;
                if (_mutex != null)
                {
                    try { _mutex.ReleaseMutex(); } catch { }
                    _mutex.Dispose();
                    _mutex = null;
                }
            };

            _instance.Show();
        }
    }
}
