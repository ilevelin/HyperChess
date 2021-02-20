
public interface Board { }

public class BoardSquare2D : Board
{
    public string[][] board;
    public bool[,,] promotions;
    public int[][] colors;
}
