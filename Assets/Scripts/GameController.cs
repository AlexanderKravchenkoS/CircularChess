using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour{
    public GameObject board;

    public bool isWhiteMove;
    public bool isRunning;

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
        isRunning = true;
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
            if (newCell.figure.figureType == FigureType.King) {
                isRunning = false;
                if (isWhiteMove) {
                    Debug.Log("White WIN");
                } else {
                    Debug.Log("Black WIN");
                }
                Destroy(newCell.figure.gameObject);
                return;
            }
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
            deltaY = endCell.y - startCell.y;
            deltaYR = BOARD_SIZE * 2 - deltaY;
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
        int n;

        int step;
        int stepX;
        int stepY;

        Figure figure = startCell.figure;

        int deltaY;
        int deltaX;

        if (startCell.x < 4 && endCell.x >= 4) {
            deltaX = (endCell.x - 4) - startCell.x;
            deltaY = (BOARD_SIZE - endCell.y) + (BOARD_SIZE - startCell.y) - 1;
        } else if (startCell.x >= 4 && endCell.x < 4) {
            deltaX = endCell.x - (startCell.x - 4);
            deltaY = (BOARD_SIZE - endCell.y) + (BOARD_SIZE - startCell.y) - 1;
        } else {
            deltaX = endCell.x - startCell.x;
            deltaY = endCell.y - startCell.y;
        }

        int deltaXabs = Mathf.Abs(deltaX);
        int deltaYabs = Mathf.Abs(deltaY);
        int deltaYRabs = BOARD_SIZE * 2 - deltaYabs;

        switch (figure.figureType) {
            case FigureType.Queen:

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
                    x = startCell.x;
                    y = startCell.y;

                    isFigureOnFirstWay = false;
                    isFigureOnSecondWay = false;

                    step = 1;

                    while (!isFigureOnFirstWay) {

                        y += step;
                        if (y >= BOARD_SIZE) {
                            step = -step;
                            y = BOARD_SIZE - 1;
                            if (x < BOARD_SIZE / 2) {
                                x += BOARD_SIZE / 2;
                            } else {
                                x -= BOARD_SIZE / 2;
                            }
                        } else if (y < 0) {
                            step = -step;
                            y = 0;
                            if (x < BOARD_SIZE / 2) {
                                x += BOARD_SIZE / 2;
                            } else {
                                x -= BOARD_SIZE / 2;
                            }
                        }

                        if (x == endCell.x && y == endCell.y) {
                            break;
                        }

                        if (cells[x, y].figure != null) {
                            isFigureOnFirstWay = true;
                        }

                    }

                    x = startCell.x;
                    y = startCell.y;

                    step = -1;

                    while (!isFigureOnSecondWay) {

                        y += step;
                        if (y >= BOARD_SIZE) {
                            step = -step;
                            y = BOARD_SIZE - 1;
                            if (x < BOARD_SIZE / 2) {
                                x += BOARD_SIZE / 2;
                            } else {
                                x -= BOARD_SIZE / 2;
                            }
                        } else if (y < 0) {
                            step = -step;
                            y = 0;
                            if (x < BOARD_SIZE / 2) {
                                x += BOARD_SIZE / 2;
                            } else {
                                x -= BOARD_SIZE / 2;
                            }
                        }

                        if (x == endCell.x && y == endCell.y) {
                            break;
                        }

                        if (cells[x, y].figure != null) {
                            isFigureOnSecondWay = true;
                        }
                    }

                    if (isFigureOnFirstWay && isFigureOnSecondWay) {
                        isFigureOnWay = true;
                    }
                    break;
                }

                if (deltaXabs == deltaYabs || deltaXabs == deltaYRabs) {
                    if (startCell.x < 4 && endCell.x >= 4) {

                        if (startCell.x + 4 > endCell.x) {
                            stepX = -1;
                        } else {
                            stepX = 1;
                        }

                        if (startCell.y >= 4) {
                            stepY = 1;
                        } else {
                            stepY = -1;
                        }

                    } else if (startCell.x >= 4 && endCell.x < 4) {

                        if (startCell.x - 4 > endCell.x) {
                            stepX = -1;
                        } else {
                            stepX = 1;
                        }

                        if (startCell.y >= 4) {
                            stepY = 1;
                        } else {
                            stepY = -1;
                        }

                    } else {

                        if (startCell.x > endCell.x) {
                            stepX = -1;
                        } else {
                            stepX = 1;
                        }

                        if (startCell.y > endCell.y) {
                            stepY = -1;
                        } else {
                            stepY = 1;
                        }

                    }

                    x = startCell.x;
                    y = startCell.y;

                    if (deltaYabs <= 4) {
                        n = deltaYabs;
                    } else {
                        n = deltaYRabs;
                    }

                    while (!isFigureOnWay) {

                        if (y + stepY >= 8) {
                            y = 8;
                            stepY = -stepY;
                            if (x < 4) {
                                x += 4;
                            } else {
                                x -= 4;
                            }
                        } else if (y + stepY <= -1) {
                            y = -1;
                            stepY = -stepY;
                            if (x < 4) {
                                x += 4;
                            } else {
                                x -= 4;
                            }
                        }

                        x += stepX;
                        y += stepY;

                        if (x == endCell.x && y == endCell.y) {
                            break;
                        }

                        if (cells[x, y].figure != null) {
                            isFigureOnWay = true;
                        }

                    }
                }

                break;
            case FigureType.Bishop:

                if (startCell.x < 4 && endCell.x >= 4) {

                    if (startCell.x + 4 > endCell.x) {
                        stepX = -1;
                    } else {
                        stepX = 1;
                    }

                    if (startCell.y >= 4) {
                        stepY = 1;
                    } else {
                        stepY = -1;
                    }

                } else if (startCell.x >= 4 && endCell.x < 4) {

                    if (startCell.x - 4 > endCell.x) {
                        stepX = -1;
                    } else {
                        stepX = 1;
                    }

                    if (startCell.y >= 4) {
                        stepY = 1;
                    } else {
                        stepY = -1;
                    }

                } else {

                    if (startCell.x > endCell.x) {
                        stepX = -1;
                    } else {
                        stepX = 1;
                    }

                    if (startCell.y > endCell.y) {
                        stepY = -1;
                    } else {
                        stepY = 1;
                    }

                }

                x = startCell.x;
                y = startCell.y;

                if (deltaYabs <= 4) {
                    n = deltaYabs;
                } else {
                    n = deltaYRabs;
                }

                while (!isFigureOnWay) {

                    if (y + stepY >= 8) {
                        y = 8;
                        stepY = -stepY;
                        if (x < 4) {
                            x += 4;
                        } else {
                            x -= 4;
                        }
                    } else if (y + stepY <= -1) {
                        y = -1;
                        stepY = -stepY;
                        if (x < 4) {
                            x += 4;
                        } else {
                            x -= 4;
                        }
                    }

                    x += stepX;
                    y += stepY;

                    if (x == endCell.x && y == endCell.y) {
                        break;
                    }

                    if (cells[x, y].figure != null) {
                        isFigureOnWay = true;
                    }

                }

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
                    x = startCell.x;
                    y = startCell.y;

                    isFigureOnFirstWay = false;
                    isFigureOnSecondWay = false;

                    step = 1;

                    while (!isFigureOnFirstWay) {

                        y += step;
                        if (y >= BOARD_SIZE) {
                            step = -step;
                            y = BOARD_SIZE - 1;
                            if (x < BOARD_SIZE / 2) {
                                x += BOARD_SIZE / 2;
                            } else {
                                x -= BOARD_SIZE / 2;
                            }
                        } else if (y < 0) {
                            step = -step;
                            y = 0;
                            if (x < BOARD_SIZE / 2) {
                                x += BOARD_SIZE / 2;
                            } else {
                                x -= BOARD_SIZE / 2;
                            }
                        }

                        if (x == endCell.x && y == endCell.y) {
                            break;
                        }

                        if (cells[x, y].figure != null) {
                            isFigureOnFirstWay = true;
                        }

                    }

                    x = startCell.x;
                    y = startCell.y;

                    step = -1;

                    while (!isFigureOnSecondWay) {

                        y += step;
                        if (y >= BOARD_SIZE) {
                            step = -step;
                            y = BOARD_SIZE - 1;
                            if (x < BOARD_SIZE / 2) {
                                x += BOARD_SIZE / 2;
                            } else {
                                x -= BOARD_SIZE / 2;
                            }
                        } else if (y < 0) {
                            step = -step;
                            y = 0;
                            if (x < BOARD_SIZE / 2) {
                                x += BOARD_SIZE / 2;
                            } else {
                                x -= BOARD_SIZE / 2;
                            }
                        }

                        if (x == endCell.x && y == endCell.y) {
                            break;
                        }

                        if (cells[x, y].figure != null) {
                            isFigureOnSecondWay = true;
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
