using Margot;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // GameObject 전체를 유지
        }
        else
        {
            Destroy(gameObject); // 중복된 인스턴스 제거
        }
    }
    #endregion

    public InteractionManager interactionManager;
    public AudioManager audioManager;
    public LoopManager loopManager;
    public CutSceneManager cutsceneManager;
}
