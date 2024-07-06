using UnityEngine;
using System.IO;
using UnityEngine.UI;
using SFB;
public class CharacterTextureManager : MonoBehaviour
{
    [SerializeField] Button loadImageButton;
    private void Start()
    {
        loadImageButton.onClick.AddListener(()=>{ OpenFileExplorer(); });
    }

    public void OpenFileExplorer()
    {
        // Dosya seçme iþlemini baþlatýn
        var extensions = new[] { new ExtensionFilter("Image Files", "png", "jpg", "jpeg") };
        var paths = StandaloneFileBrowser.OpenFilePanel("Select Texture", "", extensions, false);

        // Seçilen dosya yolu varsa, texture'ý yükle
        if (paths != null && paths.Length > 0)
        {
            string filePath = paths[0];
            LoadLocalTexture(filePath);
        }
    }

    private void LoadLocalTexture(string filePath)
    {
        // Texture dosyasýný okuyun
        byte[] textureData = File.ReadAllBytes(filePath);

        PlayerVisualUI.Instance.SetFaceTexture(textureData);
    }
}
