using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour {
    protected FighterController controller;
    protected Vector3 forward = new Vector3(1, 0, 0);

    protected bool crouched = false;

    protected int slot = 2;

    protected float maxHealth = 100f, health = 100f, critHealth = 50f, healthGainedFromPunch = 50f, damage = 50f;
    private bool critical, dead;

    private Animator animator;
    private bool attackedLast;
    private CriticalStateHandler critState;

    #region Testing
    private Vector3 startPos, targetPosition;
    private void Start()
    {
        startPos = transform.position;
        targetPosition = startPos;
        animator = GetComponent<Animator>();
        critState = GetComponentInChildren<CriticalStateHandler>();
    }

    public void RecacheStartingPosition()
    {
        startPos = transform.position;
        targetPosition = startPos;
    }
    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, GameManager.Instance.DeltaTime(controller.pauseRegister) * 10);
        if (animator.speed != 0) animator.speed = 1/GameManager.Instance.CurrentSlowMotionFactor;
    }
    #endregion

    #region Commands
    public virtual void MoveRight()
    {
        UpdateSlot(1);
        
    }

    public virtual void MoveLeft()
    {
        UpdateSlot(-1);
        
    }

    //called when both fighters try to move into the same spot at the same time
    public virtual void MoveRightBump()
    {
        /*
        if (slot < LevelManager.NUM_SLOTS - 2)
        {
            MoveRight();
        }
        */
    }

    public virtual void MoveLeftBump()
    {
        /*
        if (slot > 2)
        {
            MoveLeft();
        }
        */
    }

    //called when THIS fighter is forced right
    public virtual void ForceRight()
    {
        MoveRight();
    }

    public virtual void ForceLeft()
    {
        MoveLeft();
    }

    //Called when THIS fighter bumps the other fighter Right
    public virtual void BumpRight()
    {
        MoveRight();
    }

    public virtual void BumpLeft()
    {
        MoveLeft();
    }

    public virtual void Crouch()
    {
        crouched = true;
        targetPosition = new Vector3(LevelManager.Instance.SlotDistance * slot - (LevelManager.NUM_SLOTS / 2 * LevelManager.Instance.SlotDistance) + (LevelManager.Instance.SlotDistance / 2), -3f, 5);
    }

    public virtual void StandUp()
    {
        crouched = false;
        targetPosition = new Vector3(LevelManager.Instance.SlotDistance * slot - (LevelManager.NUM_SLOTS / 2 * LevelManager.Instance.SlotDistance) + (LevelManager.Instance.SlotDistance / 2), 0, 5);
    }

    //this is the regular attack
    public virtual void LightAttack(bool impact, bool blocked)
    {
        if (attackedLast) {
            animator.SetTrigger("Attack2");
            attackedLast = false;
        } else {
            animator.SetTrigger("Attack");
            attackedLast = true;
        }
    }

    //When two attacks collide
    public virtual void LightAttackDraw()
    {
        if (attackedLast)
        {
            animator.SetTrigger("Attack2");
            attackedLast = false;
        }
        else
        {
            animator.SetTrigger("Attack");
            attackedLast = true;
        }
        if(controller.PlayerIndex == 0)
        {
            UpdateSlot(-5);
        }
        else
            UpdateSlot(5);

        GameManager.Instance.ActivateSlowMotion();
    }

    //MOVING LEFT attack
    public virtual void LeftLightAttack()
    {
        UpdateSlot(-1);
    }

    public virtual void RightLightAttack()
    {
        UpdateSlot(1);
    }

    public virtual void Block(bool impact)
    {
        if(impact) {
            SoundEffectManager.Instance.PlayBlockSound();
            animator.SetTrigger("Block");
            HitEffectSystem.Instance.SpawnHitEffect("Block", transform.position + Vector3.up*2f - Vector3.forward*2f);
            Camera.main.GetComponent<Follow>().Screenshake(2f);
        } else {
            animator.SetTrigger("Block");
        }
    }

    public virtual void BrokenBlock()
    {

    }
    #endregion

    private void UpdateSlot(int delta)
    {
        slot += delta;
        if (slot >= LevelManager.NUM_SLOTS)
        {
            slot = LevelManager.NUM_SLOTS - 1;
        }
        else if (slot < 0)
        {
            slot = 0;
        }
            targetPosition = new Vector3(LevelManager.Instance.SlotDistance * slot - (LevelManager.NUM_SLOTS / 2 * LevelManager.Instance.SlotDistance) + (LevelManager.Instance.SlotDistance / 2), crouched ? -3f : 0f, 5);
    }

    #region Heath
    public void TakeDamage(float damage)
    {
        health -= damage;
        GameManager.Instance.Sleep(0.25f);
        if (health <= 0)
        {
            Die();
            Camera.main.GetComponent<Follow>().Screenshake(5f);
        }
        else if (!critical)
        {
            SoundEffectManager.Instance.PlayHitSound();
            HitEffectSystem.Instance.SpawnHitEffect("Hit", transform.position + Vector3.right*transform.localScale.x + Vector3.up*2f - Vector3.forward*2f);
            animator.SetTrigger("Hurt");
            Camera.main.GetComponent<Follow>().Screenshake(3f);
            if(health <= critHealth)
            {
                BecomeCritical();
            }
        }
    }

    public void AddHealth(float healthGained)
    {
        health += healthGained;
        if (health > maxHealth) health = maxHealth;
        if (critical)
        {
            if(health > critHealth)
            {
                EndCritical();
            }
        }
    }

    public void Die()
    {
        LevelManager.Instance.PlayerDied(controller.PlayerIndex);
        animator.SetTrigger("Die");
        HitEffectSystem.Instance.SpawnHitEffect("KO", transform.position + Vector3.right*transform.localScale.x + Vector3.up*2f - Vector3.forward*2f);
        SoundEffectManager.Instance.PlayKOSounds();
    }

    public void BecomeCritical()
    {
        critical = true;
        critState.UpdateCriticalState(critical);
    }

    public void EndCritical()
    {
        critical = false;
        critState.UpdateCriticalState(critical);
    }
    #endregion

    public Vector3 Forward
    {
        get { return forward; }
    }

    public bool IsFacingRight
    {
        get { return forward.x > 0 ? true : false; }
    }

    public int Slot
    {
        get { return slot; }
        set { slot = value; }
    }

    public FighterController Controller
    {
        get { return controller; }
        set { controller = value; }
    }

    public float Damage
    {
        get { return damage; }
    }

    public float HealthGainedFromPunch
    {
        get { return healthGainedFromPunch; }
    }

    public bool Crouched
    {
        get { return crouched; }
    }

    public Animator Animator
    {
        get { return animator; }
    }
}
