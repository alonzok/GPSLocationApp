using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Google.XR.ARCoreExtensions;
using System;
using UnityEngine.Networking;
using TMPro;

public class VPSManager : MonoBehaviour
{

    [SerializeField]private AREarthManager earthManager;

    [Serializable]
    public struct EarthPosition{
        public double Latitude;
        public double Longitude;
        public double Altitude;
    }
    
    [Serializable]
    public struct GeospatialObject{
        public GameObject ObjectPrefab;
        public EarthPosition EarthPosition;
    }

    [Serializable]
    public class AugmentedEntity{
        public string id;
        public string idAutor;
        public string descripcion;
        public string idEcosistema;
        public double longitud;
        public double latitud;
        public double altitud;
    }

    [Serializable]
    public class RootAugmentedEntity
    {
        public AugmentedEntity[] augmentedEntities;
    }

    [SerializeField] private ARAnchorManager ARAnchorManager;
    [SerializeField] public List<GeospatialObject> geospatialObjects = new List<GeospatialObject>();

    public TextMeshPro resultado;
    public TextMeshPro otroResultado;
    public RootAugmentedEntity rootAugmentedEntity;
    public GameObject ObjectPrefab;

    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(getEntities());
        verifyGeospatialSupport();
    }

    private void verifyGeospatialSupport(){
        var result = earthManager.IsGeospatialModeSupported(GeospatialMode.Enabled);
        switch(result){
            case FeatureSupported.Supported:
                Debug.Log("Ready to use VPS");
                // resultado.text = "Empezando";
                placeObjects();
                break;

            case FeatureSupported.Unknown:
                Debug.Log("Unknown...");
                Invoke("verifyGeospatialSupport", 5.0f);
                break;

            case FeatureSupported.Unsupported:
                Debug.Log("VPS Unsupported");
                break;

        }
    }

    private void placeObjects(){
        if (earthManager.EarthTrackingState == TrackingState.Tracking){

            
            
            foreach( var obj in geospatialObjects){

                var earthPosition = obj.EarthPosition;
                var modelAnchor = ARAnchorManagerExtensions.AddAnchor(ARAnchorManager, earthPosition.Latitude, earthPosition.Longitude, earthPosition.Altitude, Quaternion.identity);
                // obj.ObjectPrefab.active = true;
                Instantiate(obj.ObjectPrefab, modelAnchor.transform);

            }

            foreach( var entity in rootAugmentedEntity.augmentedEntities)
            {
                var modelAnchor = ARAnchorManagerExtensions.AddAnchor(ARAnchorManager, entity.latitud, entity.longitud, entity.altitud, Quaternion.identity);
                Instantiate(ObjectPrefab, modelAnchor.transform);
            }

        } else if (earthManager.EarthTrackingState == TrackingState.None){
            Invoke("placeObjects", 0.5f);
            // resultado.text = "Parado";
        }
    }

    private IEnumerator getEntities() {
        UnityWebRequest www = UnityWebRequest.Get("http://192.168.3.3:8070/getEntityByEcosistema/63c992b466067a7760322a76");
        yield return www.SendWebRequest();
 
        if(www.isNetworkError) {
            resultado.text = www.error;
        }
        else {
            // Show results as text

            rootAugmentedEntity = JsonUtility.FromJson<RootAugmentedEntity>("{\"augmentedEntities\":" + www.downloadHandler.text + "}");
            resultado.text = www.downloadHandler.text;
            Debug.Log(www.downloadHandler.text);

        }
    }

    // private IEnumerator getOtherEntities(){
    //     using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
    //     {
    //         // Request and wait for the desired page.
    //         yield return webRequest.SendWebRequest();

    //         string[] pages = uri.Split('/');
    //         int page = pages.Length - 1;

    //         switch (webRequest.result)
    //         {
    //             case UnityWebRequest.Result.ConnectionError:
    //             case UnityWebRequest.Result.DataProcessingError:
    //                 Debug.LogError(pages[page] + ": Error: " + webRequest.error);
    //                 break;
    //             case UnityWebRequest.Result.ProtocolError:
    //                 Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
    //                 break;
    //             case UnityWebRequest.Result.Success:
    //                 Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
    //                 break;
    //         }
    //     }
    // }

}
