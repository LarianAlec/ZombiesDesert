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

    public static BaseCharacter TryGetCharacterOwner(GameObject obj)
    {
        Transform currentTransform = obj.transform;
        Transform parentTransform = currentTransform.parent;

        while (parentTransform != null)
        { 
            currentTransform = parentTransform;
            parentTransform = currentTransform.parent;
        }

        if (parentTransform == null)
        {
            BaseCharacter characterOwner = currentTransform.GetComponent<BaseCharacter>();
            if(characterOwner != null)
            {
                Debug.Log("TryGetCharacterOwner : Success!");
                return characterOwner;
            }
        }

        Debug.Log("TryGetCharacterOwner() : Failed");
        return null;
    }
}