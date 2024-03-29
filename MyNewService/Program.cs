﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MyNewService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
#if (!DEBUG)
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new MyNewService(args)
            };
            ServiceBase.Run(ServicesToRun);
#else
            MyNewService myServ = new MyNewService();
            myServ.StartVboxVM();
            myServ.FindOva();
            myServ.StopVboxVm();
#endif
        }
    }
}
