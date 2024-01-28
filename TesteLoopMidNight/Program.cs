using System.Diagnostics;
using Topshelf;

namespace TesteLoopMidNight
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            HostFactory.Run(configurator =>
            {
                configurator.Service<Process>(s =>
                {
                    s.ConstructUsing(name => new Process());
                    s.WhenStarted((service, control) => service.Start(control));
                    s.WhenStopped((service, control) => service.Stop(control));
                });
                configurator.RunAsLocalSystem();

                configurator.SetDescription("App to Clean Directorys");
                configurator.SetDisplayName("CleanFiles");
                configurator.SetServiceName("CleanFiles");
            });
        }

    }
}
