using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    protected Rigidbody2D _rigidbody;
    [SerializeField] private SpriteRenderer characterRenderer;
    [SerializeField] private Transform weaponPivot;

    protected Vector2 movementDirection = Vector2.zero; // 이동하는 방향
    public Vector2 MovementDirection { get { return movementDirection; } }

    protected Vector2 lookDirection = Vector2.zero; // 바라보는 방향 
    public Vector2 LookDirection { get { return lookDirection; } }
    private Vector2 knockback = Vector2.zero;
    private float knockbackDuration = 0.0f;

    protected AnimationHandler animationHandler;
    protected StatHandler statHandler;
   
    [SerializeField] public WeaponHandler WeaponPrefab;
    protected WeaponHandler weaponHandler;

    protected bool isAttacking;
    private float timeSinceLastAttack = float.MaxValue;

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        animationHandler = GetComponent<AnimationHandler>();
        statHandler = GetComponent<StatHandler>();

        if (WeaponPrefab != null)
            weaponHandler = Instantiate(WeaponPrefab, weaponPivot);
        else weaponHandler = GetComponentInChildren<WeaponHandler>();
    }
    protected virtual void Start()
    { }
    protected virtual void Update()
    {
        HandleAction();
        Rotate(lookDirection);
        HandleAttackDelay();    

    }

    protected virtual void FixedUpdate() // 이동 담당
    {
        Movement(movementDirection);
        if (knockbackDuration > 0.0f)//넉백시간 점감 연출
        { 
            knockbackDuration -= Time.fixedDeltaTime;//프레임률과 무관하게 0.02초간격
        }
    }
    
    protected virtual void HandleAction()//입력처리, 이동에 필요한 데이터 처리
    { }

    private void Movement(Vector2 direction)
    {
        direction = direction * statHandler.Speed;//입력된 방향 벡터의 크기를 5배로 증폭. 속도값처럼 처리하는 연산
        if (knockbackDuration > 0.0f)
        {
            direction *= 0.2f;
            direction += knockback;
        }
        _rigidbody.velocity = direction;
        //        Rigidbody의 속도(velocity) 를 아예 direction으로 설정
        // 물리적으로 밀어내거나 움직이는 게 아니라 강제로 속도를 지정해서 이동을 만드는 구조
        //넉백 중에도 약간의 조작은 가능하지만,대부분의 힘은 강제로 밀리는 방향으로 흐르게 하는 구조
        animationHandler.Move(direction);

    }

    private void Rotate(Vector2 direction)//캐릭터가 방향전환을 할때 이미지를 해당방향으로 뒤집는 매서드 
    {
        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; //파이 값을 180으로 만드는 작업 ??
        bool isLeft = Mathf.Abs(rotZ) > 90f;

        characterRenderer.flipX = isLeft;
        if (weaponPivot != null)
        {
            weaponPivot.rotation = Quaternion.Euler(0f,0f,rotZ);
        }

        weaponHandler?.Rotate(isLeft);
    }

    public void ApplyKnockback(Transform other, float power, float duration) //knockback의 방향과 시간 매서드
    { 
        knockbackDuration = duration;
// vector A+ vector B = 끝점 잇기   vector A - vector B =  B를 뒤집기. 결과물이 B가 A를 바라보는 방향이 나옴 
        knockback = -(other.position - transform.position).normalized * power; // 방향만 필요하기에 nomalized를 이용해 vector의 길이를 1로 만듦
    }

    private void HandleAttackDelay() // 일정 시간마다 공격을 호출하게 
    {
        if (weaponHandler == null)
            return;

        if (timeSinceLastAttack <= weaponHandler.Delay)
        {
            timeSinceLastAttack += Time.deltaTime;
        }

        if (isAttacking && timeSinceLastAttack > weaponHandler.Delay)
        {
            timeSinceLastAttack = 0;
            Attack();
        }
    }

    protected virtual void Attack()
    {
        if (lookDirection != Vector2.zero)
            weaponHandler?.Attack();
    }

}




