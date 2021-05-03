using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InstructionText : MonoBehaviour
{
    private bool hasClicked;
    private TextMeshProUGUI tmp;

    private void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (!hasClicked)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }

                hasClicked = true;
                StartCoroutine(FadeText(1.5f));
            }
        }
    }

    IEnumerator FadeText(float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = 1f - elapsedTime / duration;
            tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }
}
