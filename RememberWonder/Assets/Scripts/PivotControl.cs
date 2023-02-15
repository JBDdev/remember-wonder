using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotControl : MonoBehaviour
{
    [SerializeField] PlayerMovement player;
    [Header("Rotation with Input")]
    [SerializeField] bool clickAndDrag;
    [SerializeField] bool invertX;
    [SerializeField] bool invertY;
    [Tooltip("Degrees per second. Speed up/slow down individual input sources in the input asset.")]
    [SerializeField] float lookSpeed;
    [SerializeField][VectorLabels("Min", "Max")] Vector2 pitchRange;
    [Header("Reposition on Jump")]
    [Tooltip("How far this pivot can be from its initial local Y when the player jumps. Ignored if player is null.")]
    [SerializeField][Range(0, 10)] float maxYOffsetOnJump = 2;
    [Tooltip("How thick the downward BoxCast during repositioning should be. LineCasts instead if 0.")]
    [SerializeField][Min(0)] float castWidth;
#if UNITY_EDITOR
    [SerializeField] bool visualize;
#endif

    float yaw;
    float pitch;
    Vector2 lookAxis;
    Quaternion targetRotation;

    Vector3 followCache;
    bool repositioning;
    Vector3 initialOffset;
    float? playerYOnJump;
    float playerCapsuleHeight;

    void Start()
    {
        initialOffset = player.transform.TransformVector(transform.localPosition);
        transform.parent = null;

        Coroutilities.DoNextFrame(this, () =>
        {
            if (player) player.GetCapsuleCastParams(out playerCapsuleHeight, out _, out _, out _);
        });
    }

    void Update()
    {
        //Follow player; if repositioning, leave Y alone
        followCache = player.transform.position + initialOffset;
        if (repositioning) followCache.y = transform.position.y;
        transform.position = followCache;

        lookAxis = InputHub.Inst.Gameplay.Look.ReadValue<Vector2>();

        //For example: If we rotate on mouse down, and none of the mouse buttons are down, don't rotate.
        if (lookAxis == Vector2.zero || (clickAndDrag && !InputHub.Inst.Gameplay.LookActivate.IsPressed()))
        {
            transform.localRotation = targetRotation;
            return;
        }

        yaw += lookSpeed * lookAxis.x * Time.deltaTime;
        //Axis for yaw is multiplied by -1 if invertX is true
        targetRotation = Quaternion.AngleAxis(yaw, Vector3.up * (invertX ? -1 : 1));

        pitch += lookSpeed * lookAxis.y * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, pitchRange.x, pitchRange.y);
        //Axis for pitch is multiplied by -1 if invertY is true
        targetRotation *= Quaternion.AngleAxis(pitch, Vector3.left * (invertY ? -1 : 1));

        transform.localRotation = targetRotation;
    }

    private void FixedUpdate()
    {
        RepositionOnJump();
    }

    void RepositionOnJump()
    {
        if (maxYOffsetOnJump <= 0) return;
        //Player landed, stop repositioning
        if (!player.jumpInProgress)
        {
            repositioning = false;
            playerYOnJump = null;
            return;
        }
        //Player fell past initial jump pos, stop repositioning
        if (player.Velocity.y < 0
            && playerYOnJump is float onJumpY
            && player.transform.position.y <= onJumpY)
        {
            repositioning = false;
            return;
        }

        //If we don't have a y pos from when the jump started, set one (because we must have just started a jump)
        repositioning = true;
        playerYOnJump ??= player.transform.position.y;

        float castEndY = Mathf.Min(player.transform.position.y, Mathf.Max(
            playerYOnJump.Value - playerCapsuleHeight / 2,
            player.transform.position.y - maxYOffsetOnJump));

        RaycastHit hit;
        bool anyHits;
        if (castWidth <= 0)
        {
            anyHits = Physics.Linecast(
                player.transform.position,
                player.transform.position.Adjust(1, castEndY),
                out hit,
                ~0,
                QueryTriggerInteraction.Ignore);
        }
        else
        {
            anyHits = Physics.BoxCast(
                player.transform.position,
                new Vector3(castWidth / 2, 0.01f, castWidth / 2),
                Vector3.down,
                out hit,
                player.transform.rotation,
                player.transform.position.y - castEndY,
                ~0,
                QueryTriggerInteraction.Ignore);
        }

        if (anyHits)
        {
            //Move pivot to where player's center would be if resting on hit point, plus offset
            transform.position = player.transform.position.Adjust(1, hit.point.y + (playerCapsuleHeight / 2) + initialOffset.y);
        }
        //Else go to the cast's end pos
        else transform.position = player.transform.position.Adjust(1, castEndY + (playerCapsuleHeight / 2) + initialOffset.y);

        #region Visualize
#if UNITY_EDITOR
        if (visualize)
        {
            var primary = new Color(1, 0, 0, 0.5f);
            var secondary = new Color(1, 0.375f, 0.375f, 0.5f);
            var tertiary = new Color(1, 0.75f, 0.75f, 0.3f);
            UtilFunctions.DrawSphere(player.transform.position.Adjust(1, playerYOnJump.Value - playerCapsuleHeight / 2), 0.05f, secondary, 5);

            UtilFunctions.DrawBox(player.transform.position.Adjust(
                1, (anyHits ? hit.point.y : castEndY)), player.transform.rotation, 0.1f, secondary, 5);
            Debug.DrawRay(player.transform.position.Adjust(
                1, (anyHits ? hit.point.y : castEndY)), Vector3.up * playerCapsuleHeight / 2, secondary, 5);
            Debug.DrawRay(player.transform.position.Adjust(
                1, (anyHits ? hit.point.y : castEndY) + playerCapsuleHeight / 2), initialOffset, primary, 5);
            UtilFunctions.DrawBox(transform.position, player.transform.rotation, 0.2f, primary, 5);

            Debug.DrawLine(player.transform.position, player.transform.position.Adjust(1, castEndY), tertiary, 5f);
            if (anyHits)
            {
                UtilFunctions.DrawBox(
                    player.transform.position.Adjust(1, hit.point.y),
                    player.transform.rotation,
                    new Vector3(castWidth, 0.01f, castWidth),
                    tertiary,
                    5f);
                UtilFunctions.DrawSphere(hit.point, 0.025f, primary, 5f);
            }
        }
#endif
        #endregion
    }
}