using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System;
using game;

namespace net {
    public class Client : MonoBehaviour {
        public GameController gameController;

        private bool isSocketReady;
        private TcpClient socket;
        private NetworkStream stream;
        private StreamWriter writer;
        private StreamReader reader;

        private void Update() {
            if (!isSocketReady) {
                return;
            }

            if (!stream.DataAvailable) {
                return;
            }

            string data = reader.ReadLine();
            if (data == null) {
                return;
            }

            OnIncomingData(data);
        }

        public bool ConnectToServer(string host, int port) {
            if (isSocketReady) {
                return false;
            }

            try {
                socket = new TcpClient(host, port);
                stream = socket.GetStream();
                writer = new StreamWriter(stream);
                reader = new StreamReader(stream);

                isSocketReady = true;

            } catch (Exception e) {
                Debug.LogError("Socket error " + e.Message);

            }

            return isSocketReady;
        }

        public void Send(string data) {
            if (!isSocketReady) {
                return;
            }

            writer.WriteLine(data);
            writer.Flush();
        }

        private void OnIncomingData(string data) {
            string[] aData = data.Split('|');

            switch (aData[0]) {
                case "MOVE":
                    gameController.MakeTurn
                        (int.Parse(aData[1]),
                        int.Parse(aData[2]),
                        int.Parse(aData[3]),
                        int.Parse(aData[4]),
                        int.Parse(aData[5]),
                        int.Parse(aData[6]));

                    break;
            }
        }

        private void OnDestroy() {
            if (!isSocketReady) {
                return;
            }

            writer.Close();
            reader.Close();
            socket.Close();
            isSocketReady = false;
        }
    }
}
