using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Common.MapReduce;

namespace Common.Providers
{
    public class FormatProviderInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ILogRowProvider>().Instance(new DefaultLogRowProvider()));
            container.Register(Component.For<ETLBaseMapReduce>().ImplementedBy<SynchronizedMapReduce>());
        }
    }
}
