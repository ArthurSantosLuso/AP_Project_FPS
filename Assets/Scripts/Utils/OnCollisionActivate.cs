using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OnCollisionActivate : MonoBehaviour
{
    [SerializeField] private bool collectChildObjs = false;
    [SerializeField] private List<GameObject> objsToAcivate;

    private void Start()
    {
        if (collectChildObjs)
        {
            GameObject[] children = GetComponentsInChildren<Transform>(true)
                .Where(t => t != transform)
                .Select(t => t.gameObject)
                .ToArray();

            objsToAcivate.AddRange(children);
        }
            
    }

    private void OnTriggerEnter(Collider other)
    {
        // If object has the flag, activate the objects
        if (other.GetComponent<Triggerable>() != null)
        {
            foreach(GameObject obj in objsToAcivate)
                obj.SetActive(true);
        }

        gameObject.SetActive(false);
    }
}
