using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;

namespace PerformanceMonitorApp
{
    public partial class PerformanceMonitorWindow : Window
    {
        private readonly HotkeyManager hotkeyManager; // Manages global hotkeys
        private readonly DispatcherTimer updateTimer = new DispatcherTimer(); // Timer for updating performance data
        private readonly PerformanceMonitor monitor = new PerformanceMonitor(); // Monitors system performance

        // Constructor to initialize the window
        public PerformanceMonitorWindow()
        {
            InitializeComponent(); // Initialize XAML components
            hotkeyManager = new HotkeyManager(this); // Create HotkeyManager instance
            ConfigureWindow(); // Configure window settings
            StartMonitoring(); // Start updating performance data
        }

        // Configures window settings, including hotkeys
        private void ConfigureWindow()
        {
            try
            {
                // Register a hotkey (F10) to toggle window visibility
                int toggleVisibilityHotkeyId = hotkeyManager.RegisterHotkey(KeyInterop.VirtualKeyFromKey(Key.F10), KeyModifier.None, ToggleVisibility);
                this.Topmost = true; // Ensure window stays on top
                MakeClickThrough(); // Make window click-through
            }
            catch (Exception ex)
            {
                // Log errors and show an error message if configuration fails
                LogError("Failed to configure the window.", ex);
                MessageBox.Show("Failed to configure the window. Please check the logs for more details.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown(); // Shut down the application
            }
        }

        // Starts the performance monitoring
        private void StartMonitoring()
        {
            try
            {
                updateTimer.Interval = TimeSpan.FromSeconds(1); // Set update interval to 1 second
                updateTimer.Tick += UpdatePerformanceData; // Attach event handler for timer ticks
                updateTimer.Start(); // Start the timer
            }
            catch (Exception ex)
            {
                // Log errors and show an error message if monitoring fails
                LogError("Failed to start monitoring performance data.", ex);
                MessageBox.Show("Failed to start monitoring performance data. Please check the logs for more details.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown(); // Shut down the application
            }
        }

        // Updates the performance data displayed in the window
        private void UpdatePerformanceData(object sender, EventArgs e)
        {
            try
            {
                // Retrieve various performance metrics
                var cpuUsage = monitor.GetCpuUsage();
                var ramUsage = monitor.GetRamUsage();
                var gpuUsage = monitor.GetGpuUsage();
                var cpuTemp = monitor.GetCpuTemperature();
                var gpuTemp = monitor.GetGpuTemperature();

                // Retrieve current screen resolution
                var screenWidth = SystemParameters.PrimaryScreenWidth;
                var screenHeight = SystemParameters.PrimaryScreenHeight;
                var resolution = $"{screenWidth}x{screenHeight}";

                // Update the text block with performance metrics and resolution
                PerformanceTextBlock.Inlines.Clear();
                AddFormattedText("CPU: ", true);
                AddFormattedText($"{cpuUsage}%", false);
                AddFormattedText(" | ", false);
                AddFormattedText($"{cpuTemp}°C   ", false);
                AddFormattedText("GPU: ", true);
                AddFormattedText($"{gpuUsage}%", false);
                AddFormattedText(" | ", false);
                AddFormattedText($"{gpuTemp}°C   ", false);
                AddFormattedText("RAM: ", true);
                AddFormattedText($"{ramUsage}%", false);
                AddFormattedText("   ", false);
                AddFormattedText(resolution, false); // Add the resolution to the end
            }
            catch (Exception ex)
            {
                // Log errors if updating performance data fails
                LogError("Failed to update performance data.", ex);
            }
        }


        // Toggles the visibility of the window
        private void ToggleVisibility()
        {
            try
            {
                this.Visibility = this.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            }
            catch (Exception ex)
            {
                // Log errors if toggling visibility fails
                LogError("Failed to toggle window visibility.", ex);
            }
        }

        // Makes the window click-through by setting its extended window style
        private void MakeClickThrough()
        {
            try
            {
                var hwnd = new WindowInteropHelper(this).Handle; // Get the window handle
                int style = GetWindowLong(hwnd, GWL_EXSTYLE); // Get current window style
                SetWindowLong(hwnd, GWL_EXSTYLE, style | WS_EX_TRANSPARENT); // Set window to be click-through
            }
            catch (Exception ex)
            {
                // Log errors if making the window click-through fails
                LogError("Failed to make the window click-through.", ex);
            }
        }

        // Adds formatted text to the performance text block
        private void AddFormattedText(string text, bool isBold)
        {
            var run = new Run(text) { FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal };
            PerformanceTextBlock.Inlines.Add(run);
        }

        // Logs error messages to the console
        private void LogError(string message, Exception ex)
        {
            Console.WriteLine($"{DateTime.Now}: {message} - {ex.Message}");
        }

        // P/Invoke declarations for interacting with the Windows API
        private const int GWL_EXSTYLE = -20; // Index for extended window style
        private const int WS_EX_TRANSPARENT = 0x20; // Extended style for click-through

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    }
}
