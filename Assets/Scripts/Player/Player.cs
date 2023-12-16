using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Player : MonoBehaviour
{
    [SerializeField]private float initialSpeed = 0.5f;
    [SerializeField]private float maxSpeed = 2f;
    private float currentSpeed;
    [SerializeField]LayerMask targetLayers;
    [SerializeField]private float maxStamina = 100f;
    [SerializeField]private float currentStamina;
    [SerializeField]private float staminaDecayRate = 20f;//�ʴ� ���׹̳� ���Ҽ�ġ
    private float regenStaminaPerRate = 10f;
    float maxTiredTime = 2f;//
    float currentTiredTime = 0f;

    [SerializeField] float maxMentalityStat = 100f;//�ִ� ���ŷ�
    float currentMentalityStat = 100f; //���� ���ŷ�
    [SerializeField] float mentalityStatLossPerMinute = 5f;//�д� ���ҷ�

    public float MaxMentalityStat
    {
        get { return maxMentalityStat; }
        set { maxMentalityStat = value; }
    }
    public float CurrentMentalityStat
    {
        get { return currentMentalityStat; }
        set { currentMentalityStat = value; }
    }
    public float MentalityStatLossPerMinute
    {
        get { return mentalityStatLossPerMinute; }
        set { mentalityStatLossPerMinute = value; }
    }

    private enum PlayerStates
    {
        idle,
        isRunning,
        isTired
    }
    private PlayerStates playerState;

    private void Start()
    {
        currentSpeed = initialSpeed;
        playerState = PlayerStates.idle;
        currentStamina = maxStamina;
        currentMentalityStat = maxMentalityStat;
    }

    private void Update()
    {
        if (playerState == PlayerStates.idle)
        {
            currentSpeed = initialSpeed;
            if(currentStamina <= maxStamina)
            {
                currentStamina += regenStaminaPerRate * Time.deltaTime;
            }
        }
        else if (playerState == PlayerStates.isRunning)
        {
            currentStamina -= staminaDecayRate * Time.deltaTime;
            currentSpeed = maxSpeed;
            if (currentStamina <= 0f)
            {
                playerState = PlayerStates.isTired;
            }
        }
        else if(playerState == PlayerStates.isTired)
        {
            currentTiredTime += Time.deltaTime;
            currentSpeed = initialSpeed;
            if (currentTiredTime >= maxTiredTime)
            {
                currentTiredTime = 0f;
                playerState = PlayerStates.idle;
            }
            if (currentStamina <= maxStamina)
            {
                currentStamina += regenStaminaPerRate * Time.deltaTime;
            }
        }
        GameManager.gameManager.setStaminaFillAmount(currentStamina, maxStamina);
    }
    private void FixedUpdate()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector2 moveDistance = new Vector2(x, y);
        moveDistance.Normalize();
        transform.Translate(moveDistance * Time.fixedDeltaTime * currentSpeed);

        //��ɱ���
        if (Input.GetButton("Run"))
        {
            if (playerState != PlayerStates.isTired)
            {
                playerState = PlayerStates.isRunning;
            }
        }
        else
        {
            if (playerState != PlayerStates.isTired)
                playerState = PlayerStates.idle;
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        int collidedLayer = other.gameObject.layer;//�ε��� Object�� layer�� ��ȯ
        if (((1 << collidedLayer) & targetLayers) != 0)
        {
            Vector3 clampedPosition = other.ClosestPoint(transform.position);
            ResetToInitialPosition(clampedPosition);
        }
    }
    //���� ��ġ�� �̵�
    void ResetToInitialPosition(Vector3 clampedPosition)
    {
        transform.position = clampedPosition;
    }

}
