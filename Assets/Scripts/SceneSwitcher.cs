using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void switchToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
