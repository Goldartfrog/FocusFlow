using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteButton : DwellTimeButton
{
    [SerializeField] private KeyboardTextSystemIntroduction keyboard;
    [SerializeField] private GameObject visualObject;

    protected override void Start()
    {
        base.Start();
        onDwellComplete.AddListener(OnDelete);
    }

    private void OnDelete()
    {
        keyboard.RecieveDelete();
    }

    public override void Enable()
    {
        base.Enable();
        if (visualObject != null) visualObject.SetActive(true);
    }

    public override void Disable()
    {
        base.Disable();
        if (visualObject != null) visualObject.SetActive(false);
    }
}
