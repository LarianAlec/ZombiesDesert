using UnityEngine;

public static class Helper
{
    public static GameObject FindGameObjectInChildWithTag(GameObject parent, string tag)
    {
        Transform t = parent.transform;
        if (t.childCount == 0)
        { 
            return null;
        }

        for (int i = 0; i < t.childCount; i++)
        {
            if (t.GetChild(i).CompareTag(tag))
            {
                return t.GetChild(i).gameObject;
            }
        }

        return null;
    }
}