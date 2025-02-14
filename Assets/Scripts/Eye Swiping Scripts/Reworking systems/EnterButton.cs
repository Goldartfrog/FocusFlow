using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterButton : DwellTimeButton
{
    [SerializeField] private KeyboardTextSystemIntroduction keyboard;
    [SerializeField] private UnityEngine.UI.Image panel;
    private bool isPressed = false;

    protected override void Start()
    {
        base.Start();
        onDwellComplete.AddListener(OnEnter);
    }

    private void OnEnter()
    {
        isPressed = !isPressed;
        UpdatePanelVisuals();
        keyboard.RecieveEnter();
    }

    private void UpdatePanelVisuals()
    {
        if (isPressed)
        {
            if (panel != null) panel.color = new Color(0.2f, 0.6f, 0.3f, 0.6f);
        }
        else
        {
            if (panel != null) panel.color = new Color(0.1f, 0.1f, 0.1f, 0.6f);
        }
    }
}