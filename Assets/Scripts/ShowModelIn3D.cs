using UnityEngine;
using System.Collections;
using System.IO;

public class ShowModelIn3D : MonoBehaviour
{

    public Transform[] m_brick;
    private int index = 0;
    public float scale = 0.5f;


    private GameObject tmpchair = null;

    void getIndexFromAndroid()
    {
        //AndroidJavaClass jc = new AndroidJavaClass("com.jd.jdstaging");
        //AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("getIndex");
        //index = jo.Call<int>("getIndex");
        AndroidJavaObject jo = new AndroidJavaObject("com.jd.staging.ShareWithUnity");
        index = jo.CallStatic<int>("getModelIndex");
    }

    // Use this for initialization
    void Start()
    {
        index = -1;
        getIndexFromAndroid();
        if (index < 0 || index > m_brick.Length)
        {
            Application.Quit();
        }
        tmpchair = Instantiate(m_brick[index].gameObject, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        tmpchair.AddComponent<TranslateByTouchIn3D>();
        tmpchair.transform.localScale = new Vector3(scale, scale, scale);
        tmpchair.transform.rotation = new Quaternion(0f, 180f, -40f, 0f);//旋转180

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            jo.Call("onBackPressed");
            return;
        }

    }

}