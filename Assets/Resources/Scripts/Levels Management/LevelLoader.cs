using UnityEngine;


public class LevelLoader : MonoBehaviour
{
    [SerializeField] private string sceneName;
    

    public void Load()
    {
        AsyncLoader.LoadScene(sceneName);
    }
}
