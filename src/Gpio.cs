using System.Management.Automation;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;

namespace Pepitenet.Powershell.IoT
{
    public class GpioPinData
    {
        public int PhysicalPin { get; set; }
        public int WiringPiPin { get; set; }
        public BcmPin BcmPinInfo { get; set; }
        public P1 P1Pin { get; set; }
        public string Name { get; set; }
        public GpioPinValue SignalLevel { get; set; }
        public GpioPinResistorPullMode InputPullMode { get; set; }
        public GpioPinDriveMode PinMode { get; set; }


        public GpioPinData(int PhysicalPin, int WiringPiPin, BcmPin BcmPinInfo, P1 P1Pin, string Name)
        {
            this.PhysicalPin = PhysicalPin;
            this.BcmPinInfo = BcmPinInfo;
            this.WiringPiPin = WiringPiPin;
            this.Name = Name;
            this.P1Pin = P1Pin;
        }
        public GpioPinData(int PhysicalPin, string Name)
        {
            this.PhysicalPin = PhysicalPin;
            this.BcmPinInfo = BcmPin.Gpio00;
            this.WiringPiPin = 255;
            this.Name = Name;
            this.P1Pin = P1.Pin40;
        }
    }

    [Cmdlet(VerbsCommon.Get, "PiMode")]
    public class Test : Cmdlet
    {
        protected override void ProcessRecord()
        {
            GpioInformation.getPiMode();
        }
    }

    [Cmdlet(VerbsCommon.Get, "AllGpioPins")]
    public class GetAllGpioPins : Cmdlet
    {
        [Parameter(Mandatory = false)]
        public SwitchParameter ReadAll { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                foreach (GpioPinData pinData in GpioInformation.BcmCorrespondance)
                {
                    if (pinData.WiringPiPin != 255)
                    {
                        var pin = Pi.Gpio[pinData.BcmPinInfo];
                        pinData.InputPullMode = pin.InputPullMode;
                        if (pin.Value)
                        {
                            pinData.SignalLevel = GpioPinValue.High;
                        }
                        else
                        {
                            pinData.SignalLevel = GpioPinValue.Low;
                        }
                    }
                }
                GpioInformation.getPiMode();
                if (this.ReadAll)
                {
                    WriteObject(GpioInformation.formatGpioList(GpioInformation.BcmCorrespondance));
                }
                else
                {
                    WriteObject(GpioInformation.BcmCorrespondance);
                }


            }
            catch // Unosquare.RaspberryIO.Gpio.GpioController.Initialize throws this TypeInitializationException
            {
                throw;
            }
        }
    }

    [Cmdlet(VerbsCommon.Get, "GpioPin")]
    public class GetGpioPin : Cmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public int Id { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter BcmPin { get; set; }

        protected override void ProcessRecord()
        {
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
                    pinData.InputPullMode = pin.InputPullMode;
                    if (pin.Value)
                    {
                        pinData.SignalLevel = GpioPinValue.High;
                    }
                    else
                    {
                        pinData.SignalLevel = GpioPinValue.Low;
                    }
                    pinData.PinMode = pin.PinMode;

                    WriteObject(pinData);
                }

            }
            catch // Unosquare.RaspberryIO.Gpio.GpioController.Initialize throws this TypeInitializationException
            {
                throw;
            }
        }
    }

    [Cmdlet(VerbsCommon.Set, "GpioPinMode")]
    public class SetGpioPinMode : Cmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public int Id { get; set; }

        [Parameter(Mandatory = true, Position = 1)]
        public GpioPinDriveMode Mode { get; set; }

        [Parameter(Mandatory = false, Position = 2)]
        public SwitchParameter BcmPin { get; set; }

        protected override void ProcessRecord()
        {
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
                    pin.PinMode = Mode;
                }

            }
            catch // Unosquare.RaspberryIO.Gpio.GpioController.Initialize throws this TypeInitializationException
            {
                throw;
            }
        }
    }

    [Cmdlet(VerbsCommon.Set, "GpioPinValue")]
    public class SetGpioPinValue : Cmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public int Id { get; set; }

        [Parameter(Mandatory = true, Position = 1)]
        public GpioPinValue Value { get; set; }

        [Parameter(Mandatory = false, Position = 2)]
        public SwitchParameter BcmPin { get; set; }

        protected override void ProcessRecord()
        {
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
                    pin.Write(Value);
                }

            }
            catch // Unosquare.RaspberryIO.Gpio.GpioController.Initialize throws this TypeInitializationException
            {
                throw;
            }
        }
    }


}
