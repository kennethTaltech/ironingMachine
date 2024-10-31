// Global data class to hold ironing methods and their temperature ranges
public static class GlobalData
{
    // Dictionary to store the temperature range for different ironing programs
    public static Dictionary<string, (int, int)> ironingMethods = new Dictionary<string, (int, int)>() { 
        {
            "Linen", (200,230)
        }, {
            "Cotton", (150, 199)
        }, {
            "Silk", (120, 149)
        }, {
            "Synthetics", (90, 119)
        } 
    };

}

// Interface defining the methods that all ironing machines must implement
public interface IroningMachine
{
    void Descale();
    void DoIroning(int temperature);
    void DoIroning(string program);
    void UseSteam();
    void TurnOn();
    void TurnOff();
}

// Base class for all types of irons, implementing the IroningMachine interface
public class BaseIron : IroningMachine
{
    // Fields to track usage, state, and configuration of the iron
    protected int usageCount; // Counts how many times DoIroning has been called
    protected bool isSteamOn; // Tracks if steam is currently on
    protected string machineType; // Stores the type of machine (Regular, Premium, Linen)
    protected bool isTurnedOn; // Indicates if the machine is turned on
    protected int maxTemp; // Maximum temperature the machine can reach
    protected int steamUsageCount;  // Counts how many times steam has been used
    protected bool wateringLightIsOn;  // Counts how many times steam has been used

    // Constructor to initialize the iron with its type and maximum temperature
    public BaseIron(string machineType, int maxTemp)
    {
        usageCount = 0;
        this.maxTemp = maxTemp;
        this.machineType = machineType;
        wateringLightIsOn = false;
        isSteamOn = false;
        isTurnedOn = false;
        steamUsageCount = 0;
    }

    // Method to turn the iron on
    public void TurnOn()
    {
        isTurnedOn = true;
        Console.WriteLine($"{machineType} iron is turned on.");
    }

    // Method to turn the iron off
    public void TurnOff()
    {
        isTurnedOn = false;
        Console.WriteLine($"{machineType} iron is turned off.");
    }

    // Method to descale (clean) the iron, resetting the usage count
    public void Descale()
    {
        steamUsageCount = 0;
        usageCount = 0;
        Console.WriteLine($"{machineType} iron is cleaned.");
    }

    // Method to use steam during ironing
    public void UseSteam()
    {
        if (isSteamOn)
        {
            Console.WriteLine("Steam is already on."); // Notify if steam is already on
        }
        else
        {
            isSteamOn = true;
            Console.WriteLine("Steam is now on."); // Turn on steam
        }
    }

    // Method to turn off steam (protected as it's used internally)
    protected void TurnOffSteam()
    {
        isSteamOn = false;
    }


    // Method to perform ironing based on a specific temperature
    public void DoIroning(int ironingTemp) {
        // Validate the temperature range
        if (ironingTemp > 230 || ironingTemp < 90) { 
            
            Console.WriteLine("Invalid temperature range. Unable to iron at this temperature.");
            return; // Exit if the temperature is invalid

        }

        if (needsCleaning(usageCount)) return; // Check if the machine needs cleaning
        usageCount++; // Increment usage count

        // Check if the temperature is within the machine's maximum allowed temperature
        if (ironingTemp <= maxTemp)
        {
            string program = GetProgramByTemperature(ironingTemp); // Get program based on temperature

            // If steam is on and the temperature is below 120, turn off steam
            if (isSteamOn && ironingTemp < 120)
            {
                Console.WriteLine($"Not ironing. Turning off steam first. To iron with steam, make sure the temperature is over 120 degrees");
                TurnOffSteam();
                return;
            }

            // Handle ironing with steam for Linen program
            if (program == "Linen")
            {
                if(!isSteamOn) UseSteam(); // Automatically turn on steam for Linen program

                Console.WriteLine($"{machineType} iron is ironing using the {program} program, with steam at {ironingTemp}°C");
                TurnOffSteam(); // Turn off steam after ironing
                checkSteamLight(steamUsageCount); // Check if steam light needs to be activated
                steamUsageCount++; // Increment steam usage count
            }
            else
            {
                string steamAppend = string.Empty;
                if (isSteamOn)
                {
                    steamAppend = "with steam";
                    steamUsageCount++; // Increment steam usage count
                }
                // Ironing with specified temperature for other programs
                Console.WriteLine($"{machineType} iron is ironing using the {program} program, at {ironingTemp}°C {steamAppend}");
                checkSteamLight(steamUsageCount); // Check if steam light needs to be activated
                TurnOffSteam();
            }
            
        }
        else
        {
            Console.WriteLine($"The temperature you entered ({ironingTemp}°C) is not within the supported range for the {machineType} iron");
        }
    }

    // Method to perform ironing based on a specific program name
    public void DoIroning(string program) {
        
        if (needsCleaning(usageCount)) return; // Check if the machine needs cleaning
        usageCount++; // Increment usage count

        // Check if the provided program is valid
        if (GlobalData.ironingMethods.ContainsKey(program))
        {
            (int rangeMin, int rangeMax) = GlobalData.ironingMethods[program]; // Get the temperature range for the program

            Random randomGenerator = new Random();
            int randomTemp = randomGenerator.Next(rangeMin, rangeMax + 1); // Generate a random temperature within the range

            // Validate if the machine can handle the program's maximum temperature
            if (rangeMax <= maxTemp)
            {
                // Check if steam is on and the random temperature is below 120
                if (randomTemp < 120 && isSteamOn)
                {
                    Console.WriteLine($"Attempted to iron at {randomTemp}°C. Not ironing. Turning off steam first. To iron with steam, make sure the temperature is over 120 degrees");
                    TurnOffSteam();
                    return;
                }

                // Handle ironing with steam for Linen program
                if (program == "Linen")
                {
                    if (!isSteamOn) UseSteam(); // Automatically turn on steam for Linen program


                    Console.WriteLine($"{machineType} iron is ironing at {randomTemp}°C with steam");
                    TurnOffSteam(); // Turn off steam after ironing
                    checkSteamLight(steamUsageCount); // Check if steam light needs to be activated
                    steamUsageCount++; // Increment steam usage count
                }
                else
                {
                    string steamAppend = string.Empty;
                    if (isSteamOn)
                    {
                        steamAppend = "with steam";
                        steamUsageCount++; // Increment steam usage count
                    }
                  
                    // Ironing with random temperature for other programs
                    Console.WriteLine($"{machineType} iron is ironing at {randomTemp}°C {steamAppend}");
                    checkSteamLight(steamUsageCount); // Check if steam light needs to be activated
                    TurnOffSteam();
                }
                
            }
            else
            {
                Console.WriteLine($"The {machineType} iron does not support the {program} ironing mode");
            }

        }
        else
        {
            Console.WriteLine($"The {machineType} iron does not support the {program} ironing mode");
        }
    }


    // Method to check if the machine needs cleaning
    protected virtual bool needsCleaning(int count)
    {
        if (count == 3)
        {
            Console.WriteLine("Machine has been used 3 times and needs cleaning");
            return true; // Indicates the machine needs cleaning
        }
        else
        {
            return false; // Indicates the machine does not need cleaning
        }
    }

    // Method to check if the steam light needs to be activated
    protected virtual void checkSteamLight(int count) {
        return; // Steam light feature doesn't exist for irons other than the premium one
    }

    // Helper method to get the ironing program based on a specific temperature
    protected string GetProgramByTemperature(int temperature)
    {
        foreach (var method in GlobalData.ironingMethods)
        {
            var (minTemp, maxTemp) = method.Value;
            // Check if the temperature falls within the program's range
            if (temperature >= minTemp && temperature <= maxTemp)
            {
                return method.Key; // Return the program name
            }
        }
        return "Invalid"; // Return "Invalid" if no matching program is found
    }

}


// Class representing a Regular Iron
public class RegularIron : BaseIron
{
    public RegularIron() : base("Regular", 199) { } // Sets the machine type and max temperature

}

// Class representing a Premium Iron
public class PremiumIron : BaseIron
{
    public PremiumIron() : base("Premium", 199) { } // Sets the machine type and max temperature

    // Override needsCleaning method to automatically clean after usage
    protected override bool needsCleaning(int count) {
        if (count == 3)
        {
            Console.WriteLine("Machine has been used 3 times and needs cleaning");
            Descale(); // Automatically clean the machine
            return false; // Prevent further ironing
        }
        else
        {
            return false; // Return false if no cleaning is needed
        }

    }

    // Override checkSteamLight method to not perform any action
    protected override void checkSteamLight(int count) {
        if (count == 2)
        {
            wateringLightIsOn = true;
            Console.WriteLine("You need to add water to the ironing machine."); // Notify the user that they need to add water
        }
    }

}

// Class representing a Linen Iron
public class LinenIron : BaseIron
{
    public LinenIron() : base("Linen", 230) { } // Sets the machine type and max temperature

}


// Example usage
class Program
{
    static void Main(string[] args)
    {
        // Create instances of different types of irons
        IroningMachine regularIron = new RegularIron();
        IroningMachine premiumIron = new PremiumIron();
        IroningMachine linenIron = new LinenIron();

        Console.WriteLine("\n=== Testing regular ===\n");
        // Test Regular Iron
        regularIron.TurnOn(); // Regular iron is turned on.
        regularIron.DoIroning(170); // Regular iron is ironing at 170°C
        regularIron.UseSteam(); // Steam is now on.
        regularIron.UseSteam(); // Steam is already on.
        regularIron.DoIroning("Cotton"); // Regular iron is ironing at 172°C
        regularIron.DoIroning("Linen"); // The Regular iron does not support the Linen ironing mode
        regularIron.Descale(); // Regular iron is cleaned.


        Console.WriteLine("\n=== Testing premium ===\n");
        // Test Premium Iron
        premiumIron.TurnOn(); // Premium iron is turned on.
        premiumIron.UseSteam(); // Steam is now on.
        premiumIron.DoIroning(150); // Premium iron is ironing using the Cotton program, at 150°C with steam
        premiumIron.UseSteam(); // Steam is now on.
        premiumIron.DoIroning(170); // Premium iron is ironing using the Cotton program, at 170°C with steam // You need to add water to the ironing machine.
        premiumIron.DoIroning(225); // The max temperature for Premium iron is 199°C, display message that denies ironing
        premiumIron.DoIroning("Silk"); // Machine has been used 3 times and needs cleaning // Premium iron is cleaned. // Premium iron is ironing at 136°C
        premiumIron.UseSteam(); // Steam is now on.
        premiumIron.DoIroning("Silk"); // Premium iron is ironing at 146°C
        premiumIron.UseSteam(); // Steam is now on.
        premiumIron.DoIroning("Synthetics"); // Should not iron, because steam is turned on
        premiumIron.DoIroning("Synthetics"); // Should iron, because steam was automatically turned off
        premiumIron.DoIroning("Linen"); // Should not iron
        premiumIron.UseSteam(); // Steam is now on.
        premiumIron.DoIroning(97); // Should not iron, because steam is turned on


        Console.WriteLine("\n=== Testing linen ===\n");
        // Test Linen Iron
        linenIron.TurnOn(); // Linen iron is turned on.
        linenIron.DoIroning(210); // Linen iron is ironing using the Linen program, with steam at 210°C
        linenIron.DoIroning("Silk"); // Linen iron is ironing at 148°C
        linenIron.UseSteam(); // Steam is now on.
        linenIron.DoIroning(130); // Linen iron is ironing using the Silk program, at 130°C with steam
        linenIron.DoIroning("Linen"); // Machine has been used 3 times and needs cleaning
        linenIron.Descale(); // Linen iron is cleaned.

        linenIron.TurnOff();
        premiumIron.TurnOff();
        regularIron.TurnOff();
    }
}
