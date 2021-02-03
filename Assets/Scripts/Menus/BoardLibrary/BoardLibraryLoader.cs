using System;
using System.Linq;
using UnityEngine;

public class BoardLibraryLoader : MonoBehaviour
{
    [SerializeField] GameObject libraryObjectPrefab, libraryListParent, cameraObject;
    [SerializeField] CameraSlider cameraSlider;
    [SerializeField] BoardLibraryCoordinator coordinator;
    MainLibrary mainLibrary;

    void Start()
    {
        try
        {
            mainLibrary = GameObject.FindGameObjectWithTag("MainLibrary").GetComponent<MainLibrary>();

            string[] ids = mainLibrary.boardLibrary.Keys.ToArray();
            int i = 0;

            //for (int i = 0; i < ids.Length; i++)
            foreach (string id in ids)
            {
                //string id = ids[i];
                BoardElement element;
                if (mainLibrary.boardLibrary.TryGetValue(id, out element))
                {
                    GameObject tmp = GameObject.Instantiate(libraryObjectPrefab, new Vector3(.9f, -0.5f * i), new Quaternion(), libraryListParent.transform);
                    tmp.GetComponent<BoardLibraryObject>().LoadBoard(element.image, id, element.version, element.author, coordinator);
                    i++;
                }
                else Debug.Log($"FAILED TO LOAD: {id}");
            }

            cameraSlider.AfterLibraryLoaded(ids.Length);
        }
        catch(Exception e)
        {
            Debug.LogError($"Loading scene without library? : {e}");

            cameraSlider.AfterLibraryLoaded(0);
        }

    }
}
