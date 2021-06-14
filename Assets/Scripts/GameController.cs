using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour{
    public GameObject board;

    public bool isBlackMove;

    private Cell[,] cells;

    private const int CIRCLES_COUNT = 8;
    private const int CELLS_IN_CIRCLE = 8;

    private void Start() {
        cells = new Cell[CIRCLES_COUNT, CELLS_IN_CIRCLE];
        for (int i = 0; i < CIRCLES_COUNT; i++) {

            var circle = board.transform.GetChild(i);

            for (int j = 0; j < CELLS_IN_CIRCLE; j++) {

                var cell = circle.transform.GetChild(j).GetComponent<Cell>();

                if (cell.figure != null) {
                    var position = cell.point.transform.position;
                    position.y = cell.figure.transform.position.y;
                    cell.figure.transform.position = position;
                    cell.figure.GetComponent<Figure>().startX = cell.x;
                    cell.figure.GetComponent<Figure>().startY = cell.y;
                }

                cells[cell.x, cell.y] = cell;
            }
        }
    }

    public void ProcessTurn(Cell originalCell, Cell newCell) {



        MoveFigure(originalCell.figure.GetComponent<Figure>(), newCell.point.transform.position);

        if (newCell.figure != null) {
            Destroy(newCell.figure);
        }

        newCell.figure = originalCell.figure;

        originalCell.figure = null;

        isBlackMove = !isBlackMove;
    }

    private void MoveFigure(Figure figure, Vector3 newPosition) {
        newPosition.y = figure.transform.position.y;
        figure.transform.position = newPosition;
        figure.isFirstTurn = false;
    }

    private bool IsCorrectMove(Figure figure, int startX, int startY, int endX, int endY) {

        switch (figure.figureType) {
            case FigureType.King:
                break;
            case FigureType.Queen:
                break;
            case FigureType.Bishop:
                break;
            case FigureType.Knight:
                break;
            case FigureType.Rook:
                break;
            case FigureType.Pawn:
                break;
        }

        return false;
    }
}
