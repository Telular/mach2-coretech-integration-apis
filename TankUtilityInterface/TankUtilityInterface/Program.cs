using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace TankUtilityInterface
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Use Mutex to stop multiple versions of application from running 
            bool bMutexWasCreated;
            Mutex m = new Mutex(true, "Global\\AzureInterface", out bMutexWasCreated);

            // If can't create mutex then application is already running so warn user and close
            if (!bMutexWasCreated)
            {
                MessageBox.Show("Error! Multiple Starts of AzureInterface.exe Closing application", "AzureInterface");
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmTankUtilityInterface());

            // Release the Mutex
            m.ReleaseMutex();
        }
    }
}
