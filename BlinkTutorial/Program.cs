using System;
using System.Device.Gpio;
using System.Threading.Tasks;

const int PinButton = 24;
const int PinLed = 23;   

using var controller = new GpioController();


controller.OpenPin(PinButton, PinMode.InputPullUp); 
controller.OpenPin(PinLed, PinMode.Output);       


Console.WriteLine(
    $"({DateTime.Now}) État initial du bouton : {(controller.Read(PinButton) == PinValue.Low ? "Pressé" : "Relâché")}");

// Callback appelé à chaque changement d’état du bouton
controller.RegisterCallbackForPinValueChangedEvent(
    PinButton,
    PinEventTypes.Falling | PinEventTypes.Rising,
    OnButtonEvent);

await Task.Delay(Timeout.Infinite);

static void OnButtonEvent(object sender, PinValueChangedEventArgs args)
{
    var controller = (GpioController)sender;

    if (args.ChangeType == PinEventTypes.Falling)
    {
        controller.Write(PinLed, PinValue.High);
        Console.WriteLine($"({DateTime.Now}) LED allumée");
    }
    else if (args.ChangeType == PinEventTypes.Rising)
    {
        controller.Write(PinLed, PinValue.Low);
        Console.WriteLine($"({DateTime.Now}) LED éteinte");
    }
}
