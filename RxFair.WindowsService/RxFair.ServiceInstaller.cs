using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace RxFair.WindowsService
{
    [RunInstaller(true)]
    public partial class RxFairServiceInstaller : System.Configuration.Install.Installer
    {
        public RxFairServiceInstaller()
        {
            InitializeComponent();
        }
    }
}
