using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float healthChangeDelay = .5f;
    private BaseController baseController;
    private StatHandler statHandler;
    private AnimationHandler animationHandler;
    private float timeSinceLastChange = float.MaxValue;

    public float CurrentHealth { get; set; }
    public float MaxHealth => statHandler.Health;

    private void Awake()
    {
        baseController = GetComponent<BaseController>();
        statHandler = GetComponent<StatHandler>();
        animationHandler = GetComponent<AnimationHandler>();
    }
    private void Start()
    {
        CurrentHealth =statHandler.Health;
    }
    private void Update()
    {
        if (timeSinceLastChange < healthChangeDelay)
        {
            timeSinceLastChange += Time.deltaTime;
            if (timeSinceLastChange >= healthChangeDelay)
            {
                animationHandler.InvincibilityEnd();
            }
        }
    }

    public bool ChangeHealth(float change)
    {
        if (change == 0 || timeSinceLastChange < healthChangeDelay)
        { 
        return false;
        }
        timeSinceLastChange = 0f;
        CurrentHealth += change;
        CurrentHealth = CurrentHealth > MaxHealth ? MaxHealth : CurrentHealth; //MaxHealth 보다 현재 체력이 커지지 않게
        CurrentHealth = CurrentHealth<0?0:CurrentHealth;// 체력이 0 아래로 내려가지 않게 

        if (change < 0)
        {
            animationHandler.Damage();
        }
        if (CurrentHealth <= 0f)
        { Death(); }

        return true;
    }

    private void Death()
    {
        Debug.Log("사망 ");
    }
}
