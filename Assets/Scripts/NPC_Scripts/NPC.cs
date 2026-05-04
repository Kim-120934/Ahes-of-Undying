using System.Collections;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public enum UnlockType { Projectile, DoubleJump,none }

    [Header("Interacción")]
    [SerializeField] private string[] dialogueLines;
    [SerializeField] private UnlockType unlockType;
    public bool absorbAnimation = false;


    private bool _playerInRange = false;
    private bool _dialogueFinished = false;
    private HollowKnightMovement _player;

    private void Start()
    {
        _player = FindFirstObjectByType<HollowKnightMovement>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            _playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            _playerInRange = false;
    }

    public void Interact()
    {
        if (!_playerInRange || _dialogueFinished)
            return;
        if (dialogueLines.Length > 0)
        {
            DialogueUI.instance.StartDialogue(dialogueLines, OnDialogueFinished);
        }
        else

        {
            OnDialogueFinished();
        }
    }

    private void OnDialogueFinished()
    {
        StartCoroutine(AbsorbAnimation());
    }

    IEnumerator AbsorbAnimation()
    {
        if (absorbAnimation)
        {
            //Lanzar animacion aquí
            yield return new WaitForSeconds(1.70f);
        }

        _dialogueFinished = true;

        if (unlockType == UnlockType.Projectile)
            _player.UnlockProjectile();
        else if (unlockType == UnlockType.DoubleJump)
            _player.UnlockDoubleJump();
        if (AbilityUnlockUI.instance != null)
            AbilityUnlockUI.instance.Show();
        else if (unlockType == UnlockType.none)
        {
            // No hace nada
        }
    }
    public IEnumerator TriggerAbsorb()
    {
        yield return StartCoroutine(AbsorbAnimation());
    }
}