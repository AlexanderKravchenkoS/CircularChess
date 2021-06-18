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
    }
}
