using UnityEngine;
using UnityEngine.UI;
using figure;
using net;

namespace game {
    public class UIController : MonoBehaviour {
        public GameController gameController;

        public Canvas MainMenuCanvas;
        public Canvas HostErrorCanvas;
        public Canvas ConnectCanvas;
        public Canvas ConnectErrorCanvas;
        public Canvas DisconnectCanvas;
        public Canvas WaitingCanvas;
        public Canvas PawnCanvas;
        public Canvas EndCanvas;
        public Canvas GameCanvas;

        public Text winText;
        public Text ipText;

        private const string DRAW_TEXT = "Draw";
        private const string BLACK_WIN_TEXT = "Black Win";
        private const string WHITE_WIN_TEXT = "White Win";

        private void Update() {
            switch (gameController.gameState) {
                case GameState.Stop:
                    SwitchCanvas(MainMenuCanvas);
                    break;

                case GameState.HostError:
                    SwitchCanvas(HostErrorCanvas);
                    break;

                case GameState.Connecting:
                    SwitchCanvas(ConnectCanvas);
                    break;

                case GameState.ConnectError:
                    SwitchCanvas(ConnectErrorCanvas);
                    break;

                case GameState.Disconnect:
                    SwitchCanvas(DisconnectCanvas);
                    break;

                case GameState.Waiting:
                    SwitchCanvas(WaitingCanvas);
                    break;

                case GameState.Pause:
                    SwitchCanvas(PawnCanvas);
                    break;

                case GameState.Running:
                    SwitchCanvas(GameCanvas);
                    break;

                case GameState.Draw:
                    SwitchCanvas(EndCanvas);

                    winText.text = DRAW_TEXT;
                    break;

                case GameState.Win:
                    SwitchCanvas(EndCanvas);

                    if (gameController.isWhiteMove) {
                        winText.text = BLACK_WIN_TEXT;
                    } else {
                        winText.text = WHITE_WIN_TEXT;
                    }
                    break;
            }
        }

        private void SwitchCanvas(Canvas canvasToEnable) {
            MainMenuCanvas.enabled = false;
            WaitingCanvas.enabled = false;
            PawnCanvas.enabled = false;
            EndCanvas.enabled = false;
            GameCanvas.enabled = false;
            ConnectCanvas.enabled = false;
            DisconnectCanvas.enabled = false;
            ConnectErrorCanvas.enabled = false;
            HostErrorCanvas.enabled = false;

            canvasToEnable.enabled = true;
        }

        public void HostButton() {
            gameController.Host();
		}

		public void ConnectMenuButton() {
            gameController.gameState = GameState.Connecting;
        }

        public void HotseatButton() {
            gameController.Hotseat();
        }

        public void CreateBishopButton() {
            gameController.TransformPawn(FigureType.Bishop);
        }

        public void CreateQueenButton() {
            gameController.TransformPawn(FigureType.Queen);
        }

        public void CreateRookButton() {
            gameController.TransformPawn(FigureType.Rook);
        }

        public void CreateKnightButton() {
            gameController.TransformPawn(FigureType.Knight);
        }

        public void BackFromConnecting() {
            gameController.gameState = GameState.Stop;
        }

        public void ConnectButton() {
            string ip = ipText.text;
            if (ip == "") {
                ip = Server.STANDART_IP;
            }
            gameController.Connect(ip);
        }

        public void MainMenuButton() {
            gameController.ClearGame();
            gameController.gameState = GameState.Stop;
        }

        public void QuitButton() {
            Application.Quit();
        }
    }
}