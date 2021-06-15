using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour{
    public GameObject board;

    public bool isWhiteMove;

    private Cell[,] cells;

    private const int BOARD_SIZE = 8;

    private void Start() {
        cells = new Cell[BOARD_SIZE, BOARD_SIZE];

        for (int i = 0; i < BOARD_SIZE; i++) {

            var circle = board.transform.GetChild(i);

            for (int j = 0; j < BOARD_SIZE; j++) {

                var cell = circle.transform.GetChild(j).GetComponent<Cell>();

                if (cell.figure != null) {
                    var position = cell.point.transform.position;
                    position.y = cell.figure.transform.position.y;
                    cell.figure.transform.position = position;
                    cell.figure.startX = cell.x;
                    cell.figure.startY = cell.y;
                }

                cells[cell.x, cell.y] = cell;
            }
        }

        isWhiteMove = true;
    }

    public void ProcessTurn(Cell originalCell, Cell newCell) {

        if(!IsCorrectMove(originalCell, newCell)) {
            return;
        }

        if (IsFigureOnWay(cells, originalCell, newCell)) {
            return;
        }

        MoveFigure(originalCell.figure, newCell.point.transform.position);

        if (newCell.figure != null) {
            Destroy(newCell.figure.gameObject);
        }

        newCell.figure = originalCell.figure;

        originalCell.figure = null;

        isWhiteMove = !isWhiteMove;
    }

    private void MoveFigure(Figure figure, Vector3 newPosition) {
        newPosition.y = figure.transform.position.y;
        figure.transform.position = newPosition;
        figure.isFirstTurn = false;
    }

    private bool IsCorrectMove(Cell startCell, Cell endCell) {
        Figure figure = startCell.figure;

        bool isCorrect = false;

        int deltaY;
        int deltaYR;
        int deltaX;

        if (startCell.x < 4 && endCell.x >= 4) {
            deltaX = (endCell.x - 4) - startCell.x;
            deltaY = (BOARD_SIZE - endCell.y) + (BOARD_SIZE - startCell.y) - 1;
            deltaYR = startCell.y + endCell.y + 1;
        } else if (startCell.x >= 4 && endCell.x < 4) {
            deltaX = endCell.x - (startCell.x - 4);
            deltaY = (BOARD_SIZE - endCell.y) + (BOARD_SIZE - startCell.y) - 1;
            deltaYR = startCell.y + endCell.y + 1;
        } else {
            deltaX = endCell.x - startCell.x;
            deltaY = deltaYR = endCell.y - startCell.y;
        }

        int deltaXabs = Mathf.Abs(deltaX);
        int deltaYabs = Mathf.Abs(deltaY);
        int deltaYRabs = Mathf.Abs(deltaYR);

        switch (figure.figureType) {

            case FigureType.King:

                if (deltaXabs == 1 && (deltaY == 0 || deltaYR == 0)) {
                    isCorrect = true;
                    break;
                }

                if (deltaX == 0 && (deltaYabs == 1 || deltaYRabs == 1)) {
                    isCorrect = true;
                    break;
                }

                if (deltaXabs == 1 && (deltaYabs == 1 || deltaYRabs == 1)) {
                    isCorrect = true;
                    break;
                }

                break;

            case FigureType.Queen:

                if (deltaXabs == deltaYabs || deltaXabs == deltaYRabs) {
                    isCorrect = true;
                    break;
                }

                if (deltaXabs == 0 || deltaYabs == 0 || deltaYRabs == 0) {
                    isCorrect = true;
                }

                break;

            case FigureType.Bishop:

                if (deltaXabs == deltaYabs || deltaXabs == deltaYRabs) {
                    isCorrect = true;
                }

                break;

            case FigureType.Knight:

                if (deltaXabs == 2 && (deltaYabs == 1 || deltaYRabs == 1)) {
                    isCorrect = true;
                    break;
                }

                if (deltaXabs == 1 && (deltaYabs == 2 || deltaYRabs == 2)) {
                    isCorrect = true;
                }

                break;

            case FigureType.Rook:

                if (deltaXabs == 0 || deltaYabs == 0 || deltaYRabs == 0) {
                    isCorrect = true;
                }

                break;

            case FigureType.Pawn:

                if (figure.isWhite) {
                    deltaX = endCell.x - startCell.x;
                    deltaY = endCell.y - startCell.y;
                } else {
                    deltaY = startCell.y - endCell.y;
                    deltaX = startCell.x - endCell.x;
                }

                if (endCell.figure == null) {
                    if (deltaY == 2 && figure.isFirstTurn && deltaX == 0) {
                        isCorrect = true;
                        break;
                    }

                    if (deltaX == 0 && deltaY == 1) {
                        isCorrect = true;
                    }

                } else {
                    if ((deltaX == 1 || deltaX == -1) && deltaY == 1) {
                        isCorrect = true;
                    }
                }

                break;
        }

        return isCorrect;
    }

    private bool IsFigureOnWay(Cell[,] cells, Cell startCell, Cell endCell) {
        bool isFigureOnWay = false;

        bool isFigureOnFirstWay;
        bool isFigureOnSecondWay;

        int x;
        int y;

        int step;

        Figure figure = startCell.figure;

        int deltaY;
        int deltaYR;
        int deltaX;

        if (startCell.x < 4 && endCell.x >= 4) {
            deltaX = (endCell.x - 4) - startCell.x;
            deltaY = (BOARD_SIZE - endCell.y) + (BOARD_SIZE - startCell.y) - 1;
            deltaYR = startCell.y + endCell.y + 1;
        } else if (startCell.x >= 4 && endCell.x < 4) {
            deltaX = endCell.x - (startCell.x - 4);
            deltaY = (BOARD_SIZE - endCell.y) + (BOARD_SIZE - startCell.y) - 1;
            deltaYR = startCell.y + endCell.y + 1;
        } else {
            deltaX = endCell.x - startCell.x;
            deltaY = deltaYR = endCell.y - startCell.y;
        }

        int deltaXabs = Mathf.Abs(deltaX);
        int deltaYabs = Mathf.Abs(deltaY);
        int deltaYRabs = Mathf.Abs(deltaYR);

        switch (figure.figureType) {
            case FigureType.Queen:
                break;
            case FigureType.Bishop:
                break;
            case FigureType.Rook:

                if (deltaY == 0) {
                    step = deltaX / deltaXabs;
                    for (int i = startCell.x + step; i < endCell.x; i += step) {
                        if (cells[i, endCell.y].figure != null) {
                            isFigureOnWay = true;
                            break;
                        }
                    }
                    break;
                }

                if (deltaX == 0) {
                    isFigureOnFirstWay = false;
                    isFigureOnSecondWay = false;
                    x = startCell.x;
                    y = startCell.y;

                    step = 1;
                    for (int i = 0; i < deltaYabs; i++) {
                        y += step;

                        if (y <= 0) {
                            y = BOARD_SIZE - 1;

                            if (x == startCell.x) {
                                x = endCell.x;
                            } else {
                                x = startCell.x;
                            }

                        } else if (y >= BOARD_SIZE) {
                            y = 0;

                            if (x == startCell.x) {
                                x = endCell.x;
                            } else {
                                x = startCell.x;
                            }
                        }

                        if (cells[x, y].figure != null) {
                            isFigureOnFirstWay = true;
                            break;
                        }
                    }

                    step = -1;
                    x = startCell.x;
                    y = startCell.y;
                    for (int i = 0; i < deltaYRabs; i++) {
                        y += step;

                        if (y <= 0) {
                            y = BOARD_SIZE - 1;

                            if (x == startCell.x) {
                                x = endCell.x;
                            } else {
                                x = startCell.x;
                            }

                        } else if (y >= BOARD_SIZE) {
                            y = 0;

                            if (x == startCell.x) {
                                x = endCell.x;
                            } else {
                                x = startCell.x;
                            }
                        }

                        if (cells[x, y].figure != null) {
                            isFigureOnSecondWay = true;
                            break;
                        }
                    }

                    if (isFigureOnFirstWay && isFigureOnSecondWay) {
                        isFigureOnWay = true;
                    }
                }

                break;

            case FigureType.Pawn:

                if (figure.isWhite) {
                    deltaY = endCell.y - startCell.y;
                } else {
                    deltaY = startCell.y - endCell.y;
                }

                if (deltaY != 2) {
                    break;
                }

                if (figure.isWhite) {
                    y = endCell.y - 1;
                } else {
                    y = endCell.y + 1;
                }

                if (cells[startCell.x, y].figure != null) {
                    isFigureOnWay = true;
                }

                break;
        }

        return isFigureOnWay;
    }
}
