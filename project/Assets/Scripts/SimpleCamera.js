#pragma strict

var cubeObject : GameObject;

private var webcamTexture : WebCamTexture;
private var texture : Texture2D;
private var data : Color32[];

private var initDone : boolean = false;

function Start()
{

	webcamTexture = WebCamTexture(8, 8, 12);
    webcamTexture.Play();

	while(1)
    {
    	if (webcamTexture.didUpdateThisFrame)
    	{
    		data = new Color32[webcamTexture.width * webcamTexture.height];

    		texture = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.RGB565, false);

    		cubeObject.GetComponent.<Renderer>().material.SetTexture("_MainTex", texture);

    		initDone = true;

    		break;
    	}
    	else
    	{
    		yield;
    	}
    }
}

function Update()
{
	if (initDone && webcamTexture.didUpdateThisFrame)
	{
		webcamTexture.GetPixels32(data);

		texture.SetPixels32(data);
		texture.Apply();
	}
}
