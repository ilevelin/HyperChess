using System.Linq;
using UnityEngine;

public class PieceLibraryLoader : MonoBehaviour
{
    [SerializeField] GameObject libraryObjectPrefab, libraryListParent, cameraObject;
    [SerializeField] CameraSlider cameraSlider;
    [SerializeField] PieceLibraryCoordinator coordinator;
    MainLibrary mainLibrary;

    void Start()
    {
        try
        {
            mainLibrary = GameObject.FindGameObjectWithTag("MainLibrary").GetComponent<MainLibrary>();

            string[] ids = mainLibrary.pieceLibrary.Keys.ToArray();
            int i = 0;

            //for (int i = 0; i < ids.Length; i++)
            foreach (string id in ids)
            {
                //string id = ids[i];
                PieceElement element;
                if (mainLibrary.pieceLibrary.TryGetValue(id, out element))
                {
                    Sprite image;
                    element.sprites.TryGetValue("default", out image);
                    GameObject tmp = GameObject.Instantiate(libraryObjectPrefab, new Vector3(1.0f, -0.5f * i), new Quaternion(), libraryListParent.transform);
                    tmp.GetComponent<PieceLibraryObject>().LoadPiece(id, image, element.name, element.version, element.author, coordinator);
                    i++;
                }
                else Debug.Log($"FAILED TO LOAD: {i}");
            }

            cameraSlider.AfterLibraryLoaded(ids.Length);
        }
        catch
        {
            Debug.LogError("Loading scene without library!");

            cameraSlider.AfterLibraryLoaded(0);
        }

    }
}
