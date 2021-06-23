using UnityEngine;
using UnityEngine.UI;
using figure;

namespace game {
    public class UIController : MonoBehaviour {
        public GameController gameController;

        public Canvas MainMenuCanvas;
        public Canvas ConnectCanvas;
        public Canvas WaitingCanvas;
        public Canvas PawnCanvas;
        public Canvas EndCanvas;
        public Canvas GameCanvas;

        public Text winText;
        public Text ipText;

        private void Update() {
            switch (gameController.gameState) {
                case GameState.Stop:
                    MainMenuCanvas.enabled = true;
                    WaitingCanvas.enabled = false;
                    PawnCanvas.enabled = false;
                    EndCanvas.enabled = false;
                    GameCanvas.enabled = false;
                    ConnectCanvas.enabled = false;
                    break;

                case GameState.Connecting:
                    MainMenuCanvas.enabled = false;
                    WaitingCanvas.enabled = false;
                    PawnCanvas.enabled = false;
                    EndCanvas.enabled = false;
                    GameCanvas.enabled = false;
                    ConnectCanvas.enabled = true;
                    break;

                case GameState.Waiting:
                    MainMenuCanvas.enabled = false;
                    WaitingCanvas.enabled = true;
                    PawnCanvas.enabled = false;
                    EndCanvas.enabled = false;
                    GameCanvas.enabled = false;
                    ConnectCanvas.enabled = false;
                    break;

                case GameState.Pause:
                    MainMenuCanvas.enabled = false;
                    WaitingCanvas.enabled = false;
                    PawnCanvas.enabled = true;
                    EndCanvas.enabled = false;
                    GameCanvas.enabled = false;
                    ConnectCanvas.enabled = false;
                    break;

                case GameState.Running:
                    MainMenuCanvas.enabled = false;
                    WaitingCanvas.enabled = false;
                    PawnCanvas.enabled = false;
                    EndCanvas.enabled = false;
                    GameCanvas.enabled = true;
                    ConnectCanvas.enabled = false;
                    break;

                case GameState.Draw:
                    MainMenuCanvas.enabled = false;
                    WaitingCanvas.enabled = false;
                    PawnCanvas.enabled = false;
                    EndCanvas.enabled = true;
                    GameCanvas.enabled = false;
                    ConnectCanvas.enabled = false;

                    winText.text = "Draw";
                    break;

                case GameState.Win:
                    MainMenuCanvas.enabled = false;
                    WaitingCanvas.enabled = false;
                    PawnCanvas.enabled = false;
                    EndCanvas.enabled = true;
                    GameCanvas.enabled = false;
                    ConnectCanvas.enabled = false;

                    if (gameController.isWhiteMove) {
                        winText.text = "Black Win";
                    } else {
                        winText.text = "White Win";
                    }
                    break;
            }
        }

        public void HostButton() {
            gameController.Host();
		}

		public void ConnectButton() {
            gameController.gameState = GameState.Connecting;
        }

        public void StartHotseat() {
            gameController.Hotseat();
        }

        public void CreateBishop() {
            gameController.TransformPawn(FigureType.Bishop);
        }

        public void CreateQueen() {
            gameController.TransformPawn(FigureType.Queen);
        }

        public void CreateRook() {
            gameController.TransformPawn(FigureType.Rook);
        }

        public void CreateKnight() {
            gameController.TransformPawn(FigureType.Knight);
        }

        public void BackFromConnecting() {
            gameController.gameState = GameState.Stop;
        }

        public void Connect() {
            var ip = ipText.text;
            if (ip == "") {
                ip = "127.0.0.1";
            }
            gameController.Connect(ip);
        }
    }
}
