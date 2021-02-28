using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Common.Entities.Sensors;
using Common.Providers;

namespace Common
{
    public class AnomaliesDetectorContainer : WindsorContainer
    {
        public static AnomaliesDetectorContainer Instance { get; set; } = new AnomaliesDetectorContainer();

        public AnomaliesDetectorContainer()
        {
            this.Install(new IWindsorInstaller[]
            {
                new FormatProviderInstaller(),
                new SensorsInstaller()
            });
        }
    }
}
