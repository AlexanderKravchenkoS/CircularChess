using UnityEngine;

public class Figure : MonoBehaviour{
    public FigureType figureType;
    public bool isWhite;
    public int startX;
    public int startY;
    public bool isFirstTurn = true;
}

public enum FigureType {
    King,
    Queen,
    Bishop,
    Knight,
    Rook,
    Pawn
}