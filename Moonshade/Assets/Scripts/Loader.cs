using Photon.Pun;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene
    {
        MainMenuScene,
        LobbyScene,
        GameScene,
        LoadingScene,
    }

    public static void Load(Scene scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }
    public static void LoadNetwork(Scene scene)
    {
        PhotonNetwork.LoadLevel((int)scene);
    }
}