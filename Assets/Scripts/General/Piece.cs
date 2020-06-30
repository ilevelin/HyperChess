using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{

    int[] position = null;
    bool onHold = false;

    private void Update()
    {
        if (onHold)
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
    }

    public void Initialize(int[] initialPosition, GameObject inputDetector)
    {
        position = initialPosition;
        inputDetector.GetComponent<BoardInputDetector>().PieceSubscription(initialPosition, this);
        RenderPiece();
    }

    public void MoveTo(int[] newPosition)
    {
        position = newPosition;
        RenderPiece();
    }

    public void Captured()
    {
        GameObject.Destroy(gameObject);
    }

    public void Catched()
    {
        onHold = true;
    }

    public void Released()
    {
        onHold = false;
        RenderPiece();
    }

    private void RenderPiece()
    {
        transform.position = new Vector3(position[0], position[1]);
    }

}
