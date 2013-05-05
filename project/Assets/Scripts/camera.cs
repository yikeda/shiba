using UnityEngine;
using System;
using System.Collections;

public class camera : MonoBehaviour {
    const int GridSize = 16;
    const int BufferSize = 3;

    bool isInitialized;
	WebCamDevice[] devices;
	WebCamTexture webcamTexture;
	Texture2D smallTexture;
	Color32[] basePixels;
	Color32[] displayPixels;
	int[] grayscalePixels;
	int[] grayscalePixelsCount;
    int[,] pixelsBuffer;
    int bufferIndex;
	int[] diffPixels;

	void Start () {
        isInitialized = false;
		devices  = WebCamTexture.devices;
		if (devices.Length > 0) {
            int cameraIndex = devices.Length - 1;
			webcamTexture  = new WebCamTexture(devices[cameraIndex].name, 128, 128, 60);
			webcamTexture.Play();
		} else {
		}
	}

    void InitializeTexture ()
    {
        if (webcamTexture != null &&  webcamTexture.didUpdateThisFrame) {
            GameObject plane1 = GameObject.CreatePrimitive(PrimitiveType.Plane);
            GameObject plane2 = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane1.transform.position = new Vector3(0, 5.0f, 0);
            plane1.transform.Rotate(-90,0,0);
            plane2.transform.position = new Vector3(0, -5.0f, 0);
            plane2.transform.Rotate(-90,0,0);

            smallTexture = new Texture2D(GridSize,GridSize);
            basePixels = new Color32[webcamTexture.width * webcamTexture.height];
            displayPixels = new Color32[GridSize * GridSize];
            grayscalePixels = new int[GridSize * GridSize];
            grayscalePixelsCount = new int[GridSize * GridSize];
            pixelsBuffer = new int[BufferSize,GridSize * GridSize];
            diffPixels = new int[GridSize * GridSize];

            plane1.renderer.material.mainTexture = smallTexture;
            plane1.renderer.material.mainTexture.filterMode = FilterMode.Point;
            plane1.renderer.material.shader = Shader.Find("Unlit/Texture");

            plane2.renderer.material.mainTexture = webcamTexture;
            //plane2.renderer.material.mainTexture.filterMode = FilterMode.Point;
            //plane2.renderer.material.shader = Shader.Find("Unlit/Texture");

            Debug.Log(webcamTexture.width);
            Debug.Log(webcamTexture.height);

            isInitialized = true;
        }
    }

	void Update () {
        if (!isInitialized) {
            InitializeTexture();
            return;
        }

        if (!webcamTexture.didUpdateThisFrame)
            return;

        webcamTexture.GetPixels32(basePixels);

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
                pixelsBuffer[bufferIndex,index] = grayscalePixels[index] / grayscalePixelsCount[index];
            }
        }


        for (int x = 0; x < GridSize; x++) {
			for (int y = 0; y < GridSize; y++) {
                int index = x + GridSize * y;

                int diff = 0;
                for (int i = 0; i < BufferSize - 1; i++) {
                    diff += Math.Abs(pixelsBuffer[bufferIndex,index] - pixelsBuffer[(bufferIndex + i) % BufferSize, index]);
                }
                diffPixels[index] = diff;
            }
        }

		for (int x = 0; x < GridSize; x++) {
			for (int y = 0; y < GridSize; y++) {
				int index = x + GridSize * y;
				//byte gray = (byte)(pixelsBuffer[bufferIndex,index]);
                byte gray = (byte)(diffPixels[index]);
				displayPixels[index].r = gray;
				displayPixels[index].g = gray;
				displayPixels[index].b = gray;
				displayPixels[index].a = (byte)1;
			}
		}

        bufferIndex = ++bufferIndex % BufferSize;

		smallTexture.SetPixels32(displayPixels);
		smallTexture.Apply();
	}
}
