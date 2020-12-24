using System.ServiceProcess;

namespace RxFair.WindowsService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var servicesToRun = new ServiceBase[]
            {
                new RxFairService()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
