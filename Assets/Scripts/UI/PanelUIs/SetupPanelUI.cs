using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupPanelUI : PanelUI
{
    [SerializeField] GameObject _setupRight = null;
    [SerializeField] GameObject _setupLeft = null;
    [SerializeField] GameObject _vsRight = null;
    [SerializeField] GameObject _vsLeft = null;
    [SerializeField] GameObject _backingPanel = null;
    [SerializeField] float _moveDistance = 0;
    [SerializeField] float _moveTime = 0;
    [SerializeField] AudioClip _clangAudio = null;

    float _lastXPos = 0;
    float _restingXPos = 0;

    private void Awake()
    {
        _lastXPos = _setupRight.transform.position.x;
        _restingXPos = _setupRight.transform.position.x;
        Debug.DrawRay(new Vector3(_restingXPos, 0, 0), -Vector3.forward * 2000, Color.blue, 20f);
    }

    private void Update()
    {
        if (_lastXPos != _setupRight.transform.position.x)
        {
            if(Mathf.Abs(_setupRight.transform.position.x - _restingXPos) < 30)
                AudioHelper.PlayClip2D(_clangAudio, 0.3f);
        }
        _lastXPos = _setupRight.transform.position.x;

        if(Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log(_setupRight.transform.position);
            Debug.Log(_setupLeft.transform.position);
            Debug.Log(_vsRight.transform.position);
            Debug.Log(_vsLeft.transform.position);
        }
    }

    public override void OpenAnimation()
    {
        _blockPanel.gameObject.SetActive(true);
        _backingPanel.SetActive(true);
        OpenTranslation(_setupRight, "Right");
        OpenTranslation(_setupLeft, "Left");
        OpenTranslation(_vsRight, "Right");
        OpenTranslation(_vsLeft, "Left");
        gameObject.SetActive(true);
        LeanTween.delayedCall(_moveTime, () => { _blockPanel.gameObject.SetActive(false); });
    }

    public override void CloseAnimation()
    {
        _blockPanel.gameObject.SetActive(true);
        _backingPanel.SetActive(false);
        CloseTranslation(_setupRight, "Right");
        CloseTranslation(_setupLeft, "Left");
        CloseTranslation(_vsRight, "Right");
        CloseTranslation(_vsLeft, "Left");
        LeanTween.delayedCall(_moveTime - 0.01f, ClosePanel);
    }

    private void OpenTranslation(GameObject panel, string direction)
    {
        float distance = DistanceDirection(direction);
        float xPos = panel.transform.position.x;
        panel.transform.position = new Vector3(xPos + distance, panel.transform.position.y, panel.transform.position.z);
        LeanTween.move(panel, new Vector3(xPos, panel.transform.position.y, panel.transform.position.z), _moveTime).setEaseOutBounce();
    }

    private void CloseTranslation(GameObject panel, string direction)
    {
        float distance = DistanceDirection(direction);
        float xPos = panel.transform.position.x;
        LeanTween.move(panel, new Vector3(xPos + distance, panel.transform.position.y, panel.transform.position.z), _moveTime).setOnComplete(
            () => { panel.transform.position = new Vector3(xPos, panel.transform.position.y, panel.transform.position.z); });
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);
        _lastXPos = _restingXPos;
    }

    private float DistanceDirection(string direction)
    {
        float distance = 0;
        switch (direction)
        {
            case "Right":
                distance = _moveDistance;
                break;
            case "Left":
                distance = _moveDistance * -1;
                break;
            default:
                break;
        }
        return distance;
    }
}
