using System;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace autoInstall
{
    class Program
    {
        Process process = new Process();                            // create a global Process
        int elapsedTime;                                            // this variable holds the elapsed time of every process
        bool eventHandled;                                          // this variable depends upon the process status
        StreamReader file = new StreamReader("input.txt");          // this is the input file that holds the working directory and commands

        public static void Main(string[] args)
        {
            Program prog = new Program();                           // creating a Program
            prog.executeCommands();
        }

        public void executeCommands()
        {
            try
            {
                processSetup();                                                     // setup the process below

                string workingDirectory = file.ReadLine();                          // read the working directory from the file's first line
                process.StartInfo.WorkingDirectory = workingDirectory;

                string commands = file.ReadLine();                                  // read all the commands
                while (!file.EndOfStream)
                    commands = commands + " & " + file.ReadLine();

                commands = commands + " & " + "exit";
                process.StartInfo.Arguments = "/K " + commands;

                Console.WriteLine("File has been successfully read, now executing your commands\nRunning");

                process.Start();                                                    // starting the process
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred trying to execute commands" + "\n" + ex.Message);
                return;
            }

            const int SLEEP_AMOUNT = 100;
            while (!eventHandled)                               // runs until the process has not completed
            {
                elapsedTime += SLEEP_AMOUNT;
                if (elapsedTime > 30000)
                {
                    break;
                }
                Thread.Sleep(SLEEP_AMOUNT);                     // makes the thread stop for the elapsed time
            }
        }

        public void processSetup()
        {
            process.StartInfo.FileName = "cmd.exe";                         // the app name
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;      // hide the app window
            process.EnableRaisingEvents = true;                             // this triggers the event exited function when the process has exited
            process.Exited += new EventHandler(processExited);              // adding a new event handler to the exited process
        }

        private void processExited(object sender, System.EventArgs e)
        {
            eventHandled = true;
            Console.WriteLine("Exit time:    {0}\r\n" +
                "Exit code:    {1}\r\nElapsed time: {2}", process.ExitTime, process.ExitCode, elapsedTime);
        }

    }
}
