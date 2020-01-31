using System;
using System.Collections.Generic;
using System.Text;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using System.Management.Automation;

namespace Pepitenet.Powershell.IoT
{
    static class MorseCode
    {
        public static Dictionary<char, string> Database { get; } = new Dictionary<char, string>
        {
            {'a', string.Concat('.', '-')},
            {'b', string.Concat('-', '.', '.', '.')},
            {'c', string.Concat('-', '.', '-', '.')},
            {'d', string.Concat('-', '.', '.')},
            {'e', '.'.ToString()},
            {'f', string.Concat('.', '.', '-', '.')},
            {'g', string.Concat('-', '-', '.')},
            {'h', string.Concat('.', '.', '.', '.')},
            {'i', string.Concat('.', '.')},
            {'j', string.Concat('.', '-', '-', '-')},
            {'k', string.Concat('-', '.', '-')},
            {'l', string.Concat('.', '-', '.', '.')},
            {'m', string.Concat('-', '-')},
            {'n', string.Concat('-', '.')},
            {'o', string.Concat('-', '-', '-')},
            {'p', string.Concat('.', '-', '-', '.')},
            {'q', string.Concat('-', '-', '.', '-')},
            {'r', string.Concat('.', '-', '.')},
            {'s', string.Concat('.', '.', '.')},
            {'t', string.Concat('-')},
            {'u', string.Concat('.', '.', '-')},
            {'v', string.Concat('.', '.', '.', '-')},
            {'w', string.Concat('.', '-', '-')},
            {'x', string.Concat('-', '.', '.', '-')},
            {'y', string.Concat('-', '.', '-', '-')},
            {'z', string.Concat('-', '-', '.', '.')},
            {'0', string.Concat('-', '-', '-', '-', '-')},
            {'1', string.Concat('.', '-', '-', '-', '-')},
            {'2', string.Concat('.', '.', '-', '-', '-')},
            {'3', string.Concat('.', '.', '.', '-', '-')},
            {'4', string.Concat('.', '.', '.', '.', '-')},
            {'5', string.Concat('.', '.', '.', '.', '.')},
            {'6', string.Concat('-', '.', '.', '.', '.')},
            {'7', string.Concat('-', '-', '.', '.', '.')},
            {'8', string.Concat('-', '-', '-', '.', '.')},
            {'9', string.Concat('-', '-', '-', '-', '.')},
            { '.', string.Concat('.', '-','.', '-','.', '-') },
            { ',', string.Concat( '-', '-', '.', '.', '-', '-') },
            { '?', string.Concat( '.', '.', '-', '-', '.', '.') },
            { '\'', string.Concat( '.', '-', '-', '-', '-', '.') },
            { '!', string.Concat( '-', '.', '-', '.', '-', '-') },
            { '/', string.Concat( '-', '.', '.', '-', '.') },
            { '(', string.Concat( '-', '.', '-', '-', '.') },
            { ')', string.Concat( '-', '.', '-', '-', '.', '-') },
            { '&', string.Concat( '.', '-', '.', '.', '.') },
            { ':', string.Concat( '-', '-', '-', '.', '.', '.') },
            { ';', string.Concat( '-', '.', '-', '.', '-', '.') },
            { '=', string.Concat( '-', '.', '.', '.', '-') },
            { '+', string.Concat( '.', '-', '.', '-', '.') },
            { '-', string.Concat( '-', '.', '.', '.', '.', '-') },
            { '_', string.Concat( '.', '.', '-', '-', '.', '-') },
            { '"', string.Concat( '.', '-', '.', '.', '-', '.') },
            { '$', string.Concat( '.', '.', '.', '-', '.', '.', '-') },
            { '@', string.Concat( '.', '-', '-', '.', '-', '.') },
            {' ', ' '.ToString()}
        };

        public static Dictionary<char, List<GpioPinValue>> Conversion { get; } = new Dictionary<char, List<GpioPinValue>>
        {
            { '.', new List<GpioPinValue>() { GpioPinValue.High, GpioPinValue.Low } },
            { '-', new List<GpioPinValue>() { GpioPinValue.High, GpioPinValue.High, GpioPinValue.High, GpioPinValue.Low } },
            { 'l', new List<GpioPinValue>() { GpioPinValue.Low, GpioPinValue.Low } },
            { ' ', new List<GpioPinValue>() { GpioPinValue.Low, GpioPinValue.Low } }
        };

        public static string ConvertStringToStringMorse(string phrase)
        {
            string stringInDot = "";

            char[] charArr = phrase.ToLower().ToCharArray();
            foreach (char ch in charArr)
            {
                stringInDot += Database[ch] + 'l';
            }

            return stringInDot;
        }


        public static List<GpioPinValue> ConvertStringToGpioPinValue(string phrase)
        {
            List<GpioPinValue> morseCode = new List<GpioPinValue>();

            char[] charArr = phrase.ToCharArray();
            foreach (char ch in charArr)
            {
                morseCode.AddRange(Conversion[ch]);
            }

            return morseCode;
        }
    }

    [Cmdlet(VerbsCommon.Set, "MorseCode")]
    public class SetMorseCode : Cmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public int Id { get; set; }

        [Parameter(Mandatory = true, Position = 1)]
        public string Message { get; set; }

        [Parameter(Mandatory = true, Position = 2)]
        public int DotTimeValue { get; set; }

        [Parameter(Mandatory = false, Position = 3)]
        public SwitchParameter BcmPin { get; set; }

        protected override void ProcessRecord()
        {
            string decodedMessage = MorseCode.ConvertStringToStringMorse(Message);
            List<GpioPinValue> morseCode = MorseCode.ConvertStringToGpioPinValue(decodedMessage);

            try
            {
                GpioPinData pinData = null;
                int pinId = 0;
                foreach (GpioPinData tempPin in GpioInformation.BcmCorrespondance)
                {
                    if (BcmPin)
                        pinId = (int)tempPin.BcmPinInfo;
                    else
                        pinId = tempPin.WiringPiPin;
                    if (pinId == Id)
                    {
                        pinData = tempPin;
                    }
                }
                if (pinData != null)
                {
                    var pin = Pi.Gpio[pinData.BcmPinInfo];
                    for (var i = 0; i < morseCode.Count; i++)
                    {
                        pin.Write(morseCode[i]);
                        System.Threading.Thread.Sleep(DotTimeValue);
                    }
                }

            }
            catch // Unosquare.RaspberryIO.Gpio.GpioController.Initialize throws this TypeInitializationException
            {
                throw;
            }
        }
    }

}
