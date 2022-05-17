using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class LoadTexture : MonoBehaviour
{
    public static List<Texture> textures = new List<Texture>();

    public void AddTexture(string textureName) {
        foreach(Texture texture in textures) {
            if(texture.name==textureName)
                textures.Remove(texture);
        }
        StartCoroutine(GetTexture(textureName, action));
        static void action(Texture texture) {
            textures.Add(texture);
        }
    }


    private IEnumerator GetTexture(string textureName, Action<Texture> action) {
        string path = Application.dataPath + "/Art/Images/" + textureName + ".png";
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(path);

        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
        } else {
            Texture texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            texture.name = textureName;
            action(texture);
        }
    }
}
