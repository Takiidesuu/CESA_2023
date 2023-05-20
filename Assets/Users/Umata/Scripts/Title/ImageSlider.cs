using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ImageSlider : MonoBehaviour
{
    public Image[] images;  // �R��Image���i�[����z��
    public int select_button = 0;  // �I�𒆂̃{�^���̃C���f�b�N�X
    public int select_distance = 50;  // �{�^�������炷����
    public float select_delay = 0.3f; // �{�^���I���̍X�V�𐧌����鎞��

    public bool is_firsttime = true; // ����N�����ǂ����̃t���O

    public string scene_start_name; //�ŏ��̃V�[����
    public string scene_continue_name; //�����̃V�[����
    public string scene_option_name; //�I�v�V�����̃V�[����


    public Image image_hammer; // �ǉ���Image Hammer
    public Image image_banner; // �ǉ���Image Banner


    public float hammer_image_distance;  // �������W��ۑ�����z��

    private float[] init_positions;  // �������W��ۑ�����z��
    private float timeSinceSelect = 0f; // �O��̃{�^���I������̎���
    private bool canSelect = true; // �{�^���I�����\���ǂ����̃t���O

    private SoundManager soundManager;
    
    void Start()
    {
        // �������W��z��ɕۑ�
        init_positions = new float[images.Length];
        for (int i = 0; i < images.Length; i++)
        {
            init_positions[i] = images[i].rectTransform.anchoredPosition.x;
        }

        // ������Ԃ̃{�^����I��
        SetSelectedButton(select_button);

        soundManager = GetComponent<SoundManager>();
    }

    void Update()
    {
        //�V�[���ؑ�
        if (InputManager.instance.press_select)
        {
            soundManager.PlaySoundEffect("OK");
            
            // �X�y�[�X�L�[�܂��̓W�����v�{�^���������ꂽ�Ƃ��̏���
            switch (select_button){
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
        // ����N���̏ꍇ�AadditionalImage�̃A���t�@�l��255�ɐݒ�
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

        // �L�[�{�[�h����őI�𒆂̃{�^����ύX
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

        // �{�^���I���̍X�V���������Ԃ𒴂�����t���O�𗧂Ă�
        if (timeSinceSelect >= select_delay)
        {
            canSelect = true;
        }

        // �I�𒆂̃{�^�����ړ�
        SetSelectedButton(select_button);
    }

    // �I�𒆂̃{�^�����ړ�����֐�
    void SetSelectedButton(int index)
    {
        for (int i = 0; i < images.Length; i++)
        {
            // �������W����I�𒆂̃{�^���̋����ɉ����āAX���W�����炷
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
}