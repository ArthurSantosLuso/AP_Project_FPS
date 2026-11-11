using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartTrigger : MonoBehaviour
{
    public int sceneIdx = 0;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<FirstPersonMovement>()!= null) SceneManager.LoadScene(sceneIdx);
    }
}