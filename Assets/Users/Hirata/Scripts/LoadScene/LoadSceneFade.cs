using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadSceneFade : MonoBehaviour
{
    [System.Serializable]
    public class Textures {
        public List<Texture2D> StageTextures = new List<Texture2D>();
    }

    public enum MOVE
    {
        NOT,
        MOVEING,
        COMPLETION,
    }

    public Canvas canvas;           //表示するキャンバス
    [SerializeField] public List<Textures> WorldTextures = new List<Textures>();       //表示する画像

    public int rows = 9;              //縦分割
    public int columns = 16;           //横分割
    public float MoveTime = 1;             //移動速度
    public float CreatTime = 0.5f;      //生成スピード

    private Vector2[,] StartPos;        //始まった場所
    private Vector2[,] split_position;  //分割の場所
    private MOVE[,] split_is_move;      //その場所にい移動したか
    private Sprite[,] load_split;       //元の画像から分割した後の画像
    private GameObject[,] gameObjects;  //Imageとして生成したゲームオブジェクト
    private float[,] StartTime;              //始まった時間

    private bool Is_move = true;        //現在画像を表示させているか
    private bool Is_Coroutine = false;  //コルーチンを通ったか
    private int MaxSplite;              //全ての分割数
    private int MoveCount;              //動いた数
    private int EndCount;               //現在正しい位置に表示された場所

    // Start is called before the first frame update
    void Start()
    {
        MaxSplite = rows * columns;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Is_move)
        {
            Vector2 random = new Vector2(Random.Range(0, columns), Random.Range(0, rows));

                switch (split_is_move[(int)random.y, (int)random.x])
                {
                    case MOVE.NOT:
                        split_is_move[(int)random.y, (int)random.x] = MOVE.MOVEING;
                        StartTime[(int)random.y, (int)random.x] = Time.time;
                        Is_move = true;
                        MoveCount++;
                        break;

                    case MOVE.MOVEING:
                        //float x = split_position[(int)random.y, (int)random.x].x;
                        //float y = split_position[(int)random.y, (int)random.x].y;
                        //gameObjects[(int)random.y, (int)random.x].transform.position = new Vector2();

                        //指定の場所に徐々に移動
                        //gameObjects[(int)random.y, (int)random.x].transform.position =
                        //    Vector3.Lerp(gameObjects[(int)random.y, (int)random.x].transform.position, split_position[(int)random.y, (int)random.x], MoveSpead * Time.deltaTime);

                        //if (gameObjects[(int)random.y, (int)random.x].transform.position == (Vector3)split_position[(int)random.y, (int)random.x])
                        //{
                        //    NowCount++;
                        //    split_is_move[(int)random.y, (int)random.x] = MOVE.COMPLETION;
                        //}
                        //Is_Not = false;
                        break;

                    case MOVE.COMPLETION:
                        random.x++;
                        if (random.x > columns - 1)
                        {
                            random.x = 0;
                            random.y++;
                            if (random.y > rows - 1)
                                random.y = 0;
                        }
                        break;
                }
        }
        if (split_is_move != null)
        {
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    if (split_is_move[y, x] == MOVE.MOVEING)
                    {
                        float t = Mathf.Clamp01((Time.time - StartTime[y, x]) / MoveTime);
                        gameObjects[y, x].transform.position = Vector3.Lerp(StartPos[y, x], split_position[y, x], t);

                        if (t >= 1)
                        {
                            EndCount++;
                            split_is_move[y, x] = MOVE.COMPLETION;
                        }
                    }
                }
            }
        }

    }

    public float SpliteOnceMove(float progress)
    {
        if (Is_move)
        {
            if (!Is_Coroutine)
            {
                if (EndCount == 0)
                    StartCoroutine(WaitMove(CreatTime));
                else if (EndCount / MaxSplite < progress - 0.1f)
                    StartCoroutine(WaitMove(CreatTime));
                Is_Coroutine = true;
            }
        }

        if (EndCount == 0)
            return 0;
        else
            return EndCount / MaxSplite;
    }

    private IEnumerator WaitMove(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Is_move = false;
        Is_Coroutine = false;
    }

    public void SetTexture(int world,int stage)
    {
        //画像を分割
        int width = WorldTextures[world].StageTextures[stage].width;
        int height = WorldTextures[world].StageTextures[stage].height;
        int tileWidth = width / columns;
        int tileHeight = height / rows;
        load_split = new Sprite[rows, columns];
        StartPos = new Vector2[rows, columns];
        split_position = new Vector2[rows, columns];
        split_is_move = new MOVE[rows, columns];
        gameObjects = new GameObject[rows, columns];
        StartTime = new float[rows, columns];
        int i = 0;
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Color[] pixels = WorldTextures[world].StageTextures[stage].GetPixels(x * tileWidth, y * tileHeight, tileWidth, tileHeight);
                Texture2D tile = new Texture2D(tileWidth, tileHeight);
                tile.SetPixels(pixels);
                tile.Apply();
                load_split[y, x] = Sprite.Create(tile, new Rect(0, 0, tileWidth, tileHeight), Vector2.zero);
            }
        }

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                GameObject gameObject = new GameObject();
                gameObject.transform.parent = canvas.transform;
                Image image = gameObject.AddComponent<Image>();
                image.sprite = load_split[y, x];

                image.GetComponent<RectTransform>().sizeDelta = new Vector2(1920 / columns, 1080 / rows);
                split_position[y, x] = new Vector2(x * (1920 / columns) + (1920 / columns) / 2, y * (1080 / rows) + (1080 / rows) / 2);
                gameObject.transform.position = new Vector2(x * (1920 / columns) + (1920 / columns) / 2, 1080/rows+1080);
                StartPos[y, x] = new Vector2(x * (1920 / columns) + (1920 / columns) / 2, 1080 / rows + 1080);
                gameObjects[y, x] = gameObject;
            }
        }
    }
}
