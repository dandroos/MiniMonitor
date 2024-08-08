using LibreHardwareMonitor.Hardware;
using System;
using System.Linq;

public class PerformanceMonitor
{
    private readonly Computer _computer; // Represents the system hardware

    // Constructor to initialize hardware monitoring
    public PerformanceMonitor()
    {
        try
        {
            // Initialize the Computer object with components to be monitored
            _computer = new Computer
            {
                IsCpuEnabled = true, // Enable CPU monitoring
                IsGpuEnabled = true, // Enable GPU monitoring
                IsMemoryEnabled = true, // Enable Memory monitoring
                IsMotherboardEnabled = true // Enable Motherboard monitoring
            };
            _computer.Open(); // Open the computer object to start monitoring
        }
        catch (Exception ex)
        {
            // Log and rethrow any errors that occur during initialization
            LogError("Failed to initialize hardware monitoring.", ex);
            throw;
        }
    }

    // Method to get the CPU usage percentage
    public int GetCpuUsage()
    {
        try
        {
            // Find the first CPU hardware component
            var cpu = _computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);
            if (cpu == null) return 0; // Return 0 if no CPU component found

            cpu.Update(); // Update the CPU data
            // Find the first sensor of type Load within the CPU component
            var cpuLoad = cpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Load);
            // Return the rounded value of the CPU load, or 0 if no load sensor found
            return RoundValue(cpuLoad?.Value ?? 0);
        }
        catch (Exception ex)
        {
            // Log errors and return 0 if getting CPU usage fails
            LogError("Failed to get CPU usage.", ex);
            return 0;
        }
    }

    // Method to get the RAM usage percentage
    public int GetRamUsage()
    {
        try
        {
            // Find the first Memory hardware component
            var memory = _computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Memory);
            if (memory == null) return 0; // Return 0 if no Memory component found

            memory.Update(); // Update the Memory data
            // Find the first sensor of type Load within the Memory component
            var memoryLoad = memory.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Load);
            // Return the rounded value of the Memory load, or 0 if no load sensor found
            return RoundValue(memoryLoad?.Value ?? 0);
        }
        catch (Exception ex)
        {
            // Log errors and return 0 if getting RAM usage fails
            LogError("Failed to get RAM usage.", ex);
            return 0;
        }
    }

    // Method to get the GPU usage percentage
    public int GetGpuUsage()
    {
        try
        {
            // Array of possible GPU hardware types
            var gpuTypes = new[] { HardwareType.GpuNvidia, HardwareType.GpuAmd, HardwareType.GpuIntel };
            foreach (var gpuType in gpuTypes)
            {
                // Find the first hardware component of the current GPU type
                var gpu = _computer.Hardware.FirstOrDefault(h => h.HardwareType == gpuType);
                if (gpu != null)
                {
                    gpu.Update(); // Update the GPU data
                    // Find the first sensor of type Load within the GPU component
                    var gpuLoad = gpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Load);
                    if (gpuLoad != null)
                    {
                        // Return the rounded value of the GPU load
                        return RoundValue(gpuLoad.Value ?? 0);
                    }
                }
            }
            return 0; // Return 0 if no GPU load sensor found
        }
        catch (Exception ex)
        {
            // Log errors and return 0 if getting GPU usage fails
            LogError("Failed to get GPU usage.", ex);
            return 0;
        }
    }

    // Method to get the CPU temperature
    public int GetCpuTemperature()
    {
        try
        {
            // Find the first CPU hardware component
            var cpu = _computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);
            if (cpu == null) return 0; // Return 0 if no CPU component found

            cpu.Update(); // Update the CPU data
            // Find the first sensor of type Temperature within the CPU component
            var cpuTemp = cpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature);
            // Return the rounded value of the CPU temperature, or 0 if no temperature sensor found
            return RoundValue(cpuTemp?.Value ?? 0);
        }
        catch (Exception ex)
        {
            // Log errors and return 0 if getting CPU temperature fails
            LogError("Failed to get CPU temperature.", ex);
            return 0;
        }
    }

    // Method to get the GPU temperature
    public int GetGpuTemperature()
    {
        try
        {
            // Array of possible GPU hardware types
            var gpuTypes = new[] { HardwareType.GpuNvidia, HardwareType.GpuAmd, HardwareType.GpuIntel };
            foreach (var gpuType in gpuTypes)
            {
                // Find the first hardware component of the current GPU type
                var gpu = _computer.Hardware.FirstOrDefault(h => h.HardwareType == gpuType);
                if (gpu != null)
                {
                    gpu.Update(); // Update the GPU data
                    // Find the first sensor of type Temperature within the GPU component
                    var gpuTemp = gpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature);
                    if (gpuTemp != null)
                    {
                        // Return the rounded value of the GPU temperature
                        return RoundValue(gpuTemp.Value ?? 0);
                    }
                }
            }
            return 0; // Return 0 if no GPU temperature sensor found
        }
        catch (Exception ex)
        {
            // Log errors and return 0 if getting GPU temperature fails
            LogError("Failed to get GPU temperature.", ex);
            return 0;
        }
    }

    // Helper method to round float values to the nearest integer
    private int RoundValue(float value)
    {
        return (int)Math.Round(value); // Ensure rounding to the nearest whole number
    }

    // Helper method to log errors
    private void LogError(string message, Exception ex)
    {
        Console.WriteLine($"{DateTime.Now}: {message} - {ex.Message}"); // Log error messages to the console
    }
}
