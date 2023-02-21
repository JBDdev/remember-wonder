using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputDebugCanvasControl : MonoBehaviour
{
    [SerializeField] private Image leftStickImage;
    [SerializeField] private Image rightStickImage;
    [SerializeField] private Image jumpImage;
    [SerializeField] private Image grabImage;
    [Space(10)]
    [SerializeField] private Color jumpPressedColor;
    [SerializeField] private Color grabPressedColor;
    [SerializeField] private float pressedSizeChange;

    private Color initJumpColor;
    private Color initGrabColor;
    private Vector2 initJumpSize;
    private Vector2 initGrabSize;

    private void Awake()
    {
        initJumpColor = jumpImage.color;
        initGrabColor = grabImage.color;
        initJumpSize = jumpImage.rectTransform.sizeDelta;
        initGrabSize = grabImage.rectTransform.sizeDelta;

        //We don't want this debug thing to show up in anything but the editor.
#if !UNITY_EDITOR
        Destroy(gameObject);
#endif
    }

    private void Update()
    {
        leftStickImage.rectTransform.anchoredPosition = InputHub.Inst.Gameplay.Move.ReadValue<Vector2>() * leftStickImage.rectTransform.sizeDelta;
        rightStickImage.rectTransform.anchoredPosition = InputHub.Inst.Gameplay.Look.ReadValue<Vector2>() * leftStickImage.rectTransform.sizeDelta;

        (bool jump, bool grab) jumpGrabPressed = (InputHub.Inst.Gameplay.Jump.IsPressed(), InputHub.Inst.Gameplay.Grab.IsPressed());

        jumpImage.color = jumpGrabPressed.jump ? jumpPressedColor : initJumpColor;
        grabImage.color = jumpGrabPressed.grab ? grabPressedColor : initGrabColor;
        jumpImage.rectTransform.sizeDelta = jumpGrabPressed.jump ? initJumpSize + Vector2.one * pressedSizeChange : initJumpSize;
        grabImage.rectTransform.sizeDelta = jumpGrabPressed.grab ? initGrabSize + Vector2.one * pressedSizeChange : initGrabSize;
    }
}
