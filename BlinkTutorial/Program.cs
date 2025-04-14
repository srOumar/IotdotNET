using System;
using System.Device.Gpio;
using System.Threading.Tasks;

const int PinButton = 24;
const int PinLed = 23;   

using var controller = new GpioController();


controller.OpenPin(PinButton, PinMode.InputPullDown); 
controller.OpenPin(PinLed, PinMode.Output);       


Console.WriteLine(
    $"({DateTime.Now}) État initial du bouton : {(controller.Read(PinButton) == PinValue.Low ? "Pressé" : "Relâché")}");

// Callback appelé à chaque changement d’état du bouton
controller.RegisterCallbackForPinValueChangedEvent(
    PinButton,
    PinEventTypes.Falling | PinEventTypes.Rising,
    OnButtonEvent);

await Task.Delay(Timeout.Infinite);

DateTime lastEventTime = DateTime.MinValue;
readonly TimeSpan debounceDelay = TimeSpan.FromMilliseconds(200);

void OnButtonEvent(object sender, PinValueChangedEventArgs args)
{
    var now = DateTime.Now;
    if ((now - lastEventTime) < debounceDelay)
    {
        return; // Ignore l'événement trop rapproché
    }
    lastEventTime = now;

    if (args.ChangeType == PinEventTypes.Falling)
    {
        controller.Write(PinLed, PinValue.High);
        Console.WriteLine($"({now}) LED allumée");
    }
    else if (args.ChangeType == PinEventTypes.Rising)
    {
        controller.Write(PinLed, PinValue.Low);
        Console.WriteLine($"({now}) LED éteinte");
    }
}
