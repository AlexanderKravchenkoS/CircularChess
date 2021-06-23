using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace net {
    public class Server : MonoBehaviour {
        public const int PORT = 8888;

        private List<TcpClient> clients;

        private TcpListener listener;
        private bool isServerProcesing;

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
                        OnIncomingData(client, data);
                }
            }
        }

        public void Init() {
            clients = new List<TcpClient>();

            try {
                listener = new TcpListener(IPAddress.Loopback, PORT);
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
            Debug.Log("Client conencted");

            if (clients.Count != 2) {
                StartListening();
            }

            isServerProcesing = true;
        }

        private void OnIncomingData(TcpClient client, string data) {
            TcpClient clientForSend = new TcpClient();

            foreach (var cl in clients) {
                if (cl != client) {
                    clientForSend = cl;
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
            }
        }
    }
}