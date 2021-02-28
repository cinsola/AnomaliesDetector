using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Common.MapReduce;
using System.Linq;

namespace Common.Entities.Sensors
{
    public class SensorsInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var sensors = new ISensor[]
            {
                new HumiditySensor(),
                new MonoxideSensor(),
                new ThermometerSensor()
            }.ToDictionary(k => k.AsDevice);
            container.Register(Component.For<IRankingReducerFactory>().Instance(new RankingReducerFactory(sensors)).LifeStyle.Singleton);
            container.Register(Component.For<IRankingManager>().ImplementedBy<BaseRankingManager>());
        }
    }
}
