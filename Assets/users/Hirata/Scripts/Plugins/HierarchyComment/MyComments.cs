using UnityEngine;

#if UNITY_EDITOR
    public class MyComments : MonoBehaviour
    {
        [SerializeField]
        [TextArea(5, 10)]
        public string commentsText;
    }
#endif