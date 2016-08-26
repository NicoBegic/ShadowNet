using UnityEngine;

public class ScrollViewContent : MonoBehaviour
{
    public void addObject(GameObject obj)
    {
        obj.transform.SetParent(transform);
    }
}
