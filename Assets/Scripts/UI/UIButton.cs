using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButton : MonoBehaviour
{
    [SerializeField] AudioClip _pressAudio = null;
    [SerializeField] AudioClip _hoverAudio = null;

    public void OnPress()
    {
        if (_pressAudio)
            AudioHelper.PlayClip2D(_pressAudio, 0.2f);
    }

    public void OnHover()
    {
        if (_hoverAudio)
            AudioHelper.PlayClip2D(_hoverAudio, 0.2f);
    }
}
