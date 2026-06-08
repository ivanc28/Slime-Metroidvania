using System.Collections;
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
    public float lifespan;
    private float lifetime;

    public float flashRate;
    private float flashTimer;
    private bool startFlashing;

    private Vector3 startingPos;
    private bool bubbleEmpty;

    [SerializeField] Rigidbody2D rb;
    [SerializeField] Collider2D bubbleTrigger;
    [SerializeField] AudioClip[] bubbleInClips;
    [SerializeField] AudioClip[] popClips;

    //private CameraSwitcher[] switchers;
    //private ContactFilter2D filter;
    public override void MakeStart()
    {
        base.MakeStart();
        startingPos = transform.position;
        lifetime = lifespan;
        bubbleEmpty = bubbleInteractGain == null;
        //switchers = FindObjectsByType<CameraSwitcher>(FindObjectsSortMode.None);
        //filter = new();
    }
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
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Player.Instance.Jump();
            }
            ExitBubble();
        }
        if (playerInBubble)
        {
            if (lifetime > 0)
            {
                lifetime -= Time.deltaTime;
                if(lifetime <= lifespan / 2)
                {
                    startFlashing = true;
                }
            }
            else if (lifetime <= 0)
            {
                ExitBubble();
            }
        }
        if (startFlashing)
        {
            if(flashTimer > 0)
            {
                flashTimer -= Time.deltaTime;
            }
            else
            {
                objRenderer.enabled = !objRenderer.enabled;
                flashTimer = flashRate;
            }
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
            if (bubbleEmpty)
            {
                DetachHook();
                Player.Instance.DetachHook();
                EnterBubble();
                lifetime = lifespan;
            }
            else
            {
                Player.Instance.SetOnBubble(true);
            }
        }
        else
        {
            Player.Instance.SetOnBubble(true);
        }
    }
    public override void DetachHook()
    {
        base.DetachHook();
        Player.Instance.SetOnBubble(false);
    }
    public void ActivateBubble()
    {
        objCollider.enabled = true;
        objRenderer.enabled = true;
    }
    public void DeactivateBubble()
    {
        DetachHook();
        if (!Player.Instance.GetHookBeingThrown() && !Player.Instance.GetIsAttaching())
        {
            Player.Instance.DetachHook();
            Player.Instance.currentAttachedCollider = null;
        }
        objCollider.enabled = false;
        objRenderer.enabled = false;
        //// MANUALLY SPAWN PEBBLES IF NEEDED
        //if(bubbleInteractGain != null)
        //{
        //    bubbleInteractGain.SpawnPebbles(bubbleInteractGain.numPebbles);
        //    bubbleInteractGain.GetRoomOfInteractable().collectedInteractables.Add(bubbleInteractGain.GetInteractableID());
        //    Destroy(bubbleInteractGain.gameObject);
        //}
        transform.position = startingPos;
        bubbleEmpty = true;
    }
    private void EnterBubble()
    {
        anim.SetBool("inBubble", true);
        if(SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(bubbleInClips, 0.5f, true);
        }
        Player.Instance.rb.gravityScale = 0;
        Player.Instance.rb.linearVelocity = Vector2.zero;
        Player.Instance.rb.position = transform.position;
        objRenderer.sortingOrder = 1;

        Player.Instance.rb.simulated = false;
        Player.Instance.transform.parent = transform;
        Player.Instance.transform.localPosition = Vector3.zero;

        Player.Instance.SetInBubble(true);
        playerInBubble = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0;

        bubbleTrigger.isTrigger = false;
        bubbleTrigger.excludeLayers = excludedObjects;
    }
    private void ExitBubble()
    {
        anim.SetBool("inBubble", false);
        Player.Instance.SetInBubble(false);
        playerInBubble = false;
        rb.bodyType = RigidbodyType2D.Static;
        objRenderer.sortingOrder = -1;

        Player.Instance.transform.parent = null;
        DontDestroyOnLoad(Player.Instance.gameObject);
        Player.Instance.rb.simulated = true;

        bubbleTrigger.isTrigger = true;
        bubbleTrigger.excludeLayers = nothingLayer;
        startFlashing = false;
        //foreach (CameraSwitcher switcher in FindObjectsByType<CameraSwitcher>(FindObjectsSortMode.None))
        //{
        //    if (switcher.UsingCamera)
        //    {
        //        switcher.OnPlayerExitBubble();
        //    }
        //}
        StartCoroutine(PopBubble(popTime));
    }

    public void Float()
    {
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
        //foreach (CameraSwitcher switcher in switchers)
        //{
        //    switcher.CheckColliderPlayerInBubble(bubbleTrigger, filter);
        //}
    }

    public IEnumerator PopBubble(float delayBeforeReappear)
    {
        yield return null;
        DeactivateBubble();
        if(SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(popClips, 0.2f, true, 0.9f, 1.1f);
        }
        yield return new WaitForSeconds(delayBeforeReappear);
        ActivateBubble();
        anim.SetTrigger("replaceBubble");
    }
}
