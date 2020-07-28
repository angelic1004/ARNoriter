using UnityEngine;
using System.Collections;

public class NavigationInfo : MonoBehaviour
{
    public enum SceneCategory
    {
        None,
        FourD,
        Sketch,
        Other
    }

    public SceneCategory whatScene;

    public GlobalDataManager.CategoryType categoryType;
}
