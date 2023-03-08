using UnityEngine;


[ExecuteAlways] // 再生していない間も座標と半径が変化するように
public class SlimeRenderer : MonoBehaviour
{
    [SerializeField] private Material material; // スライム用のマテリアル

    private const int MaxSphereCount = 256; // 球の最大個数（シェーダー側と合わせる）
    private readonly Vector4[] _spheres = new Vector4[MaxSphereCount];
    private SphereCollider[] _colliders;
    private Vector4[] _colors = new Vector4[MaxSphereCount];
    private Vector4[] InColor = new Vector4[1];

    private GameObject RootObj;//ルートオブジェクト
    private GameObject TipObj;//Tipオブジェクト

    private float StretchSize;//ヒモの伸び率
    private float StartSize;//ヒモの伸び率開始時

    GameObject PObj;//プレイヤーオブジェクト
   // Obi_Player obi_player;

    private float hardness = 0; //硬さを示す指標

    public bool isInvisible = false;
    public float Invisiblealpha = 0.0f;

    private void Start()
    {
        //プレイヤーを格納
        //PObj = transform.parent.transform.Find("Player_body").gameObject;

        //RootObj = transform.parent.transform.Find("Player_root").gameObject;

        //TipObj = transform.parent.transform.Find("Player_tip").gameObject;

        ////Obiスクリプトを入れる
        //obi_player = PObj.GetComponent<Obi_Player>();

        Refresh();

        //開始時の伸び率を格納
        StartSize = 5;
    }

    public void Refresh()
    {        
        //色を設定

        // 子のSphereColliderをすべて取得
        _colliders = GetComponentsInChildren<SphereCollider>();

        // シェーダー側の _SphereCount を更新
        material.SetInt("_SphereCount", _colliders.Length);

        // ランダムな色を配列に格納
        for (var i = 0; i < _colors.Length; i++)
        {
            _colors[i] = (Vector4)Random.ColorHSV(0, 0, 1, 1, 1, 1);
            _colors[i] = new Vector4(1, 0.4f, 0, 1);
        }

        // シェーダー側の _Colors を更新
        material.SetVectorArray("_Colors", _colors);
        //material.SetFloat("Hardness", obi_player.knead);
        material.SetFloat("Invisible",Invisiblealpha);


    }

    public void SetInvisible(bool invisible)
    {
        isInvisible = invisible;
    }

    private void Update()
    {
        if (isInvisible)
        {
            Invisiblealpha = 0.0f;
        }
        else
        {
            Invisiblealpha = 1.0f;
        }

        // 子のSphereColliderの分だけ、_spheres に中心座標と半径を入れていく
            for (var i = 0; i < _colliders.Length; i++)
            {
                var col = _colliders[i];
                var t = col.transform;
                var center = t.position;
                var radius = t.lossyScale.x * col.radius;

                //if (StartSize < obi_player.obirope_Length)
                //{
                //    radius = t.lossyScale.x * col.radius + (Mathf.Abs((obi_player.obirope_Length - StartSize) / 20));
                //}

                // 中心座標と半径を格納
                _spheres[i] = new Vector4(center.x, center.y, center.z, radius);
            }

            //硬さを代入
            //hardness = obi_player.knead;
            // シェーダー側の _Spheres を更新
            material.SetVectorArray("_Spheres", _spheres);
            material.SetFloat("Hardness", hardness);
            material.SetFloat("Invisible", Invisiblealpha);
            material.SetInt("SlimeCount", _colliders.Length);
    }
}