using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueInput : MonoBehaviour
{
    [SerializeField] InputActionAsset inputActions;
    private InputAction selectChoice;
    private InputAction seekDialogue;
    private Vector2 choiceInput;
    private InputActionMap dialogueMap;
    public static DialogueInput Instance { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        dialogueMap = inputActions.FindActionMap("Dialogue");
        seekDialogue = dialogueMap.FindAction("SeekDialogue");
        seekDialogue.RemoveAllBindingOverrides();
        // use the same button jump to close message
        InputAction jumpAction = inputActions.FindActionMap("Gameplay").FindAction("Jump");
        foreach (InputBinding binding in jumpAction.bindings)
        {
            seekDialogue.AddBinding().WithPath(binding.effectivePath);
        }

        selectChoice = dialogueMap.FindAction("SelectChoice");
        selectChoice.RemoveAllBindingOverrides();
        InputAction moveAction = inputActions.FindActionMap("Gameplay").FindAction("Move");
        for (int i = 0; i < moveAction.bindings.Count; i++)
        {
            InputBinding binding = moveAction.bindings[i];
            if (binding.isComposite)
            {
                var composite = selectChoice.AddCompositeBinding("2DVector");
                for (int j = 1; j <= 4; j++)
                {
                    InputBinding partBinding = moveAction.bindings[i + j];
                    composite.With(partBinding.name, partBinding.effectivePath);
                }
            }
            else if (!binding.isPartOfComposite)
            {
                selectChoice.AddBinding().WithPath(binding.effectivePath);
            }
        }
    }
    public void EnableDialogueInput()
    {
        seekDialogue.started += OnSeekDialogue;
        selectChoice.performed += OnSelectChoice;
        selectChoice.canceled += OnSelectChoice;

        seekDialogue.Enable();
        selectChoice.Enable();
    }
    public void DisableDialogueInput()
    {
        seekDialogue.started -= OnSeekDialogue;
        selectChoice.performed -= OnSelectChoice;
        selectChoice.canceled -= OnSelectChoice;

        seekDialogue.Disable();
        selectChoice.Disable();
    }
    public void OnSeekDialogue(InputAction.CallbackContext context)
    {
        if(NPC.CurrentInteractingNPC != null)
        {
            NPC.CurrentInteractingNPC.SetSeekDialogueFlag();
        }
    }
    public void OnSelectChoice(InputAction.CallbackContext context)
    {
        choiceInput = context.ReadValue<Vector2>();
        if(NPC.CurrentInteractingNPC != null)
        {
            if(choiceInput.y > 0)
            {
                NPC.CurrentInteractingNPC.SetChoiceUpFlag();
            }
            else if (choiceInput.y < 0)
            {
                NPC.CurrentInteractingNPC.SetChoiceDownFlag();
            }
        }
    }
}
