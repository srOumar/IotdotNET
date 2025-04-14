// See https://aka.ms/new-console-template for more information
/*
Console.WriteLine("Hello, World!");
//ceci etait le code par défaut
*/

using System;
using System.Device.Gpio;
using System.Threading;

Console.WriteLine("Blinking LED. Press Ctrl+C to end.");
int pin = 23; //pensez à changer en fonction du pin utilisé dans ton circuit
using var controller = new GpioController();
controller.OpenPin(pin, PinMode.Output);
bool ledOn = true;
while (true)
{
    controller.Write(pin, ((ledOn) ? PinValue.High : PinValue.Low));
    Thread.Sleep(1000);
    ledOn = !ledOn;
}