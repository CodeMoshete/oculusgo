using UnityEngine.SceneManagement;

public class ChangeSceneAction : CustomAction
{
    public string NextSceneName;

    public override void Initiate()
    {
        SceneManager.LoadScene(NextSceneName);
    }
}
