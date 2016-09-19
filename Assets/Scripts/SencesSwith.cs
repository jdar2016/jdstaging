using UnityEngine;
using System.Collections;
using System.IO;
using Tango;
using UnityEngine.SceneManagement;

public class SencesSwith : MonoBehaviour {
	private readonly string[] m_sceneNames =
	{
		"findfloor",
		"3DShow"
	};

	// Use this for initialization
	void Start()
	{
		AndroidJavaObject jo = new AndroidJavaObject("com.jd.staging.ShareWithUnity");
		int _index = jo.CallStatic<int>("is3D");
		//int _index = 1;
#pragma warning disable 618
		//Application.LoadLevel(m_sceneNames[1]);
		//SceneManager.LoadScene(1);
		SceneManager.LoadScene(m_sceneNames[_index]);
#pragma warning restore 618

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
