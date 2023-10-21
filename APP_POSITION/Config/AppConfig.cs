using System;
using System.Collections.Generic;

namespace APP_POSITION.Config
{
    [Serializable]
    internal class AppConfig
    {
        public Application Application { get; set; }
        public List<ProcessesList> Processes { get; set; }
    }
}
