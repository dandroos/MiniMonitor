using System;
using System.Windows;

public partial class App : Application
{
    // This method is called when the application starts up
    protected override void OnStartup(StartupEventArgs e)
    {
        // Attach an event handler for unhandled exceptions
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        base.OnStartup(e);
    }

    // This method handles unhandled exceptions
    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        // Cast the exception object to Exception
        Exception ex = e.ExceptionObject as Exception;

        // Log the error details
        LogError("Unhandled exception occurred.", ex);

        // Show a message box to the user indicating an unexpected error
        MessageBox.Show("An unexpected error occurred. Please check the logs.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    // This method logs error messages to the console
    private void LogError(string message, Exception ex)
    {
        // Output the error message and exception details to the console
        Console.WriteLine($"{DateTime.Now}: {message} - {ex.Message}");
    }
}
