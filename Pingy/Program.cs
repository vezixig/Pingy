// See https://aka.ms/new-console-template for more information

using System.Net.NetworkInformation;


// send aping teams.microsoft.com 

var pingA = new Ping();
var pingB = new Ping();
var pingC = new Ping();
var payload = new byte[1024];
var total = 0.0;
List<double> failed = [0.0, 0.0, 0.0];

while (true)
{
    try
    {
        new Random().NextBytes(payload);
        total++;

        // Create tasks to send pings to both targets in parallel
        var pingTask1 = pingA.SendPingAsync("teams.microsoft.com", 1000, payload);
        var pingTask2 = pingB.SendPingAsync("google.de", 1000, payload);
        var pingTask3 = pingC.SendPingAsync("192.168.0.1", 1000, payload);

        // Wait for both tasks to complete
        var results = await Task.WhenAll(pingTask1, pingTask2, pingTask3);

        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($"{DateTime.Now:HH:mm:ss} ");

        for (var index = 0; index < results.Length; index++)
        {
            var result = results[index];
            // Check if the ping was successful
            if (result.Status == IPStatus.Success)
            {
                // Print the round-trip time in milliseconds
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"| {failed[index] / total * 100:f2}% {result.RoundtripTime,4}ms ");

                if (result.RoundtripTime < 50)
                    Console.ForegroundColor = ConsoleColor.Green;
                else if (result.RoundtripTime < 100)
                    Console.ForegroundColor = ConsoleColor.Yellow;
                else
                    Console.ForegroundColor = ConsoleColor.DarkRed;

                var latency = "";
                for (var i = 0; i <= result.RoundtripTime / 5; i++) latency += "\u2593";
                Console.Write($"{latency}".PadRight(25, ' '));
            }
            else
            {
                failed[index]++;
                Console.ForegroundColor = ConsoleColor.Red;
                // Print the status if ping fails
                Console.Write(
                    $"| {failed[index] / total * 100:f2}%       XXXXXXXXXXXXXXXXXXXXXXXXXX");
            }
        }

        Console.WriteLine();
    }
    catch (Exception ex)
    {
        // Print any exceptions that occur
        Console.WriteLine($"Error pinging : {ex.Message}");
    }

    // Wait for 1 second before sending the next ping
    Thread.Sleep(1000);
}