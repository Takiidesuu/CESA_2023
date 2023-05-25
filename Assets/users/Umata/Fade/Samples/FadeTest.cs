using UnityEngine;
using System.Collections;

public class FadeTest : MonoBehaviour 
{
	private bool isMainColor = false;
	[SerializeField] Color color1 = Color.white, color2 = Color.white;
	[SerializeField] UnityEngine.UI.Image image = null;

	[SerializeField]
	CanvasGroup group = null;

	[SerializeField]
	Fade fade = null;

	public void Fadeout()
	{
		group.blocksRaycasts = false;
		fade.FadeIn (1, () =>
		{
			image.color = (isMainColor) ? color1 : color2;
			isMainColor = !isMainColor;
			
		});
	}
}
