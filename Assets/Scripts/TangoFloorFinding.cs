using UnityEngine;
using System.Collections;
using System.IO;
using Tango;
using UnityEngine.SceneManagement;

public class TangoFloorFinding : MonoBehaviour
{

    //marker for found
    public GameObject m_foundmarker;
    //marker for not found
    public GameObject m_notfoundmarker;

    public Transform[] m_brick;
    private  int index = 0;

    private bool foundStatue = false;

    //private bool m_findingFloor = false;


    private TangoApplication m_tangoApplication;

    private TangoPointCloud m_pointCloud;

    private TangoPointCloudFloor m_pointCloudFloor;


    private bool hasAddObj = false;

    private GameObject tmpchair = null;

    private bool isTest = true;
    //private int NFrame = 15;
    //private int testEveryNFrame = 10;
    private float timeStep = 0.25f;
    private float curtime = 0f;
    private int curNum = 0;
    public float speed = 0.1F;
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
       
        //tmpchair = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //if (Application.platform == RuntimePlatform.Android)
        //{
        index = -1;
        getIndexFromAndroid();
        if (index < 0 || index > m_brick.Length)
        {
            Application.Quit();
        }
        tmpchair = Instantiate(m_brick[index].gameObject, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        tmpchair.AddComponent<TranslateByTouch>();
        tmpchair.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        tmpchair.transform.rotation = new Quaternion(0f, 180f, 0f, 0f);
        tmpchair.SetActive(false);

        m_foundmarker.SetActive(foundStatue);
        m_notfoundmarker.SetActive(!foundStatue);
        m_pointCloud = FindObjectOfType<TangoPointCloud>();
        m_pointCloudFloor = FindObjectOfType<TangoPointCloudFloor>();
        m_tangoApplication = FindObjectOfType<TangoApplication>();
        //}

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
        if (!hasAddObj)
        {
            //isTest = !isTest;
            //if (!isTest)
            //{
            //    return;
            //}

            //AndroidHelper.ShowAndroidToastMessage(string.Format("Floor finding."));
            findFloor();
            //AndroidHelper.ShowAndroidToastMessage(foundStatue.ToString(), AndroidHelper.ToastLength.SHORT);
            //if (foundStatue = true && Input.touchCount > 0)
            //{
            //    hasAddObj = true;
            //}

        }
        if (hasAddObj)
        {
            changeObjStatueByTouch();
        }
    }


    void findFloor()
    {
        //AndroidHelper.ShowAndroidToastMessage(string.Format("Floor finding."));
        //m_findingFloor = true;
        //if (m_pointCloud == null)
        //{
        //    Debug.LogError("TangoPointCloud required to find floor.");
        //    return;
        //}
        curtime -= Time.deltaTime;
        if (curtime > 0)
        {
            return;
        }

        curtime = timeStep;
       
        m_tangoApplication.SetDepthCameraRate(TangoEnums.TangoDepthCameraRate.MAXIMUM);
        
        //Debug.Log(m_pointCloudFloor.m_floorFound.ToString() + " " + m_pointCloud.m_floorFound.ToString());
        //AndroidHelper.ShowAndroidToastMessage(string.Format("Floor find over.") + m_pointCloudFloor.m_floorFound.ToString() + " " + m_pointCloud.m_floorFound.ToString());
        if (m_pointCloudFloor.m_floorFound && m_pointCloud.m_floorFound)
        {
            
            System.Console.WriteLine(m_pointCloudFloor.m_floorFound.ToString() + " " + m_pointCloud.m_floorFound.ToString());
            foundStatue = true;
            showMarker();
            makeMarkerPos(m_foundmarker);
            //AndroidHelper.ShowAndroidToastMessage(string.Format("Floor found."), AndroidHelper.ToastLength.SHORT);
        }
        else
        {
            System.Console.WriteLine(m_pointCloudFloor.m_floorFound.ToString() + " " + m_pointCloud.m_floorFound.ToString());
            foundStatue = false;
            showMarker();
            makeMarkerPos(m_notfoundmarker);
            //<size=30>Searching for floor position. Make sure the floor is visible.</size>
            //AndroidHelper.ShowAndroidToastMessage("Searching for floor position. Make sure the floor is visible.", AndroidHelper.ToastLength.SHORT);
        }
        m_pointCloud.FindFloor();

        if (foundStatue)
        {
            ++curNum;
        }
        else
        {
            curNum = 0;
        }
        if (curNum > 1.5 / timeStep)
        {
            hasAddObj = true;
        }
       
        //m_findingFloor = false;
    }

    void makeMarkerPos(GameObject marker)
    {
        Vector3 target;
        RaycastHit hitInfo;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f)), out hitInfo))
        {
            // Limit distance of the marker position from the camera to the camera's far clip plane. This makes sure that the marker
            // is visible on screen when the floor is found.
            Vector3 cameraBase = new Vector3(Camera.main.transform.position.x, hitInfo.point.y, Camera.main.transform.position.z);
            target = cameraBase + Vector3.ClampMagnitude(hitInfo.point - cameraBase, Camera.main.farClipPlane * 0.9f);
        }
        else
        {
            // If no raycast hit, place marker in the camera's forward direction.
            Vector3 dir = new Vector3(Camera.main.transform.forward.x, 0.0f, Camera.main.transform.forward.z);
            target = dir.normalized * (Camera.main.farClipPlane * 0.9f);
            target.y = m_pointCloudFloor.transform.position.y ;
        }
        target.y -= 0.2f;
        marker.transform.position = target;
    }

    void showMarker()
    {
        m_foundmarker.SetActive(foundStatue);
        m_notfoundmarker.SetActive(!foundStatue);
    }


    void changeObjStatueByTouch()
    {
        
        if (m_foundmarker.activeSelf)
        {
            //m_pointCloud.StopAllCoroutines();
            //m_pointCloudFloor.StopAllCoroutines();
            m_pointCloudFloor.m_turnOffDepthCamera = true;
            tmpchair.transform.position = m_foundmarker.transform.position;
            tmpchair.SetActive(true);
            m_foundmarker.SetActive(false);
            m_notfoundmarker.SetActive(false);
            //m_tangoApplication.Shutdown();
            //AndroidHelper.ShowAndroidToastMessage(string.Format("create chair") + m_foundmarker.activeSelf.ToString(), AndroidHelper.ToastLength.SHORT);
        }
        
        //m_notfoundmarker.SetActive(false);
        //tmpchair.transform.position = m_foundmarker.transform.position;
        //tmpchair.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        //m_foundmarker.SetActive(false);
        //tmpchair.SetActive(true);
        //AndroidHelper.ShowAndroidToastMessage(string.Format("changeObjStatueByTouch") + m_foundmarker.activeSelf.ToString(), AndroidHelper.ToastLength.SHORT);

    }
}
