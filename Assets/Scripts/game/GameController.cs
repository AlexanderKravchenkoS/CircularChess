using UnityEngine;
using cell;
using figure;
using net;
using option;
using resource;

namespace game {
    public class GameController : MonoBehaviour {
        public Resource resources;

        public GameState gameState;
        public bool isWhiteMove;
        public bool isWhitePlayer;

        private GameObject playground;

        private Client client;
        private Server server;

        private Cell[,] cells;
        private CellData[,] cellDatas;

        private Cell startCellWithPawn;
        private Cell endCellWithPawn;

        private const int BOARD_SIZE = 8;

        private void Start() {
            gameState = GameState.Stop;
        }

        public void StartGame() {
            playground = Instantiate(resources.playground);
            var board = playground.transform.Find("Board");

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

                        cellData.figureData = Option<FigureData>.Some(cell.figure.figureData);
                    }

                    cells[cell.x, cell.y] = cell;
                    cellDatas[cell.x, cell.y] = cellData;
                }
            }

            gameState = GameState.Running;
            isWhiteMove = true;
        }

        public void ProcessSelect(Cell startCell, Cell endCell) {

            if (!IsCorrectMove(cellDatas, startCell.x, startCell.y, endCell.x, endCell.y)) {
                return;
            }

            MakeTurn(startCell, endCell);
        }

        private bool IsCorrectMove(CellData[,] data, int startX, int startY, int endX, int endY) {

            CellData[,] tempCells = new CellData[BOARD_SIZE, BOARD_SIZE];
            for (int i = 0; i < BOARD_SIZE; i++) {
                for (int j = 0; j < BOARD_SIZE; j++) {
                    tempCells[i, j] = data[i, j];
                }
            }

            var startCellData = tempCells[startX, startY];
            var endCellData = tempCells[endX, endY];

            if (!IsCorrectMovePattern(startCellData, endCellData)) {
                return false;
            }

            if (IsFigureOnWay(tempCells, startCellData, endCellData)) {
                return false;
            }

            var figureData = tempCells[startX, startY].figureData.Peel();
            figureData.isFirstMove = false;
            tempCells[endX, endY].figureData = Option<FigureData>.Some(figureData);
            tempCells[startX, startY].figureData = Option<FigureData>.None();

            if (IsKingUnderCheck(tempCells, figureData.isWhite)) {
                return false;
            }

            return true;
        }

        private bool IsCorrectMovePattern(CellData startCell, CellData endCell) {
            FigureData figure = startCell.figureData.Peel();

            bool isCorrect = false;

            int deltaY;
            int deltaX;

            if (startCell.x < BOARD_SIZE / 2 && endCell.x >= BOARD_SIZE / 2) {
                deltaX = (endCell.x - BOARD_SIZE / 2) - startCell.x;
                deltaY = (BOARD_SIZE - endCell.y) + (BOARD_SIZE - startCell.y) - 1;
            } else if (startCell.x >= BOARD_SIZE / 2 && endCell.x < BOARD_SIZE / 2) {
                deltaX = endCell.x - (startCell.x - BOARD_SIZE / 2);
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

                    if (endCell.figureData.IsNone()) {
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
            FigureData figure = startCell.figureData.Peel();

            bool isFigureOnWay = false;

            bool isFigureOnFirstWay;
            bool isFigureOnSecondWay;

            int x;
            int y;

            int stepX;
            int stepY;

            int deltaY;
            int deltaX;

            if (startCell.x < BOARD_SIZE / 2 && endCell.x >= BOARD_SIZE / 2) {
                deltaX = (endCell.x - BOARD_SIZE / 2) - startCell.x;
                deltaY = (BOARD_SIZE - endCell.y) + (BOARD_SIZE - startCell.y) - 1;
            } else if (startCell.x >= BOARD_SIZE / 2 && endCell.x < BOARD_SIZE / 2) {
                deltaX = endCell.x - (startCell.x - BOARD_SIZE / 2);
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
                        x = startCell.x;
                        while (!isFigureOnWay) {
                            x += stepX;

                            if (x == endCell.x) {
                                break;
                            }

                            if (cellDatas[x, endCell.y].figureData.IsNone()) {
                                continue;
                            }

                            isFigureOnWay = true;
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

                            if (cellDatas[x, y].figureData.IsSome()) {
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

                            if (cellDatas[x, y].figureData.IsSome()) {
                                isFigureOnSecondWay = true;
                            }
                        }

                        if (isFigureOnFirstWay && isFigureOnSecondWay) {
                            isFigureOnWay = true;
                        }
                    }

                    if (deltaXabs == deltaYabs || deltaXabs == deltaYRabs) {
                        if (startCell.x < BOARD_SIZE / 2 && endCell.x >= BOARD_SIZE / 2) {

                            if (startCell.x + BOARD_SIZE / 2 > endCell.x) {
                                stepX = -1;
                            } else {
                                stepX = 1;
                            }

                            if (startCell.y >= BOARD_SIZE / 2) {
                                stepY = 1;
                            } else {
                                stepY = -1;
                            }

                        } else if (startCell.x >= BOARD_SIZE / 2 && endCell.x < BOARD_SIZE / 2) {

                            if (startCell.x - BOARD_SIZE / 2 > endCell.x) {
                                stepX = -1;
                            } else {
                                stepX = 1;
                            }

                            if (startCell.y >= BOARD_SIZE / 2) {
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

                            if (y + stepY >= BOARD_SIZE) {
                                y = BOARD_SIZE;
                                stepY = -stepY;
                                if (x < BOARD_SIZE / 2) {
                                    x += BOARD_SIZE / 2;
                                } else {
                                    x -= BOARD_SIZE / 2;
                                }
                            } else if (y + stepY <= -1) {
                                y = -1;
                                stepY = -stepY;
                                if (x < BOARD_SIZE / 2) {
                                    x += BOARD_SIZE / 2;
                                } else {
                                    x -= BOARD_SIZE / 2;
                                }
                            }

                            x += stepX;
                            y += stepY;

                            if (x == endCell.x && y == endCell.y) {
                                break;
                            }

                            if (cellDatas[x, y].figureData.IsSome()) {
                                isFigureOnWay = true;
                            }

                        }
                    }

                    break;
                case FigureType.Bishop:

                    if (startCell.x < BOARD_SIZE / 2 && endCell.x >= BOARD_SIZE / 2) {

                        if (startCell.x + BOARD_SIZE / 2 > endCell.x) {
                            stepX = -1;
                        } else {
                            stepX = 1;
                        }

                        if (startCell.y >= BOARD_SIZE / 2) {
                            stepY = 1;
                        } else {
                            stepY = -1;
                        }

                    } else if (startCell.x >= BOARD_SIZE / 2 && endCell.x < BOARD_SIZE / 2) {

                        if (startCell.x - BOARD_SIZE / 2 > endCell.x) {
                            stepX = -1;
                        } else {
                            stepX = 1;
                        }

                        if (startCell.y >= BOARD_SIZE / 2) {
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

                        if (y + stepY >= BOARD_SIZE) {
                            y = BOARD_SIZE;
                            stepY = -stepY;
                            if (x < BOARD_SIZE / 2) {
                                x += BOARD_SIZE / 2;
                            } else {
                                x -= BOARD_SIZE / 2;
                            }
                        } else if (y + stepY <= -1) {
                            y = -1;
                            stepY = -stepY;
                            if (x < BOARD_SIZE / 2) {
                                x += BOARD_SIZE / 2;
                            } else {
                                x -= BOARD_SIZE / 2;
                            }
                        }

                        x += stepX;
                        y += stepY;

                        if (x == endCell.x && y == endCell.y) {
                            break;
                        }

                        if (cellDatas[x, y].figureData.IsSome()) {
                            isFigureOnWay = true;
                        }

                    }

                    break;
                case FigureType.Rook:

                    if (deltaY == 0) {
                        stepX = deltaX / deltaXabs;
                        x = startCell.x;
                        while (!isFigureOnWay) {
                            x += stepX;

                            if (x == endCell.x) {
                                break;
                            }

                            if (cellDatas[x, endCell.y].figureData.IsNone()) {
                                continue;
                            }

                            isFigureOnWay = true;
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

                            if (cellDatas[x, y].figureData.IsSome()) {
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

                            if (cellDatas[x, y].figureData.IsSome()) {
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

                    if (cellDatas[startCell.x, y].figureData.IsSome()) {
                        isFigureOnWay = true;
                    }

                    break;
            }

            return isFigureOnWay;
        }

        private bool IsKingUnderCheck(CellData[,] cellDatas, bool isWhiteTurn) {

            CellData cellWithKing = new CellData();

            foreach (var item in cellDatas) {
                if (item.figureData.IsNone()) {
                    continue;
                }

                if (item.figureData.Peel().isWhite != isWhiteTurn) {
                    continue;
                }

                if (item.figureData.Peel().figureType != FigureType.King) {
                    continue;
                }

                cellWithKing = item;
                break;
            }

            foreach (var item in cellDatas) {
                if (item.figureData.IsNone()) {
                    continue;
                }

                if (item.figureData.Peel().isWhite == cellWithKing.figureData.Peel().isWhite) {
                    continue;
                }

                if (cellWithKing.x == item.x && cellWithKing.y == item.y) {
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

            if (IsPawnInTheEnd(endCell)) {
                gameState = GameState.Pause;
                startCellWithPawn = startCell;
                endCellWithPawn = endCell;
                return;
            }

            isWhiteMove = !isWhiteMove;

            gameState = GetNewGameState(cellDatas, isWhiteMove);

            if (!client) {
                isWhitePlayer = !isWhitePlayer;
                return;
            }

            string msg = Server.MOVE_COMMAND + "|"
                + startCell.x.ToString() + "|"
                + startCell.y.ToString() + "|"
                + endCell.x.ToString() + "|"
                + endCell.y.ToString() + "|"
                + ((int)endCell.figure.figureData.figureType).ToString() + "|"
                + ((int)gameState).ToString();

            client.SendData(msg);
        }

        public void MakeTurn(int startX, int startY, int endX, int endY, int type, int state) {

            if (cells[endX, endY].figure != null) {
                Destroy(cells[endX, endY].figure.gameObject);
            }

            MoveFigure(cells[startX, startY], cells[endX, endY]);

            if ((FigureType)type != cells[endX, endY].figure.figureData.figureType) {
                endCellWithPawn = cells[endX, endY];
                TransformPawnToNewFigure((FigureType)type);
            }

            isWhiteMove = !isWhiteMove;

            gameState = (GameState)state;
        }

        private void MoveFigure(Cell startCell, Cell endCell) {
            Vector3 newPosition = endCell.point.position;
            newPosition.y = startCell.figure.transform.position.y;

            startCell.figure.transform.position = newPosition;
            startCell.figure.figureData.isFirstMove = false;

            endCell.figure = startCell.figure;

            var figureData = cellDatas[startCell.x, startCell.y].figureData.Peel();
            figureData.isFirstMove = false;
            cellDatas[endCell.x, endCell.y].figureData = Option<FigureData>.Some(figureData);
            cellDatas[startCell.x, startCell.y].figureData = Option<FigureData>.None();

            startCell.figure = null;
        }

        private GameState GetNewGameState(CellData[,] cellDatas, bool isWhiteTurn) {
            bool isCheck = IsKingUnderCheck(cellDatas, isWhiteTurn);

            foreach (var start in cellDatas) {
                if (start.figureData.IsNone()) {
                    continue;
                }

                if (start.figureData.Peel().isWhite != isWhiteTurn) {
                    continue;
                }

                for (int i = 0; i < BOARD_SIZE; i++) {
                    for (int j = 0; j < BOARD_SIZE; j++) {
                        if (start.x == i && start.y == j) {
                            continue;
                        }

                        if (cellDatas[i, j].figureData.IsSome()) {
                            if (start.figureData.Peel().isWhite ==
                                cellDatas[i, j].figureData.Peel().isWhite) {

                                continue;
                            }
                        }

                        if (!IsCorrectMove(cellDatas, start.x, start.y, i, j)) {
                            continue;
                        }

                        return GameState.Running;
                    }
                }
            }

            if (isCheck) {
                return GameState.Win;
            } else {
                return GameState.Draw;
            }
        }

        private bool IsPawnInTheEnd(Cell cell) {
            if (cell.figure.figureData.figureType != FigureType.Pawn) {
                return false;
            }

            if (cell.y == 0 || cell.y == 7) {
                return true;
            }

            return false;
        }

        public void HighlightAllMoves(Cell startCell) {
            startCell.gameObject.GetComponent<MeshRenderer>().material = startCell.secondMaterial;

            foreach (var cell in cellDatas) {

                if (cell.x == startCell.x && cell.y == startCell.y) {
                    continue;
                }

                if (cell.figureData.IsSome()) {
                    if (cell.figureData.Peel().isWhite == startCell.figure.figureData.isWhite) {
                        continue;
                    }
                }

                if (!IsCorrectMove(cellDatas, startCell.x, startCell.y, cell.x, cell.y)) {
                    continue;
                }

                var material = cells[cell.x, cell.y].secondMaterial;
                cells[cell.x, cell.y].gameObject.GetComponent<MeshRenderer>().material = material;
            }
        }

        public void RemoveHighlight() {
            foreach (var cell in cells) {
                cell.gameObject.GetComponent<MeshRenderer>().material = cell.mainMaterial;
            }
        }

        public void Host() {
            try {
                Server server = Instantiate(resources.serverPrefab);
                server.Init();
                this.server = server;
                server.listener.Pending();


                Client client = Instantiate(resources.clientPrefab);
                client.gameController = this;
                this.client = client;
                isWhitePlayer = true;

                if (!client.ConnectToServer(Server.STANDART_IP, Server.PORT)) {
                    Destroy(client.gameObject);
                    return;
                }

                gameState = GameState.Waiting;
            } catch (System.Exception e) {
                Debug.Log(e.Message);
                Destroy(server.gameObject);
                gameState = GameState.HostError;
            }
        }

        public void Connect(string hostAddress) {
            try {
                Client client = Instantiate(resources.clientPrefab);
                client.gameController = this;
                this.client = client;
                isWhitePlayer = false;

                if (!client.ConnectToServer(hostAddress, Server.PORT)) {
                    Destroy(client.gameObject);
                    return;
                }

                gameState = GameState.Waiting;
            } catch (System.Exception e) {
                Debug.Log(e.Message);
            }
        }

        public void Hotseat() {
            StartGame();

            isWhitePlayer = true;
        }

        public void TransformPawn(FigureType newType) {
            TransformPawnToNewFigure(newType);

            isWhiteMove = !isWhiteMove;

            gameState = GetNewGameState(cellDatas, isWhiteMove);

            if (!client) {
                isWhitePlayer = !isWhitePlayer;
                return;
            }

            string msg = Server.MOVE_COMMAND + "|"
                + startCellWithPawn.x.ToString() + "|"
                + startCellWithPawn.y.ToString() + "|"
                + endCellWithPawn.x.ToString() + "|"
                + endCellWithPawn.y.ToString() + "|"
                + ((int)endCellWithPawn.figure.figureData.figureType).ToString() + "|"
                + ((int)gameState).ToString();

            client.SendData(msg);
        }

        private void TransformPawnToNewFigure(FigureType newType) {
            Figure figure;

            if (endCellWithPawn.figure.figureData.isWhite) {
                figure = resources.whiteFigurePrefabs[newType];
            } else {
                figure = resources.blackFigurePrefabs[newType];
            }

            var position = endCellWithPawn.figure.transform.position;
            var rotation = endCellWithPawn.figure.transform.rotation;
            var newFigure = Instantiate(figure, position, rotation, playground.transform);

            Destroy(endCellWithPawn.figure.gameObject);
            endCellWithPawn.figure = newFigure;
            cellDatas[endCellWithPawn.x, endCellWithPawn.y].figureData =
                Option<FigureData>.Some(figure.figureData);
        }

        public void ClearGame() {
            if (client) {
                Destroy(client.gameObject);
            }

            if (server) {
                Destroy(server.gameObject);
            }

            if (playground) {
                Destroy(playground);
            }
        }
    }
}