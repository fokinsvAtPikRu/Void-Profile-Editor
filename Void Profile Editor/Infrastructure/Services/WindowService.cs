using System;
using System.Windows;
using Void_Profile_Editor.Domain.Abstraction.Services;

namespace Void_Profile_Editor.Infrastructure.Services
{
    public class WindowService : IWindowService
    {
        private Window _window;

        public void SetWindow(Window window)
        {
            _window = window ?? throw new ArgumentNullException(nameof(window));
        }

        public void HideWindow()
        {
            if (_window.Dispatcher.CheckAccess())
            {
                _window.Hide();
            }
            else
            {
                _window.Dispatcher.Invoke(() => _window.Hide());
            }
        }

        public void ShowWindow()
        {
            if (_window.Dispatcher.CheckAccess())
            {
                _window.Show();
                _window.Activate();
                _window.Topmost = true;
                _window.Topmost = false;
                _window.Focus();
            }
            else
            {
                _window.Dispatcher.Invoke(() =>
                {
                    _window.Show();
                    _window.Activate();
                    _window.Topmost = true;
                    _window.Topmost = false;
                    _window.Focus();
                });
            }
        }
        public void BringToFront()
        {
            if (_window.Dispatcher.CheckAccess())
            {
                if (_window.WindowState == WindowState.Minimized)
                    _window.WindowState = WindowState.Normal;

                _window.Activate();
                _window.Topmost = true;
                _window.Topmost = false;
            }
            else
            {
                _window.Dispatcher.Invoke(() =>
                {
                    if (_window.WindowState == WindowState.Minimized)
                        _window.WindowState = WindowState.Normal;

                    _window.Activate();
                    _window.Topmost = true;
                    _window.Topmost = false;
                });
            }
        }
    }
}
