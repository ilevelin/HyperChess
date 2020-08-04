using System.Linq;
using UnityEngine;

public class PieceLibraryLoader : MonoBehaviour
{
    [SerializeField] GameObject libraryObjectPrefab, libraryListParent, cameraObject;
    MainLibrary mainLibrary;

    void Start()
    {
        mainLibrary = GameObject.FindGameObjectWithTag("MainLibrary").GetComponent<MainLibrary>();

        for (int i = 0; i < mainLibrary.pieceLibrary.Values.Count; i++)
        {
            PieceElement element = mainLibrary.pieceLibrary.Values.ToList()[i];
            GameObject tmp = GameObject.Instantiate(libraryObjectPrefab, new Vector3(1.0f, -0.5f * i), new Quaternion(), libraryListParent.transform);
            tmp.GetComponent<PieceLibraryObject>().LoadPiece(element.image, element.name, element.version, element.author);
        }
    }

    private void Update()
    {
        cameraObject.transform.position = new Vector3((((float) Screen.width) / ((float) Screen.height)), 0.0f, -10.0f);
    }
}
