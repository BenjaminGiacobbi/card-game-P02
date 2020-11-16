using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButton : MonoBehaviour
{
    [SerializeField] AudioClip _pressAudio = null;

    public void OnPress()
    {
        if (_pressAudio)
            AudioHelper.PlayClip2D(_pressAudio, 0.3f);
    }
}
