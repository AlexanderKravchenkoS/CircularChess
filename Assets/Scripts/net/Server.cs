using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using game;

namespace net {
    public class Server : MonoBehaviour {
        private List<TcpClient> clients;

        private TcpListener listener;
        private bool isServerProcesing;

        public const int PORT = 8888;
        public const string STANDART_IP = "127.0.0.1";

        private void Update() {
            if (!isServerProcesing) {
                return;
            }

            foreach (var client in clients) {
                NetworkStream s = client.GetStream();
                if (s.DataAvailable) {
                    StreamReader reader = new StreamReader(s, true);
                    string data = reader.ReadLine();

                    if (data != null)
                        SendData(data, client);
                }
            }
        }

        public void Init() {
            clients = new List<TcpClient>();

            try {
                listener = new TcpListener(IPAddress.Any, PORT);
                listener.Start();

                StartListening();
                isServerProcesing = false;

            } catch (Exception ex) {
                Debug.LogError($"Socket error: {ex.Message}");
                return;
            }
        }

        private void StartListening() {
            listener.BeginAcceptTcpClient(AcceptTcpClient, listener);
        }

        private void AcceptTcpClient(IAsyncResult ar) {
            TcpListener listener = (TcpListener)ar.AsyncState;

            var tcpClient = listener.EndAcceptTcpClient(ar);
            clients.Add(tcpClient);
            Debug.Log("Client connected");

            if (clients.Count != 2) {
                StartListening();
                return;
            }

            isServerProcesing = true;

            SendData("START");
        }

        private void SendData(string data, TcpClient client = null) {
            TcpClient clientForSend = new TcpClient();

            if (client != null) {
                foreach (var cl in clients) {
                    if (cl != client) {
                        clientForSend = cl;
                    }
                }
            }

            string[] aData = data.Split('|');

            switch (aData[0]) {
                case "MOVE":
                    try {
                        StreamWriter writer = new StreamWriter(clientForSend.GetStream());
                        writer.WriteLine(data);
                        writer.Flush();
                    } catch (Exception e) {
                        Debug.Log("Write error : " + e.Message);
                    }
                    break;

                case "START":
                    foreach (var item in clients) {
                        try {
                            StreamWriter writer = new StreamWriter(item.GetStream());
                            writer.WriteLine("START");
                            writer.Flush();
                        } catch (Exception e) {
                            Debug.Log("Write error : " + e.Message);
                        }
                    }
                    break;
            }
        }

        private void OnDestroy() {
            listener.Stop();
        }
    }
}