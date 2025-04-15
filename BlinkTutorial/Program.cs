using System;
using System.Device.Gpio;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

const int PinButton = 24;
const int PinLed = 23;
const string ApiUrl = "http://172.16.144.19:5242/api/chatapi/send";

using var controller = new GpioController();
using var httpClient = new HttpClient();


controller.OpenPin(PinButton, PinMode.Input); 
controller.OpenPin(PinLed, PinMode.Output);       


Console.WriteLine(
    $"({DateTime.Now}) État initial du bouton : {(controller.Read(PinButton) == PinValue.Low ? "Pressé" : "Relâché")}");

// Callback appelé à chaque changement d’état du bouton
controller.RegisterCallbackForPinValueChangedEvent(
    PinButton,
    PinEventTypes.Falling | PinEventTypes.Rising,
    OnButtonEvent);

await Task.Delay(Timeout.Infinite);

async void OnButtonEvent(object sender, PinValueChangedEventArgs args)
{
    if (args.ChangeType == PinEventTypes.Falling)
    {
        controller.Write(PinLed, PinValue.High);
        Console.WriteLine($"({DateTime.Now}) LED allumée");

        var payload = new
        {
            user = "raspberry",
            message = "Bouton pressé !"
        };

        try
        {
            var response = await httpClient.PostAsJsonAsync(ApiUrl, payload);
            response.EnsureSuccessStatusCode();
            Console.WriteLine($"({DateTime.Now}) Message envoyé dans le chat !");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur en envoyant le message : {ex.Message}");
        }
    }
    else if (args.ChangeType == PinEventTypes.Rising)
    {
        controller.Write(PinLed, PinValue.Low);
        Console.WriteLine($"({DateTime.Now}) LED éteinte");
    }
}
