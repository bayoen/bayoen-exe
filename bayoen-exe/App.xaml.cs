﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace bayoen
{    
    public partial class App : Application
    {              
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Core.Initialize();
        }
    }
}
