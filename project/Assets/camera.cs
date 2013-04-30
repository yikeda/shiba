using UnityEngine;
using System;
using System.Collections;

public class camera : MonoBehaviour {
    const int GridSize = 16;

	WebCamDevice[] devices;
	WebCamTexture webcamTexture;
	Texture2D smallTexture;
	Color32[] basePixels;
	Color32[] displayPixels;
	int[] grayscalePixels;
	int[] grayscalePixelsPrev;
	int[] grayscalePixelsCount;

	void Start () {
		GameObject plane1 = GameObject.CreatePrimitive(PrimitiveType.Plane);
		GameObject plane2 = GameObject.CreatePrimitive(PrimitiveType.Plane);
		plane1.transform.position = new Vector3(0, 5.0f, 0);
		plane1.transform.Rotate(-90,0,0);
		plane2.transform.position = new Vector3(0, -5.0f, 0);
		plane2.transform.Rotate(-90,0,0);

		devices  = WebCamTexture.devices;
		if (devices.Length > 0) {
			webcamTexture  = new WebCamTexture(devices[1].name, 128, 128, 12);
			smallTexture = new Texture2D(GridSize,GridSize);
			plane1.renderer.material.mainTexture = smallTexture;
            plane1.renderer.material.mainTexture.filterMode = FilterMode.Point;
            plane1.renderer.material.shader = Shader.Find("Unlit/Texture");

			plane2.renderer.material.mainTexture = webcamTexture;
			plane2.renderer.material.mainTexture.filterMode = FilterMode.Point;
            plane2.renderer.material.shader = Shader.Find("Unlit/Texture");

			webcamTexture.Play();
			basePixels = new Color32[webcamTexture.width * webcamTexture.height];
			displayPixels = new Color32[GridSize * GridSize];
			grayscalePixels = new int[GridSize * GridSize];
			grayscalePixelsPrev = new int[GridSize * GridSize];
			grayscalePixelsCount = new int[GridSize * GridSize];

			Debug.Log(webcamTexture.width);
			Debug.Log(webcamTexture.height);
		} else {
		}
	}

	void Update () {
		webcamTexture.GetPixels32(basePixels);
		grayscalePixels.CopyTo(grayscalePixelsPrev, 0);

		for (int x = 0; x < GridSize; x++) {
			for (int y = 0; y < GridSize; y++) {
				int index = x + GridSize * y;
				grayscalePixels[index] = 0;
				grayscalePixelsCount[index] = 0;
			}
		}

		for (int x = 0; x < webcamTexture.width; x++) {
			for (int y = 0; y < webcamTexture.height; y++) {
				Color32 color = basePixels[x + webcamTexture.width * y];
				int small_x = x / (webcamTexture.width / GridSize);
				int small_y = y / (webcamTexture.height / GridSize);
				int index = small_x + GridSize * small_y;
				int gray = (int)(0.299 * (int)color.r + 0.587 * (int)color.g + 0.114 * (int)color.b);
				grayscalePixels[index] += gray;
				grayscalePixelsCount[index]++;
			}
		}

		for (int x = 0; x < GridSize; x++) {
			for (int y = 0; y < GridSize; y++) {
				int index = x + GridSize * y;
				byte gray = (byte)(grayscalePixels[index] / grayscalePixelsCount[index]);
				displayPixels[index].r = gray;
				displayPixels[index].g = gray;
				displayPixels[index].b = gray;
				displayPixels[index].a = (byte)1;
			}
		}

		smallTexture.SetPixels32(displayPixels);
		smallTexture.Apply();
	}
}
