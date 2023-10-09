using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Text;
using Unity.VisualScripting;

public class ButtonHandler : MonoBehaviour
{
    public Button btnClick;
    public TextMeshProUGUI respuesta;
    public TMP_InputField mensaje;

    public void Start()
    {
        btnClick.onClick.AddListener(Chat);
    }

    public void SetText()
    {
        respuesta.text = mensaje.text;
        //string inputText = GameObject.Find("InputText").GetComponent<InputField>().text;
        //Debug.Log(GameObject.Find("InputText").GetComponent<InputField>().text);
        //Text outputText = GameObject.Find("OutputText").GetComponent<Text>();
        //outputText.text = inputText;
    }

async void Chat()
    {
        var request = new UnityWebRequest("http://192.168.3.3:8060/chat", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes("{\"prompt\":" + "\"" +mensaje.text + "\"}");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SendWebRequest();
        while (!request.isDone)
        {

        }
        Debug.Log("Status Code: " + request.result);
        respuesta.text = request.downloadHandler.text;
    }
}
