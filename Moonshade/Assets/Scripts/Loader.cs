using Photon.Pun;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene
    {
        MainMenuScene,
        LobbyScene,
        Level_1,
        Level_2,
        Level_3,
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