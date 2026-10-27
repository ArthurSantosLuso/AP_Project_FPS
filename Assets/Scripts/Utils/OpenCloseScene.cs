using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenCloseScene : MonoBehaviour
{
    [SerializeField, SceneDropdown] private string sceneToOpen;
    public void OpenScene()
    {
        SceneManager.LoadScene(sceneToOpen);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
