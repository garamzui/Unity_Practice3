using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    protected Rigidbody2D _rigidbody;
    [SerializeField] private SpriteRenderer characterRenderer;
    [SerializeField] private Transform weaponPivot;

    protected Vector2 movementDirection = Vector2.zero; // �̵��ϴ� ����
    public Vector2 MovementDirection { get { return movementDirection; } }

    protected Vector2 lookDirection = Vector2.zero; // �ٶ󺸴� ���� 
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

    protected virtual void FixedUpdate() // �̵� ���
    {
        Movement(movementDirection);
        if (knockbackDuration > 0.0f)//�˹�ð� ���� ����
        { 
            knockbackDuration -= Time.fixedDeltaTime;//�����ӷ��� �����ϰ� 0.02�ʰ���
        }
    }
    
    protected virtual void HandleAction()//�Է�ó��, �̵��� �ʿ��� ������ ó��
    { }

    private void Movement(Vector2 direction)
    {
        direction = direction * statHandler.Speed;//�Էµ� ���� ������ ũ�⸦ 5��� ����. �ӵ���ó�� ó���ϴ� ����
        if (knockbackDuration > 0.0f)
        {
            direction *= 0.2f;
            direction += knockback;
        }
        _rigidbody.velocity = direction;
        //        Rigidbody�� �ӵ�(velocity) �� �ƿ� direction���� ����
        // ���������� �о�ų� �����̴� �� �ƴ϶� ������ �ӵ��� �����ؼ� �̵��� ����� ����
        //�˹� �߿��� �ణ�� ������ ����������,��κ��� ���� ������ �и��� �������� �帣�� �ϴ� ����
        animationHandler.Move(direction);

    }

    private void Rotate(Vector2 direction)//ĳ���Ͱ� ������ȯ�� �Ҷ� �̹����� �ش�������� ������ �ż��� 
    {
        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; //���� ���� 180���� ����� �۾� ??
        bool isLeft = Mathf.Abs(rotZ) > 90f;

        characterRenderer.flipX = isLeft;
        if (weaponPivot != null)
        {
            weaponPivot.rotation = Quaternion.Euler(0f,0f,rotZ);
        }

        weaponHandler?.Rotate(isLeft);
    }

    public void ApplyKnockback(Transform other, float power, float duration) //knockback�� ����� �ð� �ż���
    { 
        knockbackDuration = duration;
// vector A+ vector B = ���� �ձ�   vector A - vector B =  B�� ������. ������� B�� A�� �ٶ󺸴� ������ ���� 
        knockback = -(other.position - transform.position).normalized * power; // ���⸸ �ʿ��ϱ⿡ nomalized�� �̿��� vector�� ���̸� 1�� ����
    }

    private void HandleAttackDelay() // ���� �ð����� ������ ȣ���ϰ� 
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




