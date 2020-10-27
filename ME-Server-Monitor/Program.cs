using System;
using System.ServiceProcess;
using System.Threading;
using DiscordWebhookLib;

namespace MEServerMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceController service = new ServiceController();
            service.ServiceName = "ME-Server-1"; // Insert service name to be monitored
            int n = 0, b = 0;
            bool msgSent = false;

            // logging

            string token = "https://discord.com/api/webhooks/704997459776241664/qcCxo-NhNaghVaX64sT_8rdt-dC_2vRL2mNwjZC6kjDkuZxd_pAS7nzM8xqTHq7Lj1aY"; // Insert Discord webhook URL 
            string username = "Server Status"; // Insert username that posts messages
            string avatar = "https://cdn.discordapp.com/attachments/704997359372730419/770637256074592266/logo.png"; // Insert URL for avatar image

            Console.WriteLine("-------------------------------------");
            Console.WriteLine("| Medieval Engineers Server Monitor |");
            Console.WriteLine("-------------------------------------\n");

            DateTime restart1 = DateTime.Today.AddHours(3); // Set time for scheduled restart1
            DateTime restart2 = DateTime.Today.AddHours(14).AddMinutes(4); // Set time for scheduled restart2
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

                // Initial message
                if (service.Status == ServiceControllerStatus.Running && msgSent == false)
                {
                    discord.Execute(now +  " - Server is running.", username, avatar, false);
                    msgSent = true;
                }

                // Initial start when service is stopped
                if (service.Status == ServiceControllerStatus.Stopped && msgSent == false)
                {
                    discord.Execute(now + " - Server stopped.", username, avatar, false);
                    Thread.Sleep(10000);
                    now = DateTime.Now;
                    Console.WriteLine("Starting service {0}...", service.ServiceName);
                    discord.Execute(now + " - Starting server...", username, avatar, false);
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running);
                    service.Refresh();
                    now = DateTime.Now;
                    Console.WriteLine("Service {0} started successfully.\n", service.ServiceName);
                    discord.Execute(now + " - Server started successfully.", username, avatar, false);
                }

                // Scheduled restarts
                // T-15 min
                if (n != 15 && now >= restart1.AddMinutes(-15) && now <= restart1.AddMinutes(-14) || n != 15 && now >= restart2.AddMinutes(-15) && now <= restart2.AddMinutes(-14) || n != 15 && now >= restart3.AddMinutes(-15) && now <= restart3.AddMinutes(-14) || n != 15 && now >= restart4.AddMinutes(-15) && now <= restart4.AddMinutes(-14))
                {
                    Console.WriteLine("!!! Server restart in 15 minutes !!!");
                    discord.Execute(now + " - Scheduled server restart in 15 minutes.", username, avatar, false);
                    n = 15;
                }

                // T-10 min
                if (n != 10 && now >= restart1.AddMinutes(-10) && now <= restart1.AddMinutes(-9) || n != 10 && now >= restart2.AddMinutes(-10) && now <= restart2.AddMinutes(-9) || n != 10 && now >= restart3.AddMinutes(-10) && now <= restart3.AddMinutes(-9) || n != 10 && now >= restart4.AddMinutes(-10) && now <= restart4.AddMinutes(-9))
                {
                    Console.WriteLine("!!! Server restart in 10 minutes !!!");
                    discord.Execute(now + " - Scheduled server restart in 10 minutes.", username, avatar, false);
                    n = 10;
                }

                // T-5min
                if (n != 5 && now >= restart1.AddMinutes(-5) && now <= restart1.AddMinutes(-4) || n != 5 && now >= restart2.AddMinutes(-5) && now <= restart2.AddMinutes(-4) || n != 5 && now >= restart3.AddMinutes(-5) && now <= restart3.AddMinutes(-4) || n != 5 && now >= restart4.AddMinutes(-5) && now <= restart4.AddMinutes(-4))
                {
                    Console.WriteLine("!!! Server restart in 5 minutes !!!");
                    discord.Execute(now + " - Scheduled server restart in 5 minutes.", username, avatar, false);
                    n = 5;
                }

                // T-0.5 min
                if (now >= restart1.AddMinutes(-0.5) && now <= restart1.AddMinutes(-0.25) || now >= restart2.AddMinutes(-0.5) && now <= restart2.AddMinutes(-0.25) || now >= restart3.AddMinutes(-0.5) && now <= restart3.AddMinutes(-0.25) || now >= restart4.AddMinutes(-0.5) && now <= restart4.AddMinutes(-0.25))
                {
                    Console.WriteLine("!!! Server restart imminent !!!");
                    discord.Execute(now + " - Scheduled server restart imminent.", username, avatar, false);

                    // Timer for precise shutdown
                    while (now > restart1.AddMinutes(-1) && now < restart1 || now > restart2.AddMinutes(-1) && now < restart2 || now > restart3.AddMinutes(-1) && now < restart3 || now > restart4.AddMinutes(-1) && now < restart4)
                    {
                        now = DateTime.Now;
                        Thread.Sleep(1000);
                    }

                    // Stop
                    Console.WriteLine("Stopping service {0}...", service.ServiceName);
                    discord.Execute(now + " - Stopping server...", username, avatar, false);
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped);

                    now = DateTime.Now;

                    Console.WriteLine("Service {0} is {1}.", service.ServiceName, service.Status.ToString());
                    discord.Execute(now + " - Server stopped successfully.", username, avatar, false);

                    Thread.Sleep(10000);
                    now = DateTime.Now;

                    // Start
                    Console.WriteLine("Starting service {0}...\n", service.ServiceName);
                    discord.Execute(now + " - Starting server...", username, avatar, false);
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running);
                    msgSent = false; // Reset bool, for healthcheck message
                }

                // Restart after crash
                if (service.Status == ServiceControllerStatus.Stopped && msgSent == true)
                {
                    discord.Execute(now + " - <@&381388788536049664> Server stopped unexpectedly!", username, avatar, false); // Mention @role
                    Thread.Sleep(10000);
                    now = DateTime.Now;
                    Console.WriteLine("Starting service {0}...", service.ServiceName);
                    discord.Execute(now + " - Starting server...", username, avatar, false);
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running);
                    service.Refresh();
                    now = DateTime.Now;
                    Console.WriteLine("Service {0} started successfully.\n", service.ServiceName);
                    discord.Execute(now + " - Server started successfully.", username, avatar, false);
                    msgSent = false;
                }

                Thread.Sleep(10000);
            }
        }
    }
}
