using UnityEngine;
using cell;
using figure;
using System.Collections.Generic;

namespace game {
    public class GameManager : MonoBehaviour {
        public GameObject board;

        public bool isWhiteMove;
        public bool isRunning;

        private Cell[,] cells;
        private CellData[,] cellDatas;

        private const int BOARD_SIZE = 8;

        private void Start() {
            cells = new Cell[BOARD_SIZE, BOARD_SIZE];
            cellDatas = new CellData[BOARD_SIZE, BOARD_SIZE];

            for (int i = 0; i < BOARD_SIZE; i++) {

                var column = board.transform.GetChild(i);

                for (int j = 0; j < BOARD_SIZE; j++) {

                    var cell = column.transform.GetChild(j).GetComponent<Cell>();

                    CellData cellData = new CellData {
                        x = cell.x,
                        y = cell.y
                    };

                    if (cell.figure != null) {
                        var position = cell.point.transform.position;
                        position.y = cell.figure.transform.position.y;
                        cell.figure.transform.position = position;

                        cellData.figureData = cell.figure.figureData;
                    }

                    cells[cell.x, cell.y] = cell;
                    cellDatas[cell.x, cell.y] = cellData;
                }
            }

            isWhiteMove = true;
            isRunning = true;
        }

        public void ProcessSelect(Cell startCell, Cell endCell) {

            if (!IsCorrectMove(startCell, endCell)) {
                return;
            }

            MakeTurn(startCell, endCell);
        }

        private bool IsCorrectMove(Cell startCell, Cell endCell) {

            CellData[,] tempCells = new CellData[BOARD_SIZE, BOARD_SIZE];
            for (int i = 0; i < BOARD_SIZE; i++) {
                for (int j = 0; j < BOARD_SIZE; j++) {
                    tempCells[i, j] = cellDatas[i, j];
                }
            }

            var startCellData = tempCells[startCell.x, startCell.y];
            var endCellData = tempCells[endCell.x, endCell.y];

            if (!IsCorrectMovePattern(startCellData, endCellData)) {
                return false;
            }

            if (IsFigureOnWay(tempCells, startCellData, endCellData)) {
                return false;
            }

            var figureData = tempCells[startCell.x, startCell.y].figureData;
            figureData.isFirstMove = false;
            tempCells[endCell.x, endCell.y].figureData = figureData;
            tempCells[startCell.x, startCell.y].figureData = null;

            if (IsChecked(tempCells, figureData.isWhite)) {
                return false;
            }

            return true;
        }

        private bool IsCorrectMovePattern(CellData startCell, CellData endCell) {
            FigureData figure = startCell.figureData;

            bool isCorrect = false;

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

                case FigureType.King:

                    if (deltaXabs == 1 && (deltaYabs == 0 || deltaYRabs == 0)) {
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

                    if (endCell.figureData == null) {
                        if (deltaY == 2 && figure.isFirstMove && deltaX == 0) {
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

        private bool IsFigureOnWay(CellData[,] cellDatas, CellData startCell, CellData endCell) {
            FigureData figure = startCell.figureData;

            bool isFigureOnWay = false;

            bool isFigureOnFirstWay;
            bool isFigureOnSecondWay;

            int x;
            int y;

            int stepX;
            int stepY;

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
                        stepX = deltaX / deltaXabs;
                        for (int i = startCell.x + stepX; i < endCell.x; i += stepX) {
                            if (cellDatas[i, endCell.y].figureData != null) {
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

                        stepY = 1;

                        while (!isFigureOnFirstWay) {

                            y += stepY;
                            if (y >= BOARD_SIZE) {
                                stepY = -stepY;
                                y = BOARD_SIZE - 1;
                                if (x < BOARD_SIZE / 2) {
                                    x += BOARD_SIZE / 2;
                                } else {
                                    x -= BOARD_SIZE / 2;
                                }
                            } else if (y < 0) {
                                stepY = -stepY;
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

                            if (cellDatas[x, y].figureData != null) {
                                isFigureOnFirstWay = true;
                            }

                        }

                        x = startCell.x;
                        y = startCell.y;

                        stepY = -1;

                        while (!isFigureOnSecondWay) {

                            y += stepY;
                            if (y >= BOARD_SIZE) {
                                stepY = -stepY;
                                y = BOARD_SIZE - 1;
                                if (x < BOARD_SIZE / 2) {
                                    x += BOARD_SIZE / 2;
                                } else {
                                    x -= BOARD_SIZE / 2;
                                }
                            } else if (y < 0) {
                                stepY = -stepY;
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

                            if (cellDatas[x, y].figureData != null) {
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

                            if (cellDatas[x, y].figureData != null) {
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

                        if (cellDatas[x, y].figureData != null) {
                            isFigureOnWay = true;
                        }

                    }

                    break;
                case FigureType.Rook:

                    if (deltaY == 0) {
                        stepX = deltaX / deltaXabs;
                        for (int i = startCell.x + stepX; i < endCell.x; i += stepX) {
                            if (cellDatas[i, endCell.y].figureData != null) {
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

                        stepY = 1;

                        while (!isFigureOnFirstWay) {

                            y += stepY;
                            if (y >= BOARD_SIZE) {
                                stepY = -stepY;
                                y = BOARD_SIZE - 1;
                                if (x < BOARD_SIZE / 2) {
                                    x += BOARD_SIZE / 2;
                                } else {
                                    x -= BOARD_SIZE / 2;
                                }
                            } else if (y < 0) {
                                stepY = -stepY;
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

                            if (cellDatas[x, y].figureData != null) {
                                isFigureOnFirstWay = true;
                            }

                        }

                        x = startCell.x;
                        y = startCell.y;

                        stepY = -1;

                        while (!isFigureOnSecondWay) {

                            y += stepY;
                            if (y >= BOARD_SIZE) {
                                stepY = -stepY;
                                y = BOARD_SIZE - 1;
                                if (x < BOARD_SIZE / 2) {
                                    x += BOARD_SIZE / 2;
                                } else {
                                    x -= BOARD_SIZE / 2;
                                }
                            } else if (y < 0) {
                                stepY = -stepY;
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

                            if (cellDatas[x, y].figureData != null) {
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

                    if (cellDatas[startCell.x, y].figureData != null) {
                        isFigureOnWay = true;
                    }

                    break;
            }

            return isFigureOnWay;
        }

        private bool IsChecked(CellData[,] cellDatas, bool isWhiteTurn) {

            CellData cellWithKing = new CellData();

            foreach (var item in cellDatas) {
                if (item.figureData == null) {
                    continue;
                }

                if (item.figureData.isWhite != isWhiteTurn) {
                    continue;
                }

                if (item.figureData.figureType != FigureType.King) {
                    continue;
                }

                cellWithKing = item;
                break;
            }

            foreach (var item in cellDatas) {
                if (item.figureData == null) {
                    continue;
                }

                if (item.figureData.isWhite == cellWithKing.figureData.isWhite) {
                    continue;
                }

                var startCellData = item;

                if (!IsCorrectMovePattern(startCellData, cellWithKing)) {
                    continue;
                }

                if (IsFigureOnWay(cellDatas, startCellData, cellWithKing)) {
                    continue;
                }

                return true;
            }

            return false;
        }

        private void MakeTurn(Cell startCell, Cell endCell) {

            if (endCell.figure != null) {
                Destroy(endCell.figure.gameObject);
            }

            MoveFigure(startCell, endCell);

            isWhiteMove = !isWhiteMove;
        }

        private void MoveFigure(Cell startCell, Cell endCell) {
            Vector3 newPosition = endCell.point.position;
            newPosition.y = startCell.figure.transform.position.y;

            startCell.figure.transform.position = newPosition;
            startCell.figure.figureData.isFirstMove = false;

            endCell.figure = startCell.figure;

            var figureData = cellDatas[startCell.x, startCell.y].figureData;
            figureData.isFirstMove = false;
            cellDatas[endCell.x, endCell.y].figureData = figureData;
            cellDatas[startCell.x, startCell.y].figureData = null;

            startCell.figure = null;
        }

        public List<Cell> FindAllMoves(Cell[,] cells, Cell startCell) {
            List<Cell> allMoves = new List<Cell>();

            foreach (var item in cells) {
                if (item.x == startCell.x && item.y == startCell.y) {
                    continue;
                }

                if (!IsCorrectMove(item, startCell)) {
                    continue;
                }

                allMoves.Add(item);
            }

            return allMoves;
        }
    }
}