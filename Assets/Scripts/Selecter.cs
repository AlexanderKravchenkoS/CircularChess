using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selecter : MonoBehaviour{
    public GameController gameController;

    private Cell selectedCell;

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (!Physics.Raycast(ray, out hit)) {
                return;
            }

            var cell = hit.transform.gameObject.GetComponent<Cell>();

            if (cell.figure != null) {
                if (cell.figure.GetComponent<Figure>().isBlack == gameController.isBlackMove) {
                    selectedCell = cell;
                    return;
                }
            }

            if (selectedCell == null) {
                return;
            }

            gameController.ProcessTurn(selectedCell, cell);

            selectedCell = null;
        }
    }
}
