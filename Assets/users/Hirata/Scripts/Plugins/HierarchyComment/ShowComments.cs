using UnityEngine;
using UnityEditor;
public class ShowComments : MonoBehaviour
{
    [InitializeOnLoadMethod]
    private static void HierarchyExtensionInit()
    {
        EditorApplication.hierarchyWindowItemOnGUI += AddCommentsField;
    }
    /// <summary>
    /// Hierarchy‚ÉMyComments‚Ì“à—e‚ð•\Ž¦‚·‚é
    /// </summary>
    private static void AddCommentsField(int instanceID, Rect rc)
    {
        var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (gameObject != null)
        {
            var commComp = gameObject.GetComponent<MyComments>();
            if (commComp != null)
            {
                rc.xMin += rc.width - 150;
                rc.yMin += rc.height - 18;
                commComp.commentsText = EditorGUI.TextField(rc, commComp.commentsText);
            }
        }
    }
}