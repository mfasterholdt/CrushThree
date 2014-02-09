using UnityEngine;

public class FadeInOut : MonoBehaviour
{
	int drawDepth = -1000;

	public Texture2D fadeOutTexture;
	public float fadeSpeed = 0.3f;
	private float alpha = 1.0f; 
	private float fadeDir = -1;
	
	void Start()
	{
		alpha = 1;
		fadeIn();
	}

	void OnGUI()
	{
		Color nextColor = GUI.color;

		alpha += fadeDir * fadeSpeed * Time.deltaTime;	
		
		alpha = Mathf.Clamp01(alpha);	

		nextColor.a = alpha;

		GUI.color = nextColor;
		GUI.depth = drawDepth;
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);
	}

	void fadeIn()
	{
		fadeDir = -1;	
	}

	void fadeOut()
	{
		fadeDir = 1;	
	}
}