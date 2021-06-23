using UnityEngine;
using cell;

namespace game {
    public class Selecter : MonoBehaviour {
        public GameController gameController;

        private Cell selectedCell;

        private void Update() {

            if (gameController.gameState != GameState.Running) {
                return;
            }

            if (Input.GetMouseButtonDown(0)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (!Physics.Raycast(ray, out hit)) {
                    return;
                }

                var cell = hit.transform.gameObject.GetComponent<Cell>();

                if (cell == null) {
                    return;
                }

                if (cell.figure != null) {
                    if (cell.figure.figureData.isWhite == gameController.isWhiteMove
                        && gameController.isWhitePlayer == gameController.isWhiteMove) {

                        selectedCell = cell;
                        gameController.RemoveHighlight();
                        gameController.HighlightAllMoves(selectedCell);
                        return;
                    }
                }

                if (selectedCell == null) {
                    return;
                }

                gameController.ProcessSelect(selectedCell, cell);

                selectedCell = null;

                gameController.RemoveHighlight();
            }
        }
    }
}
