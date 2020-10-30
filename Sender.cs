﻿/*
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


namespace VMCProtocolReflector
{
    class Sender : IDisposable
    {
        UdpClient udpClient;
        string host;
        int port;

        public Sender(string host,int port) {
            udpClient = new UdpClient();
            this.host = host;
            this.port = port;
        }

        public string GetHost()
        {
            return host;
        }
        public int GetPort()
        {
            return port;
        }

        ~Sender()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (udpClient != null)
            {
                udpClient.Close();
                udpClient = null;
            }
        }

        public void Send(byte[] data) {
            udpClient.Send(data, data.Length, host, port);
        }
    }
}
