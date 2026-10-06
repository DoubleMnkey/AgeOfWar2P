using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitControl : MonoBehaviour, IDamageable
{
    public enum UnitType { Melee, Ranged, Tank }
    public enum UnitState { Waiting, Moving, Idle, Attacking, Dead, IdleAttacking, WalkAttacking }

    public bool IsInvincible { get; set; } = false;

    [Header("방향 설정")]
    [Tooltip("왼쪽(-X)으로 전진하는 유닛이면 true, 오른쪽(+X)으로 전진하는 유닛이면 false")]
    public bool isLeft = false;

    [Header("유닛 정보")]
    public UnitType unitType = UnitType.Melee;
    public UnitState currentState = UnitState.Waiting;
    public float maxHealth = 100f;
    public float currentHealth;
    public float moveSpeed = 2f;
    public float attackDamage = 10f;

    [Header("보상 정보")]
    public int eraIndex = 0; 
    public int unitID = 0;   

    [Header("거리 및 감지 설정")]
    public float minAllyGap = 0.3f;      
    public float meleeAttackRange = 0.2f;  
    public float rangedAttackRange = 5.0f; 
    public LayerMask enemyLayer;
    public LayerMask allyLayer;

    [Header("피격 이펙트")]
    [SerializeField] private GameObject bloodParticlePrefab;

    [Header("사운드 설정")]
    [SerializeField] private AudioClip attackSound;    
    [SerializeField] private AudioClip rangedAttackSound;

    [Tooltip("Melee 및 Ranged 유닛용 사망 효과음 (5개 중 랜덤 재생)")]
    [SerializeField] private AudioClip[] randomDieSounds = new AudioClip[5];

    [Tooltip("Tank 유닛 전용 고유 사망 효과음")]
    [SerializeField] private AudioClip tankDieSound;

    [Header("컴포넌트 및 UI")]
    [SerializeField] private Collider2D unitCollider;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private UnitHealthBar healthBar; 

    [Header("UI 위치 설정")]
    public float healthBarYOffset = 1.8f; 

    private IDamageable currentTarget = null;

    public bool IsDead => currentState == UnitState.Dead;

    private readonly int animWalk = Animator.StringToHash("Walk Animation");
    private readonly int animIdle = Animator.StringToHash("Idle Animation");
    private readonly int animAttack = Animator.StringToHash("Attack Animation");
    private readonly int animMeleeAttack = Animator.StringToHash("MeleeAttack Animation");
    private readonly int animWalkAttack = Animator.StringToHash("WalkAttack Animation");
    private readonly int animIdleAttack = Animator.StringToHash("IdleAttack Animation");
    private readonly int animDie = Animator.StringToHash("Die Animation");

    protected virtual void Awake()
    {
        if (unitCollider == null) unitCollider = GetComponent<Collider2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        currentHealth = maxHealth;
        SetColliderActive(false);

        if (healthBar == null) healthBar = GetComponentInChildren<UnitHealthBar>();
        if (healthBar != null) healthBar.Init(this);
    }

    protected virtual void Update()
    {
        if (currentState == UnitState.Dead || currentState == UnitState.Waiting) return;

        EvaluateState();
        ExecuteState();
    }

    protected void ChangeState(UnitState newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        switch (currentState)
        {
            case UnitState.Moving:
                animator.Play(animWalk);
                break;

            case UnitState.Idle:
                animator.Play(animIdle);
                break;

            case UnitState.Attacking:
                if (unitType == UnitType.Melee || unitType == UnitType.Tank)
                    animator.Play(animAttack);
                else
                    animator.Play(animMeleeAttack);
                break;

            case UnitState.WalkAttacking:
                animator.Play(animWalkAttack);
                break;

            case UnitState.IdleAttacking:
                animator.Play(animIdleAttack);
                break;

            case UnitState.Dead:
                animator.Play(animDie);
                break;
        }
    }


    protected virtual void EvaluateState()
    {
        Vector2 moveDir = isLeft ? Vector2.left : Vector2.right;

        UnitControl frontAlly = GetFrontAlly(moveDir);
        bool isBlockedByAlly = false;

        if (frontAlly != null)
        {
            if (isLeft)
            {
                float myFrontX = transform.position.x - GetHalfWidth();
                float frontAllyBackX = frontAlly.transform.position.x + frontAlly.GetHalfWidth();

                float nextMyFrontX = myFrontX - (moveSpeed * Time.deltaTime);
                float stopLineX = frontAllyBackX + minAllyGap;

                if (nextMyFrontX <= stopLineX) isBlockedByAlly = true;
            }
            else
            {
                float myFrontX = transform.position.x + GetHalfWidth();
                float frontAllyBackX = frontAlly.transform.position.x - frontAlly.GetHalfWidth();

                float nextMyFrontX = myFrontX + (moveSpeed * Time.deltaTime);
                float stopLineX = frontAllyBackX - minAllyGap;

                if (nextMyFrontX >= stopLineX) isBlockedByAlly = true;
            }
        }

        IDamageable meleeEnemy = FindMeleeEnemy(moveDir);
        if (meleeEnemy != null && !isBlockedByAlly)
        {
            currentTarget = meleeEnemy;
            ChangeState(UnitState.Attacking);
            return;
        }

        if (unitType == UnitType.Ranged)
        {
            IDamageable rangedEnemy = FindRangedEnemy();
            if (rangedEnemy != null)
            {
                currentTarget = rangedEnemy;
                ChangeState(isBlockedByAlly ? UnitState.IdleAttacking : UnitState.WalkAttacking);
                return;
            }
        }

        if (isBlockedByAlly)
        {
            currentTarget = null;
            ChangeState(UnitState.Idle);
            return;
        }

        currentTarget = null;
        ChangeState(UnitState.Moving);
    }

    protected virtual void ExecuteState()
    {
        Vector3 moveDir = isLeft ? Vector3.left : Vector3.right;

        switch (currentState)
        {
            case UnitState.Moving:
            case UnitState.WalkAttacking:
                transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);
                break;

            case UnitState.Attacking:
            case UnitState.IdleAttacking:
                break;
        }
    }

    public void OnAttackHitFrame()
    {
        if (currentTarget != null && !currentTarget.IsDead)
        {
            currentTarget.TakeDamage(attackDamage);
        }

        PlayAttackSound(isRanged: false);
    }

    public void OnRangedAttackFrame()
    {
        if (currentTarget != null && !currentTarget.IsDead)
        {
            currentTarget.TakeDamage(attackDamage);
        }

        PlayAttackSound(isRanged: true);
    }

    private void PlayAttackSound(bool isRanged)
    {
        if (SoundManager.Instance == null) return;

        if (isRanged && rangedAttackSound != null)
        {
            SoundManager.Instance.PlaySFX(rangedAttackSound);
        }
        else if (attackSound != null)
        {
            SoundManager.Instance.PlaySFX(attackSound);
        }
    }


    protected UnitControl GetFrontAlly(Vector2 moveDir)
    {
        Vector2 rayOrigin = (Vector2)transform.position + Vector2.up * 0.5f - moveDir * 0.5f;
        float checkDistance = 15f;

        RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, moveDir, checkDistance, allyLayer);

        UnitControl closestAlly = null;
        float minDistance = Mathf.Infinity;

        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject != gameObject)
            {
                UnitControl ally = hit.collider.GetComponent<UnitControl>();
                if (ally != null && !ally.IsDead)
                {
                    bool isFront = isLeft ? (ally.transform.position.x < transform.position.x)
                                          : (ally.transform.position.x > transform.position.x);

                    if (isFront)
                    {
                        float dist = Mathf.Abs(ally.transform.position.x - transform.position.x);
                        if (dist < minDistance)
                        {
                            minDistance = dist;
                            closestAlly = ally;
                        }
                    }
                }
            }
        }

        return closestAlly;
    }

    protected IDamageable FindMeleeEnemy(Vector2 moveDir)
    {
        Vector2 rayOrigin = (Vector2)transform.position + Vector2.up * 0.5f + moveDir * GetHalfWidth();

        RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, moveDir, meleeAttackRange, enemyLayer);

        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject != gameObject)
            {
                IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                if (damageable != null && !damageable.IsDead)
                {
                    return damageable;
                }
            }
        }
        return null;
    }

    protected IDamageable FindRangedEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, rangedAttackRange, enemyLayer);
        float minDistance = Mathf.Infinity;
        IDamageable nearestEnemy = null;

        foreach (var enemyCol in enemies)
        {
            IDamageable damageable = enemyCol.GetComponent<IDamageable>();
            if (damageable != null && !damageable.IsDead)
            {
                float dist = Vector2.Distance(transform.position, enemyCol.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearestEnemy = damageable;
                }
            }
        }
        return nearestEnemy;
    }

    public void SetSortingOrder(int order)
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = order;
        }
    }

    public virtual void TakeDamage(float damage)
    {
        if (IsInvincible) return;
        if (currentState == UnitState.Dead) return;

        currentHealth -= damage;

        if (healthBar != null) healthBar.UpdateHealthBar();

        PlayBloodEffect();

        if (currentHealth <= 0) Die();
    }

    protected virtual void Die()
    {
        SetColliderActive(false);
        ChangeState(UnitState.Dead);

        PlayDieSound();

        GiveKillReward();

        StartCoroutine(DestroyAfterAnimation());
    }

    private void PlayDieSound()
    {
        if (SoundManager.Instance == null) return;

        if (unitType == UnitType.Tank)
        {
            if (tankDieSound != null)
            {
                SoundManager.Instance.PlaySFX(tankDieSound);
            }
        }
        else
        {
            AudioClip selectedClip = GetRandomDieSound();
            if (selectedClip != null)
            {
                SoundManager.Instance.PlaySFX(selectedClip);
            }
        }
    }

    private AudioClip GetRandomDieSound()
    {
        if (randomDieSounds == null || randomDieSounds.Length == 0) return null;

        List<AudioClip> validClips = new List<AudioClip>();
        foreach (AudioClip clip in randomDieSounds)
        {
            if (clip != null) validClips.Add(clip);
        }

        if (validClips.Count == 0) return null;

        int randomIndex = Random.Range(0, validClips.Count);
        return validClips[randomIndex];
    }

    private void PlayBloodEffect()
    {
        if (bloodParticlePrefab == null) return;

        Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
        GameObject bloodInst = Instantiate(bloodParticlePrefab, spawnPos, Quaternion.identity);

        ParticleSystem ps = bloodInst.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            Destroy(bloodInst, ps.main.duration + ps.main.startLifetime.constantMax);
        }
        else
        {
            Destroy(bloodInst, 1f);
        }
    }
    private IEnumerator DestroyAfterAnimation()
    {
        yield return null;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);
        Destroy(gameObject);
    }

    public void ActivateMovement()
    {
        SetColliderActive(true);
        ChangeState(UnitState.Moving);
    }

    public void SetColliderActive(bool isActive)
    {
        if (unitCollider != null) unitCollider.enabled = isActive;
    }

    public float GetHalfWidth()
    {
        if (unitCollider != null) return unitCollider.bounds.extents.x;
        return 0.5f;
    }

    public void Heal(float amount)
    {
        if (IsDead) return;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        if (healthBar != null) healthBar.UpdateHealthBar();
    }

    private void GiveKillReward()
    {
        MenuManager menuManager = FindObjectOfType<MenuManager>();
        if (menuManager == null) return;

        int goldReward = GameDatabase.KillGoldReward[eraIndex, unitID];
        int expReward = GameDatabase.KillExpReward[eraIndex, unitID];

        bool killerIsLeft = isLeft;

        menuManager.AddReward(killerIsLeft, goldReward, expReward);

        if (GoldPopupManager.Instance != null && goldReward > 0)
        {
            Vector3 popupPos = transform.position;
            if (unitCollider != null)
            {
                popupPos = unitCollider.bounds.center;
            }

            GoldPopupManager.Instance.ShowGoldPopup(popupPos, goldReward);
        }
    }

}