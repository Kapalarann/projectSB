using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SelectorController : MonoBehaviour
{
    public static readonly HashSet<SelectorController> selectorControllers = new HashSet<SelectorController>();

    public int playerIndex;
    private Color[] colors = { Color.red, Color.blue, Color.green, Color.black };
    public float moveSpeed = 100f;
    public RectTransform rect;
    public Canvas canvas;
    public SpriteRenderer cursor;

    private Vector2 moveInput;

    private void Awake()
    {
        selectorControllers.Add(this);
        playerIndex = selectorControllers.Count - 1;
        cursor.color = colors[playerIndex];

        rect = GetComponent<RectTransform>();
        if (canvas == null) canvas = FindAnyObjectByType<Canvas>();

        CharacterSelectManager.instance.Inst(playerIndex);
        transform.SetParent(canvas.transform, false);
    }

    private void OnDestroy()
    {
        selectorControllers.Remove(this);
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnMouseMove(InputValue value)
    {
        Vector2 screenPos = value.Get<Vector2>();

        Vector2 localPoint;

        Camera renderCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect.parent as RectTransform, screenPos, renderCamera, out localPoint);

        rect.anchoredPosition = localPoint;
    }

    public void OnAttack()
    {
        GameObject hovered = CharacterSelectManager.instance.GetHovered(rect.position);
        if (hovered)
        {
            PlayerInput playerInput = GetComponent<PlayerInput>();

            if (playerInput != null && playerInput.devices.Count > 0)
            {
                InputDevice device = playerInput.devices[0];
                CharacterSelectManager.instance.SelectCharacter(this, hovered, device);
            }
        }
    }

    public void OnBlock()
    {
        CharacterSelectManager.instance.DeselectCharacter(this);

        PlayerInput input = GetComponent<PlayerInput>();
        if (input != null) input.DeactivateInput();

        Destroy(gameObject);
    }


    void FixedUpdate()
    {
        Vector3 delta = (Vector3)moveInput * moveSpeed * Time.fixedDeltaTime;
        rect.anchoredPosition += new Vector2(delta.x, delta.y);

        GameObject hovered = CharacterSelectManager.instance.GetHovered(rect.position);
        CharacterSelectManager.instance.UpdateHover(hovered, playerIndex);
    }
}
