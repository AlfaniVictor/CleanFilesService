using System;

namespace TesteLoopMidNight.Object
{
    public class jsonObject
    {
        public string FolderToClean { get; set; }
        public int HourToClean { get; set; }
        public int timeMinToProcess = 1;
        public int TimeMinToProcess
        {
            get => timeMinToProcess;
            set => value *= 60000;
        }
    }
}
