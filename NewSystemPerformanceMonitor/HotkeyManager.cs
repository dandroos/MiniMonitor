using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

public class HotkeyManager
{
    private readonly IntPtr hwnd; // Handle to the application window
    private int nextHotkeyId = 1; // ID counter for hotkeys
    private readonly Dictionary<int, Action> hotkeyActions = new Dictionary<int, Action>(); // Map of hotkey IDs to actions

    // Import methods to register and unregister hotkeys from the Windows API
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    // Constructor to initialize the HotkeyManager with the application's window handle
    public HotkeyManager(Window window)
    {
        var helper = new WindowInteropHelper(window);
        hwnd = helper.Handle; // Get the window handle
        ComponentDispatcher.ThreadPreprocessMessage += ThreadPreprocessMessageMethod; // Register message handling method
    }

    // Method to process Windows messages for hotkey events
    private void ThreadPreprocessMessageMethod(ref MSG msg, ref bool handled)
    {
        if (msg.message == 0x0312) // Hotkey message
        {
            int hotkeyId = msg.wParam.ToInt32(); // Get the hotkey ID from the message
            if (hotkeyActions.TryGetValue(hotkeyId, out var action))
            {
                action?.Invoke(); // Execute the action associated with the hotkey
                handled = true; // Indicate that the message has been handled
            }
        }
    }

    // Method to register a new hotkey
    public int RegisterHotkey(int virtualKeyCode, KeyModifier modifier, Action action)
    {
        try
        {
            int hotkeyId = nextHotkeyId++; // Get a new hotkey ID
            // Register the hotkey with the Windows API
            if (!RegisterHotKey(hwnd, hotkeyId, (uint)modifier, (uint)virtualKeyCode))
            {
                int errorCode = Marshal.GetLastWin32Error(); // Get the error code
                throw new InvalidOperationException($"Hotkey registration failed with error code {errorCode}.");
            }

            hotkeyActions[hotkeyId] = action; // Map the hotkey ID to the action
            return hotkeyId;
        }
        catch (Exception ex)
        {
            // Log and rethrow any errors that occur during hotkey registration
            LogError($"Failed to register hotkey {virtualKeyCode} with modifier {modifier}.", ex);
            throw;
        }
    }

    // Method to unregister an existing hotkey
    public void UnregisterHotkey(int hotkeyId)
    {
        try
        {
            if (hotkeyActions.ContainsKey(hotkeyId))
            {
                // Unregister the hotkey with the Windows API
                if (!UnregisterHotKey(hwnd, hotkeyId))
                {
                    int errorCode = Marshal.GetLastWin32Error(); // Get the error code
                    throw new InvalidOperationException($"Failed to unregister hotkey with ID {hotkeyId}. Error code: {errorCode}.");
                }
                hotkeyActions.Remove(hotkeyId); // Remove the hotkey from the dictionary
            }
        }
        catch (Exception ex)
        {
            // Log and rethrow any errors that occur during hotkey unregistration
            LogError($"Failed to unregister hotkey with ID {hotkeyId}.", ex);
            throw;
        }
    }

    // Method to log errors to the console
    private void LogError(string message, Exception ex)
    {
        // Output the error message and exception details to the console
        Console.WriteLine($"{DateTime.Now}: {message} - {ex.Message}");
    }
}

// Enum to represent key modifiers (e.g., Alt, Ctrl)
[Flags]
public enum KeyModifier
{
    None = 0, // No modifier
    Alt = 1, // Alt key
    Control = 2, // Ctrl key
    Shift = 4, // Shift key
    WinKey = 8 // Windows key
}
