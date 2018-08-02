using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(IAPButton))]
public class IAPButtonRebinder : MonoBehaviour {

    private Button m_Button;
    private IAPButton m_IAPButton;

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI priceText;

    private void Awake() {
        m_Button = GetComponent<Button>();
        m_IAPButton = GetComponent<IAPButton>();
    }

    void Start () {
        string shortenedText = m_IAPButton.titleText.text;
        int parenthesisIndex = shortenedText.IndexOf('(');
        if(parenthesisIndex != -1) shortenedText = shortenedText.Substring(0, shortenedText.IndexOf('('));
        titleText.text = shortenedText;
        descriptionText.text = m_IAPButton.descriptionText.text;
        priceText.text = m_IAPButton.priceText.text;
    }
}
