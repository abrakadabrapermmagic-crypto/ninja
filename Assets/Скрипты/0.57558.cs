using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text myText;  // Перетащите Text сюда в инспекторе
    public Button showButton;  // Кнопка "Показать"
    public Button hideButton;  // Кнопка "Скрыть"

    void Start()
    {
        myText.gameObject.SetActive(false);  // Текст скрыт изначально

        showButton.onClick.AddListener(ShowText);
        hideButton.onClick.AddListener(HideText);
    }

    void ShowText()
    {
        myText.gameObject.SetActive(true);
    }

    void HideText()
    {
        myText.gameObject.SetActive(false);
    }
}
