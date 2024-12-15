using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    private TextMeshProUGUI damageText;

    public float lifetime = 1.5f;
    private Vector3 floatDirection = Vector3.up;
    private float floatSpeed = 2.0f;

    private float elapsedTime;

    void Awake()
    {
        damageText = GetComponent<TextMeshProUGUI>();
    }

    public void Initialize(string text, Color color)
    {
        damageText.text = text;
        damageText.color = color;
        elapsedTime = 0f;
    }

    void Update()
    {
        transform.position += floatDirection * floatSpeed * Time.deltaTime;

        elapsedTime += Time.deltaTime;
        Color currentColor = damageText.color;
        currentColor.a = (lifetime - elapsedTime) / lifetime;
        damageText.color = currentColor;

        if (elapsedTime >= lifetime) gameObject.SetActive(false);
    }
}
