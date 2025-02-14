using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuggestionButton : DwellTimeButton
{
    [SerializeField] private KeyboardTextSystemIntroduction keyboard;
    [SerializeField] private TMPro.TextMeshPro suggestionText;

    protected override void Start()
    {
        base.Start();
        onDwellComplete.AddListener(OnSuggestionSelect);
    }

    private void OnSuggestionSelect()
    {
        if (suggestionText != null)
        {
            keyboard.RecieveSuggestion(suggestionText.text);
        }
    }
}
