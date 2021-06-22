using net;
using UnityEngine;

namespace game {
    public class ComponentController : MonoBehaviour {
        public GameController gameController;
        public Selecter selecter;



        private GameState lastGameState;

        private void Update() {
            if (lastGameState == gameController.gameState) {
                return;
            }

            lastGameState = gameController.gameState;

            switch (lastGameState) {
                case GameState.Running:
                    selecter.enabled = true;
                    break;
                default:
                    selecter.enabled = false;
                    break;
            }
        }
    }
}
