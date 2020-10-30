/*
MIT License

Copyright (c) 2020 gpsnmeajp

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace VMCProtocolReflector
{
    class Process
    {
        Receiver receiver;
        List<Sender> senders = new List<Sender>();

        const string title = "VMCProtocolReflector v0.01";
        const string filename = "setting.txt";

        public void Run() {
            Int64 count = 0;

            Setup();

            //リフレクター処理
            Task task = Task.Run(async () => {
                while (true)
                {
                    UdpReceiveResult res = await receiver.ReceiveAsync();
                    foreach (Sender s in senders) {
                        s.Send(res.Buffer);
                    }
                    count = unchecked(count + res.Buffer.Length); //オーバーフローを無視
                }
            });


            //ステータス表示
            while (!Console.KeyAvailable)
            {
                Console.Clear();

                Console.WriteLine(title);

                Console.WriteLine("Receiver Port : " + receiver.GetPort());
                foreach (Sender s in senders)
                {
                    Console.WriteLine("Sender : "+s.GetHost() + " : "+ s.GetPort());
                }

                Console.WriteLine("Packets : " + count);
                Thread.Sleep(500);
            }

            Teardown();
        }

        void Setup()
        {
            string[] settings = File.ReadAllLines(filename);

            receiver = new Receiver(int.Parse(settings[0]));

            for (int i = 1; i < settings.Count(); i++) {
                string[] host = settings[i].Split(':');
                if (host.Length == 2) {
                    senders.Add(new Sender(host[0], int.Parse(host[1])));
                }
            }
        }

        void Teardown()
        {
            receiver.Dispose();
            foreach (Sender s in senders)
            {
                s.Dispose();
            }
        }
    }
}
