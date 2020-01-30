using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Unosquare.RaspberryIO.Abstractions;

namespace Pepitenet.Powershell.IoT
{
    static class GpioInformation
    {
        public static List<GpioPinData> BcmCorrespondance { get; set; } = new List<GpioPinData>
        {
            new GpioPinData(1, "3.3v"),
            new GpioPinData(2, "5v"),
            new GpioPinData(3, 8, BcmPin.Gpio02, P1.Pin03, "SDA.1"),
            new GpioPinData(4, "5v"),
            new GpioPinData(5, 9, BcmPin.Gpio03, P1.Pin05, "SCL.1"),
            new GpioPinData(6, "Ground"),
            new GpioPinData(7, 7, BcmPin.Gpio04, P1.Pin07, "GPIO.7"),
            new GpioPinData(8, 15, BcmPin.Gpio14, P1.Pin08, "TxD"),
            new GpioPinData(9, "Ground"),
            new GpioPinData(10, 16, BcmPin.Gpio15, P1.Pin10, "RxD"),
            new GpioPinData(11, 0, BcmPin.Gpio17, P1.Pin11, "GPIO.0"),
            new GpioPinData(12, 1, BcmPin.Gpio18, P1.Pin12, "GPIO.1"),
            new GpioPinData(13, 2, BcmPin.Gpio27, P1.Pin13, "GPIO.2"),
            new GpioPinData(14, "Ground"),
            new GpioPinData(15, 3, BcmPin.Gpio22, P1.Pin15, "GPIO.3"),
            new GpioPinData(16, 4, BcmPin.Gpio23, P1.Pin16, "GPIO.4"),
            new GpioPinData(17, "3.3v"),
            new GpioPinData(18, 5, BcmPin.Gpio24, P1.Pin18, "GPIO.5"),
            new GpioPinData(19, 12, BcmPin.Gpio10, P1.Pin19, "MOSI"),
            new GpioPinData(20, "Ground"),
            new GpioPinData(21, 13, BcmPin.Gpio09, P1.Pin21, "MISO"),
            new GpioPinData(22, 6, BcmPin.Gpio25, P1.Pin22, "GPIO.6"),
            new GpioPinData(23, 14, BcmPin.Gpio11, P1.Pin23, "SCLK"),
            new GpioPinData(24, 10, BcmPin.Gpio08, P1.Pin24, "CE0"),
            new GpioPinData(25, "Ground"),
            new GpioPinData(26, 11, BcmPin.Gpio07, P1.Pin21, "CE1"),
            new GpioPinData(27, 30, BcmPin.Gpio00, P1.Pin27, "SDA.0"),
            new GpioPinData(28, 31, BcmPin.Gpio01, P1.Pin28, "SCL.0"),
            new GpioPinData(29, 21, BcmPin.Gpio05, P1.Pin29, "GPIO.21"),
            new GpioPinData(30, "Ground"),
            new GpioPinData(31, 22, BcmPin.Gpio06, P1.Pin31, "GPIO.22"),
            new GpioPinData(32, 26, BcmPin.Gpio12, P1.Pin32, "GPIO.26"),
            new GpioPinData(33, 23, BcmPin.Gpio13, P1.Pin33, "GPIO.23"),
            new GpioPinData(34, "Ground"),
            new GpioPinData(35, 24, BcmPin.Gpio19, P1.Pin35, "GPIO.24"),
            new GpioPinData(36, 27, BcmPin.Gpio16, P1.Pin36, "GPIO.27"),
            new GpioPinData(37, 25, BcmPin.Gpio26, P1.Pin37, "GPIO.25"),
            new GpioPinData(38, 28, BcmPin.Gpio20, P1.Pin38, "GPIO.28"),
            new GpioPinData(39, "Ground"),
            new GpioPinData(40, 29, BcmPin.Gpio21, P1.Pin40, "GPIO.29")
        };

        public static string GetValue(int group, string test)
        {
            var pattern = @"^[|] +(\d+) [|] ([\w]+).* [|] .*[|].*[|] +(\d+) [|] ([\w]+).* [|].*[|]";
            var result = default(string);
            var match = System.Text.RegularExpressions.Regex.Match(test, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                result = match.Groups[group].Value;
            }
            return result;
        }

        public static GpioPinDriveMode ConvertMode(string mode)
        {
            switch (mode)
            {
                case "IN":
                    return GpioPinDriveMode.Input;
                case "OUT":
                    return GpioPinDriveMode.Output;
                case "ALT0":
                    return GpioPinDriveMode.Alt0;
                case "ALT1":
                    return GpioPinDriveMode.Alt1;
                case "ALT2":
                    return GpioPinDriveMode.Alt2;
                case "ALT3":
                    return GpioPinDriveMode.Alt3;
                case "PWM":
                    return GpioPinDriveMode.PwmOutput;
                case "CLCK":
                    return GpioPinDriveMode.GpioClock;
                default:
                    return GpioPinDriveMode.Input;
            }
        }

        public static void getPiMode()
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = @"/usr/bin/gpio";
            cmd.StartInfo.Arguments = "allreadall";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            // result of gpio allreadall
            string data;
            do
            {
                data = cmd.StandardOutput.ReadLine();
                string sPinNumber1 = "";
                int pinNumber1 = 0;
                string pinMode1 = "";
                string sPinNumber2 = "";
                int pinNumber2 = 0;
                string pinMode2 = "";
                if (!String.IsNullOrEmpty(data))
                {
                    sPinNumber1 = GpioInformation.GetValue(1, data);
                    pinMode1 = GpioInformation.GetValue(2, data);
                    sPinNumber2 = GpioInformation.GetValue(3, data);
                    pinMode2 = GpioInformation.GetValue(4, data);
                }
                if (!String.IsNullOrEmpty(sPinNumber1))
                {
                    pinNumber1 = Int32.Parse(sPinNumber1);
                    pinNumber2 = Int32.Parse(sPinNumber2);

                    foreach (GpioPinData tempPin in GpioInformation.BcmCorrespondance)
                    {
                        if (pinNumber1 == (int)tempPin.BcmPinInfo)
                        {
                            tempPin.PinMode = GpioInformation.ConvertMode(pinMode1);
                        }
                        if (pinNumber2 == tempPin.PhysicalPin)
                        {
                            tempPin.PinMode = GpioInformation.ConvertMode(pinMode2);
                        }
                    }

                }
            }
            while (!String.IsNullOrEmpty(data));

        }

        public static string[] formatGpioList(List<GpioPinData> data)
        {
            string[] result = new string[26];
            string bcm1, mode1, value1, wpi1 = "";
            string bcm2, mode2, value2, wpi2 = "";
            // PinMode = 9 char max
            // PinValue = 4 char max
            // Name = 10 char
            result[0] = "+-----+-----+------------+-----------+-------+----------+-------+-----------+------------+-----+-----+";
            result[1] = "| BCM | wPi |    Name    |    Mode   | Value | Physical | Value |    Mode   |    Name    | wPi | BCM |";
            result[2] = "+-----+-----+------------+-----------+-------+----------+-------+-----------+------------+-----+-----+";
            for (int i = 0; i < 20; i++)
            {
                if ((int)data[2 * i].WiringPiPin == 255)
                {
                    bcm1 = mode1 = value1 = wpi1 = "";
                }
                else
                {
                    bcm1 = ((int)data[2 * i].BcmPinInfo).ToString();
                    mode1 = data[2 * i].PinMode.ToString();
                    value1 = data[2 * i].SignalLevel.ToString();
                    wpi1 = data[2 * i].WiringPiPin.ToString();
                }
                if ((int)data[2 * i + 1].WiringPiPin == 255)
                {
                    bcm2 = mode2 = value2 = wpi2 = "";
                }
                else
                {
                    bcm2 = ((int)data[2 * i + 1].BcmPinInfo).ToString();
                    mode2 = data[2 * i + 1].PinMode.ToString();
                    value2 = data[2 * i + 1].SignalLevel.ToString();
                    wpi2 = data[2 * i + 1].WiringPiPin.ToString();
                }
                result[3 + i] = "| " + bcm1.PadLeft(3, ' ') + " | " +
                    wpi1.PadLeft(3, ' ') + " | " +
                    data[2 * i].Name.PadLeft(10, ' ') + " | " +
                    mode1.PadLeft(9, ' ') + " | " +
                    value1.PadLeft(5, ' ') + " | " +
                    data[2 * i].PhysicalPin.ToString().PadLeft(2, ' ') +
                    " || " +
                    data[2 * i + 1].PhysicalPin.ToString().PadLeft(2, ' ') + " | " +
                    value2.PadLeft(5, ' ') + " | " +
                    mode2.PadLeft(9, ' ') + " | " +
                    data[2 * i + 1].Name.PadLeft(10, ' ') + " | " +
                    wpi2.PadLeft(3, ' ') + " | " +
                    bcm2.PadLeft(3, ' ') + " |";
            }

            result[23] = "+-----+-----+------------+-----------+-------+----------+-------+-----------+------------+-----+-----+";
            result[24] = "| BCM | wPi |    Name    |    Mode   | Value | Physical | Value |    Mode   |    Name    | wPi | BCM |";
            result[25] = "+-----+-----+------------+-----------+-------+----------+-------+-----------+------------+-----+-----+";

            return result;
        }
    }
}
