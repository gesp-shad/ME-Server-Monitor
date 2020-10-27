using System;
using System.ServiceProcess;
using System.Threading;
using DiscordWebhookLib;

namespace ME_ServiceController
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceController service = new ServiceController();
            service.ServiceName = "[Service Name]"; // Insert service name to be monitored
            int n = 0;

            string token = "[webhook URL]"; // Insert Discord webhook URL 
            string username = "[username]"; // Insert username that posts messages
            string avatar = "[avatar URL]"; // Insert URL for avatar image
            string status;

            Console.WriteLine("--------------------------------------------");
            Console.WriteLine("| Medieval Engineers Server Restart Script |");
            Console.WriteLine("--------------------------------------------\n");

            DateTime restart1 = DateTime.Today.AddHours(3); // Set time for scheduled restart1
            DateTime restart2 = DateTime.Today.AddHours(9); // Set time for scheduled restart2
            DateTime restart3 = DateTime.Today.AddHours(15); // Set time for scheduled restart3
            DateTime restart4 = DateTime.Today.AddHours(20); // Set time for scheduled restart4

            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("| Server Restart #1 at {0:HH:mm:ss zzz}. |", restart1);
            Console.WriteLine("| Server Restart #2 at {0:HH:mm:ss zzz}. |", restart2);
            Console.WriteLine("| Server Restart #3 at {0:HH:mm:ss zzz}. |", restart3);
            Console.WriteLine("| Server Restart #4 at {0:HH:mm:ss zzz}. |", restart4);
            Console.WriteLine("-----------------------------------------\n");

            DiscordWebhookExecutor discord = new DiscordWebhookExecutor(token);

            while (true)
            {
                DateTime now = DateTime.Now;
                Console.WriteLine("System Time: {0:HH:mm:ss}.", now);

                // Health check
                service.Refresh();
                Console.WriteLine("Service {0} is {1}.\n", service.ServiceName, service.Status.ToString());

                if (service.Status == ServiceControllerStatus.Running)
                {
                    status = "running";
                }

                else
                {
                    status = "stopped";
                }

                discord.Execute("Server is " + status + ".", username, avatar, false);

                if (n != 15 && now >= restart1.AddMinutes(-15) && now <= restart1.AddMinutes(-14) || n != 15 && now >= restart2.AddMinutes(-15) && now <= restart2.AddMinutes(-14) || n != 15 && now >= restart3.AddMinutes(-15) && now <= restart3.AddMinutes(-14) || n != 15 && now >= restart4.AddMinutes(-15) && now <= restart4.AddMinutes(-14))
                {
                    Console.WriteLine("!!! Server restart in 15 minutes !!!");
                    discord.Execute("Scheduled server restart in 15 minutes.", username, avatar, false);
                    n = 15;
                }

                if (n != 10 && now >= restart1.AddMinutes(-10) && now <= restart1.AddMinutes(-9) || n != 10 && now >= restart2.AddMinutes(-10) && now <= restart2.AddMinutes(-9) || n != 10 && now >= restart3.AddMinutes(-10) && now <= restart3.AddMinutes(-9) || n != 10 && now >= restart4.AddMinutes(-10) && now <= restart4.AddMinutes(-9))
                {
                    Console.WriteLine("!!! Server restart in 10 minutes !!!");
                    discord.Execute("Scheduled server restart in 10 minutes.", username, avatar, false);
                    n = 10;
                }

                if (n != 5 && now >= restart1.AddMinutes(-5) && now <= restart1.AddMinutes(-4) || n != 5 && now >= restart2.AddMinutes(-5) && now <= restart2.AddMinutes(-4) || n != 5 && now >= restart3.AddMinutes(-5) && now <= restart3.AddMinutes(-4) || n != 5 && now >= restart4.AddMinutes(-5) && now <= restart4.AddMinutes(-4))
                {
                    Console.WriteLine("!!! Server restart in 5 minutes !!!");
                    discord.Execute("Scheduled server restart in 5 minutes.", username, avatar, false);
                    n = 5;
                }

                if (now >= restart1.AddMinutes(-0.5) && now <= restart1.AddMinutes(-0.25) || now >= restart2.AddMinutes(-0.5) && now <= restart2.AddMinutes(-0.25) || now >= restart3.AddMinutes(-0.5) && now <= restart3.AddMinutes(-0.25) || now >= restart4.AddMinutes(-0.5) && now <= restart4.AddMinutes(-0.25))
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Console.WriteLine("!!! Server restart imminent !!!");
                        discord.Execute("Scheduled server restart imminent.", username, avatar, false);
                        Thread.Sleep(10000);
                    }

                    Console.WriteLine("Stopping service {0}...", service.ServiceName);
                    discord.Execute("Stopping server...", username, avatar, false);
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped);
                    Console.WriteLine("Service {0} is {1}.", service.ServiceName, service.Status.ToString());

                    Thread.Sleep(10000);

                    Console.WriteLine("Starting service {0}...\n", service.ServiceName);
                    discord.Execute("Starting server...", username, avatar, false);
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running);
                }

                // Restart after crash
                if (service.Status == ServiceControllerStatus.Stopped)
                {
                    Console.WriteLine("Starting service {0}...", service.ServiceName);
                    discord.Execute("Server crashed!", username, avatar, false);
                    discord.Execute("Starting server...", username, avatar, false);
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running);
                    service.Refresh();
                    Console.WriteLine("Service {0} started successfully.\n", service.ServiceName);
                    discord.Execute("Server started successfully.", username, avatar, false);
                }

                Thread.Sleep(10000);
            }
        }
    }
}
