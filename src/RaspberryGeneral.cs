using System;
using System.Management.Automation;
using Unosquare.RaspberryIO;
using Unosquare.WiringPi;

namespace Pepitenet.Powershell.IoT
{
    public static class RaspberryGeneral
    {
        public static bool isGpioInitialized { get; set; } = false;
    }

    [Cmdlet(VerbsCommon.Set, "PiModule")]
    public class SetPiModule : Cmdlet
    {
        protected override void ProcessRecord()
        {
            try
            {
                Pi.Init<BootstrapWiringPi>();
                GpioInformation.getPiMode();
            }
            catch (Exception e)
            {
                WriteObject("Error initializing PI Module : " + e.Message);
            }
            finally
            {
                RaspberryGeneral.isGpioInitialized = true;
            }
        }
    }

    [Cmdlet(VerbsCommon.Get, "PiInformation")]
    public class GetPiInformation : Cmdlet
    {
        protected override void ProcessRecord()
        {
            WriteObject(Pi.Info);
        }
    }
}
