using net;
using UnityEngine;

namespace game {
    public class UIController : MonoBehaviour {
        public GameController gameController;

        private GameState lastGameState;

        private void Update() {
            if (gameController.gameState == lastGameState) {
                return;
            }

            lastGameState = gameController.gameState;

            switch (lastGameState) {
                case GameState.Stop:
                    break;
                case GameState.Pause:
                    break;
                case GameState.Running:
                    break;
                case GameState.Draw:
                    Debug.Log("Draw");
                    break;
                case GameState.End:
                    if (gameController.isWhiteMove) {
                        Debug.Log("Black Win");
                    } else {
                        Debug.Log("White Win");
                    }
                    break;
            }
        }

		public void HostServerButton() {
			string hostAddress = "127.0.0.1";
			try {
				Server s = Instantiate(gameController.serverPrefab);
				s.Init();

				Client c = Instantiate(gameController.clientPrefab);
				c.ConnectToServer(hostAddress, 8888);

                c.gameController = gameController;
                gameController.client = c;
                gameController.isWhitePlayer = true;
			} catch (System.Exception e) {
				Debug.Log(e.Message);
			}
		}
		public void ConnectToServerButton() {
	        string hostAddress = "127.0.0.1";

            try {
                Client c = Instantiate(gameController.clientPrefab);
                c.ConnectToServer(hostAddress, 8888);

                c.gameController = gameController;
                gameController.client = c;
                gameController.isWhitePlayer = false;
            } catch (System.Exception e) {
                Debug.Log(e.Message);
            }
		}
	}
}
