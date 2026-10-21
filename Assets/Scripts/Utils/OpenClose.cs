using UnityEngine;
using System.Collections.Generic;

public class OpenClose : MonoBehaviour
{
    [SerializeField] private KeyCode key;
    [SerializeField] private List<GameObject> objToOpenClose;

    private void Update()
    {
        if (Input.GetKeyDown(key))
        {
            foreach (GameObject obj in objToOpenClose)
            {
                obj.SetActive(!obj.activeSelf);
            }
        }    
    }
}
