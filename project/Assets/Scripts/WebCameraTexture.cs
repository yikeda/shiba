using UnityEngine;
using System.Collections;

public class WebCameraTexture : MonoBehaviour {

		public int Width = 1920;
		public int Height = 1080;
		public int FPS = 30;
		
		void Start () {
			
			var euler = transform.localRotation.eulerAngles;
			transform.localRotation = Quaternion.Euler( euler.x, euler.y, euler.z - 90 );
			
			var devices = WebCamTexture.devices;
			if (devices.Length > 0)
			{
				int cameraIndex = devices.Length - 1;
				var webcamTexture = new WebCamTexture(devices[cameraIndex].name,Width, Height, FPS);
				GetComponent<Renderer>().material.mainTexture = webcamTexture;
				webcamTexture.Play();
			}else
			{
				Debug.Log("no camera");
				return;
			}
		}
	}

