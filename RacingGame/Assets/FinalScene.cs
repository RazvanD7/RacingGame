using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void PlayAgain()
    {
        SceneManager.LoadSceneAsync(1);
    }
    public void MainMeniu()
    {
        SceneManager.LoadSceneAsync(0);
    }
}
