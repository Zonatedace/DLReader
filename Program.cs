using System;
using System.Text.RegularExpressions;

public class DriversLicense
{
    public string LicenseNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public string State { get; set; }
}

public class LicenseParser
{
    public static DriversLicense Parse(string rawData)
    {
        var license = new DriversLicense();

        // Identify the state from the raw data to apply state-specific rules
        if (rawData.Contains("DAJMI"))  // Michigan
        {
            license.State = "MI";
            license.LastName = ExtractElement(rawData, "DCS", new[] { "DCT", "DBD" });
            license.FirstName = ExtractElement(rawData, "DCT", new[] { "DBD" });
            license.MiddleName = ExtractElement(rawData, "DAD", new[] { "DBD" });
            license.LicenseNumber = ExtractElement(rawData, "DAQ", new[] { "DCF" });
        }
        else if (rawData.Contains("DAJLA"))  // Louisiana
        {
            license.State = "LA";
            license.LastName = ExtractElement(rawData, "DCS", new[] { "DFU", "DEU", "DDG", "DCS" });
            license.FirstName = ExtractElement(rawData, "DAC", new[] { "DFU", "DEU", "DDG", "DCS" });
            license.MiddleName = ExtractElement(rawData, "DAD", new[] { "DFU", "DEU", "DDG", "DCS" });
            license.LicenseNumber = ExtractElement(rawData, "DAQ", new[] { "DFU", "DEU", "DDG", "DCS" });
        }
        else
        {
            // Default parsing, if specific state identifiers aren't found
            license.LastName = ExtractElement(rawData, "DCS", new[] { "DAU", "DAY", "DAJ", "DAK" });
            license.FirstName = ExtractElement(rawData, "DAC", new[] { "DAU", "DAY", "DAJ", "DAK" });
            license.MiddleName = ExtractElement(rawData, "DAD", new[] { "DAU", "DAY", "DAJ", "DAK" });
            license.LicenseNumber = ExtractElement(rawData, "DAQ", new[] { "DAU", "DAY", "DAJ", "DAK" });
        }

        return license;
    }

    private static string ExtractElement(string data, string elementId, string[] stopWords)
    {
        string stopPattern = string.Join("|", stopWords);
        var match = Regex.Match(data, $@"{elementId}([A-Z0-9\s\-]+?)(?=(?:{stopPattern})|$)");
        return match.Success ? match.Groups[1].Value.Trim() : string.Empty;
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Please scan or enter the raw driver's license data:");

        string scannedData = "";
        while (true)
        {
            string input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
                break;
            scannedData += input + "\n";
        }

        DriversLicense license = LicenseParser.Parse(scannedData.ToUpper());

        Console.WriteLine("\nExtracted Driver's License Information:");
        Console.WriteLine("State: " + license.State);
        Console.WriteLine("First Name: " + license.FirstName);
        Console.WriteLine("Last Name: " + license.LastName);
        Console.WriteLine("Middle Name: " + license.MiddleName);
        Console.WriteLine("License Number: " + license.LicenseNumber);

        Console.WriteLine("\nScanning complete. Press any key to exit...");
        Console.ReadKey();
    }
}
