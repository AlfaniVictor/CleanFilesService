using System;
using System.Linq;
using TesteLoopMidNight.Object;
using System.Text.Json;
using System.Reflection;
using System.IO;
using Topshelf;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace TesteLoopMidNight
{
    public class Process : ServiceControl
    {
        private enum LOOP
        {
            PROCESS = 1,
            VERIFY_TIME = 2,
            CLEAN_DIRECTORY = 3
        }

        private LOOP cycle = LOOP.PROCESS;
        private static jsonObject obj = null;
        private static bool alreadyProcessed = false;
        private static DateTime NextDay;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private static Stopwatch stopwatch = new Stopwatch();
        private static int IntervaloSegundos = 1;

        public Process()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public bool Start(HostControl hostControl)
        {
            Task.Run(() =>
            {
                stopwatch.Start();
                while (true)
                {
                    if (VerificarTempoPassado())
                    {
                        ProcessLogic();
                        stopwatch.Restart();
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }
            }, _cancellationTokenSource.Token);

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            return true;
        }

        public void ProcessLogic()
        {
            if (!alreadyProcessed)
            {
                switch (cycle)
                {
                    case LOOP.PROCESS:
                        {
                            string strJson = GetAndReadJsonFile();
                            obj = JsonSerializer.Deserialize<jsonObject>(strJson);
                            cycle = LOOP.VERIFY_TIME;
                            break;
                        }
                    case LOOP.VERIFY_TIME:
                        {
                            if (DateTime.Now.Hour == obj.HourToClean)
                                cycle = LOOP.CLEAN_DIRECTORY;
                            else
                                IntervaloSegundos = obj.TimeMinToProcess;

                            break;
                        }
                    case LOOP.CLEAN_DIRECTORY:
                        {
                            IntervaloSegundos = obj.TimeMinToProcess;
                            string[] files = Directory.GetFiles(obj.FolderToClean);
                            files.ToList().ForEach(file => File.Delete(file));
                            alreadyProcessed = true;
                            NextDay = DateTime.Now.AddDays(1);
                            break;
                        }
                }
            }
            else
            {
                if (DateTime.Now == NextDay)
                    alreadyProcessed = false;
            }
        }

        private string GetAndReadJsonFile()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string path = System.IO.Path.GetDirectoryName(asm.Location);
            string[] contentFile = File.ReadAllLines(path + "\\JSONConfigs.json");
            string json = string.Empty;
            json = string.Join(json, contentFile);

            return json;
        }

        private static bool VerificarTempoPassado()
        {
            return stopwatch.Elapsed.TotalSeconds >= IntervaloSegundos;
        }

        
    }
}
