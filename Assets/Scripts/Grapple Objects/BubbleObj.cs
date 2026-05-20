using UnityEngine;
using UnityEngine.InputSystem;

public class BubbleObj : GrappleObj
{
    public BubbleInteractable bubbleInteractGain;
    public float popTime;
    private bool playerInBubble;
    private Vector2 moveInput;
    public LayerMask excludedObjects;
    public LayerMask nothingLayer;

    [SerializeField] Rigidbody2D rb;
    [SerializeField] Collider2D bubbleTrigger;

    public override void MakeUpdate()
    {
        base.MakeUpdate();
        Vector2 move = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed)
                move.x = -1;
            if (Keyboard.current.dKey.isPressed)
                move.x = 1;
            if ((Keyboard.current.aKey.isPressed && Keyboard.current.dKey.isPressed) || (!Keyboard.current.aKey.isPressed && !Keyboard.current.dKey.isPressed))
                move.x = 0;
            if (Keyboard.current.sKey.isPressed)
                move.y = -1;
            if (Keyboard.current.wKey.isPressed)
                move.y = 1;
            if ((Keyboard.current.sKey.isPressed && Keyboard.current.wKey.isPressed) || (!Keyboard.current.sKey.isPressed && !Keyboard.current.wKey.isPressed))
                move.y = 0;
        }
        moveInput = move;

        // Escape bubble
        if (playerInBubble && (Input.GetKeyDown(KeyCode.Space) || Player.Instance.GetIsAttaching()))
        {
            ExitBubble();
        }
    }
    public override void MakeFixedUpdate()
    {
        base.MakeFixedUpdate();
        if (playerInBubble)
        {
            Float();
            Player.Instance.rb.position = rb.position;
        }
    }
    public override void EffectOnPlayerContactAfterHook()
    {
        if (Player.Instance.GetBubbledState())
        {
            DetachHook();
            Player.Instance.DetachHook();
            EnterBubble();
        }
    }

    public void ActivateBubble()
    {
        objCollider.enabled = true;
        objRenderer.enabled = true;
    }
    public void DeactivateBubble()
    {
        DetachHook();
        Player.Instance.DetachHook();
        objCollider.enabled = false;
        objRenderer.enabled = false;
    }
    private void EnterBubble()
    {
        Player.Instance.rb.gravityScale = 0;
        Player.Instance.rb.linearVelocity = Vector2.zero;
        Player.Instance.rb.position = transform.position;

        Player.Instance.rb.simulated = false;
        Player.Instance.transform.parent = transform;
        Player.Instance.transform.localPosition = Vector3.zero;

        Player.Instance.SetInBubble(false);
        playerInBubble = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0;

        bubbleTrigger.isTrigger = false;
        bubbleTrigger.excludeLayers = excludedObjects;
    }
    private void ExitBubble()
    {
        Player.Instance.SetInBubble(true);
        playerInBubble = false;
        rb.bodyType = RigidbodyType2D.Static;

        Player.Instance.transform.parent = null;
        Player.Instance.rb.simulated = true;

        bubbleTrigger.isTrigger = true;
        bubbleTrigger.excludeLayers = nothingLayer;
    }

    public void Float()
    {
        Debug.Log(moveInput);
        // Calculate the direction we want to move in and our desired velocity
        Vector2 normalizedMoveInput = moveInput.normalized;
        Vector2 targetSpeed = normalizedMoveInput * Player.Instance.data.bubbleMoveSpeed;

        #region Calculate AccelRate
        float accelRate;

        accelRate = (Mathf.Abs(targetSpeed.magnitude) > 0.01f) ? Player.Instance.data.floatAccelAmount : Player.Instance.data.floatDecelAmount;
        #endregion

        //Calculate difference between current velocity and desired velocity
        Vector2 velocityDif = targetSpeed - rb.linearVelocity;
        Vector2 movement = velocityDif * accelRate;

        rb.AddForce(movement, ForceMode2D.Force);
    }
}
