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
    public (int CoreMax, int Total) GetCpuUsage()
    {
        try
        {
            var cpu = _computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);
            if (cpu == null) return (0, 0); // Return 0 if no CPU component found

            cpu.Update(); // Update the CPU data

            var coreMax = cpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Load && s.Name.Contains("Core Max"));
            var totalLoad = cpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Load && s.Name.Contains("Total"));

            return (RoundValue(coreMax?.Value ?? 0), RoundValue(totalLoad?.Value ?? 0));
        }
        catch (Exception ex)
        {
            LogError("Failed to get CPU usage.", ex);
            return (0, 0);
        }
    }

    // Method to get the RAM usage percentage
    public int GetRamUsage()
    {
        try
        {
            var memory = _computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Memory);
            if (memory == null) return 0; // Return 0 if no Memory component found

            memory.Update(); // Update the Memory data

            var memoryLoad = memory.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Load);
            return RoundValue(memoryLoad?.Value ?? 0);
        }
        catch (Exception ex)
        {
            LogError("Failed to get RAM usage.", ex);
            return 0;
        }
    }

    // Method to get the GPU usage percentage
    public int GetGpuUsage()
    {
        try
        {
            var gpuTypes = new[] { HardwareType.GpuNvidia, HardwareType.GpuAmd, HardwareType.GpuIntel };

            foreach (var gpuType in gpuTypes)
            {
                var gpu = _computer.Hardware.FirstOrDefault(h => h.HardwareType == gpuType);
                if (gpu != null)
                {
                    gpu.Update();

                    // Look for the "GPU Core" or equivalent sensor representing overall GPU load
                    var gpuLoad = gpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Load && s.Name.Contains("Core") || s.Name.Contains("Total"));

                    if (gpuLoad != null && gpuLoad.Value.HasValue)
                    {
                        return RoundValue(gpuLoad.Value.Value);
                    }
                }
            }

            return 0; // Return 0 if no suitable GPU load sensor found
        }
        catch (Exception ex)
        {
            LogError("Failed to get GPU usage.", ex);
            return 0;
        }
    }

    // Method to get the CPU temperature
    public int GetCpuTemperature()
    {
        try
        {
            var cpu = _computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);
            if (cpu == null) return 0; // Return 0 if no CPU component found

            cpu.Update(); // Update the CPU data

            var cpuTemp = cpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature && (s.Name.Contains("Tctl") || s.Name.Contains("Tdie")));
            if (cpuTemp == null)
            {
                cpuTemp = cpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature);
            }

            return RoundValue(cpuTemp?.Value ?? 0);
        }
        catch (Exception ex)
        {
            LogError("Failed to get CPU temperature.", ex);
            return 0;
        }
    }

    // Method to get the GPU temperature
    public int GetGpuTemperature()
    {
        try
        {
            var gpuTypes = new[] { HardwareType.GpuNvidia, HardwareType.GpuAmd, HardwareType.GpuIntel };
            foreach (var gpuType in gpuTypes)
            {
                var gpu = _computer.Hardware.FirstOrDefault(h => h.HardwareType == gpuType);
                if (gpu != null)
                {
                    gpu.Update();

                    var gpuTemp = gpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature);
                    if (gpuTemp != null)
                    {
                        return RoundValue(gpuTemp.Value ?? 0);
                    }
                }
            }
            return 0; // Return 0 if no GPU temperature sensor found
        }
        catch (Exception ex)
        {
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
