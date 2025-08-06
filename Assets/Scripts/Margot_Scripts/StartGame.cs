using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public void StartGameBtn()
    {
        SceneManager.LoadScene(1);
    }
}
