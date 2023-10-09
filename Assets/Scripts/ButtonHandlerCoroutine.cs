using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Text;


public class ButtonHandlerCoroutine : MonoBehaviour
{
    public TextMeshProUGUI respuesta;
    public TMP_InputField mensaje;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Upload());
    }

    IEnumerator Upload()
    {
        string bodyRaw = "{\"prompt\":" + "\"" + mensaje.text + "\"}";
        UnityWebRequest www = UnityWebRequest.Post("http://192.168.3.3:8060/chat", bodyRaw);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            respuesta.text = www.downloadHandler.text;
            Debug.Log("Form upload complete!");
        }
    }
}
