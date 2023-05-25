using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ImageSlider : MonoBehaviour
{
    public Image[] images; 
    public int select_button = 0; 
    public int select_distance = 50;  
    public float select_delay = 0.3f; 

    public bool is_firsttime = true;

    public string scene_start_name; 
    public string scene_continue_name;
    public string scene_option_name;


    public Image image_hammer;
    public Image image_banner; 

    public float hammer_image_distance; 

    private float[] init_positions; 
    private float timeSinceSelect = 0f;
    private bool canSelect = true; 

    private SoundManager soundManager;

    [SerializeField] Color color1 = Color.white, color2 = Color.white;
    [SerializeField] UnityEngine.UI.Image image = null;

    [SerializeField]
    CanvasGroup group = null;

    [SerializeField]
    Fade fade = null;

    void Start()
    {
       
        init_positions = new float[images.Length];
        for (int i = 0; i < images.Length; i++)
        {
            init_positions[i] = images[i].rectTransform.anchoredPosition.x;
        }

       
        SetSelectedButton(select_button);

        soundManager = GetComponent<SoundManager>();
    }

    void Update()
    {
       
        if (InputManager.instance.press_select)
        {
            soundManager.PlaySoundEffect("OK");

            group.blocksRaycasts = false;
            fade.FadeIn(1, () =>
            {

            });

            Invoke("ChangeScene", 1);
        }
       
        if (!is_firsttime)
        {
            Color color = images[1].color;
            color.a = 1;
            images[1].color = color;
        }
        else
        {
            Color color = images[1].color;
            color.a = 0.5f;
            images[1].color = color;
        }

        timeSinceSelect += Time.deltaTime;

       
        if (canSelect && InputManager.instance.GetMenuMoveFloat() < 0)
        {
            soundManager.PlaySoundEffect("Cursor");
            timeSinceSelect = 0f;
            canSelect = false;
            select_button--;
            if (select_button < 0)
            {
                select_button = images.Length - 1;
            }
        }
        else if (canSelect && InputManager.instance.GetMenuMoveFloat() > 0)
        {
            soundManager.PlaySoundEffect("Cursor");
            timeSinceSelect = 0f;
            canSelect = false;
            select_button++;
            if (select_button >= images.Length)
            {
                select_button = 0;
            }
        }

       
        if (timeSinceSelect >= select_delay)
        {
            canSelect = true;
        }

        SetSelectedButton(select_button);
    }

   
    void SetSelectedButton(int index)
    {
        for (int i = 0; i < images.Length; i++)
        {
          
            float x = init_positions[i];
            if (i == index)
            {
                x -= select_distance;
            }

            images[i].rectTransform.anchoredPosition = new Vector2(x, images[i].rectTransform.anchoredPosition.y);
            image_hammer.rectTransform.anchoredPosition = new Vector2(images[select_button].rectTransform.anchoredPosition.x - hammer_image_distance, images[select_button].rectTransform.anchoredPosition.y);
            image_banner.rectTransform.anchoredPosition = new Vector2(images[select_button].rectTransform.anchoredPosition.x, images[select_button].rectTransform.anchoredPosition.y);

        }
    }

     void ChangeScene()
    {
        switch (select_button)
        {
            case 0:
                SceneManager.LoadScene(scene_start_name);
                break;
            case 1:
                SceneManager.LoadScene(scene_continue_name);
                break;

            case 2:
                SceneManager.LoadScene(scene_option_name);
                break;
        }
    }
}