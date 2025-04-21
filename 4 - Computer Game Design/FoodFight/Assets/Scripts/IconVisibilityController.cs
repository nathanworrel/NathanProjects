using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconVisibilityController : MonoBehaviour
{
    public List<GameObject> icons;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < icons.Count; i++)
        {
            icons[i].SetActive(false);
        }
    }

    public void EnableIcon()
    {
        for (int i = 0; i < icons.Count; i++)
        {
            icons[i].SetActive(true);
        }
    }

    public void DisableIcon()
    {
        for (int i = 0; i < icons.Count; i++)
        {
            icons[i].SetActive(false);
        }
    }

}
