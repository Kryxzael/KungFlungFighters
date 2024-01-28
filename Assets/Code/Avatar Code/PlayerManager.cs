using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public CurrentAction currentAction; 

    [Header("Components")]
    public Rigidbody2D rb;
    public GameObject leftPunchHitBox;
    public GameObject rightPunchHitBox;
    public SpriteRenderer sprite;
    public Image healthBar;

    [Header("Health")]
    public float maxHealth;

    public float health;
    public float invincibilityTime = 5f;
    public bool isInvincible;

    [Header("Punch Force")]
    public float minPunchForce;
    public float maxPunchForce;
    
    public float MinPunchTorque;
    public float MaxPunchTorque;
    public float MaxRecoveryTorque;
    
    public float jumpForce;

    [Header("Input")]
    public string xMovementAxisName;
    public string yMovementAxisName;

    [Header("Gravity")]
    public float maxGravityScale;
    public float minGravityY;
    public float maxGravityY;

    [Header("Sound players")]
    public AudioSource rightAttackAudio;
    public AudioSource leftAttackAudio;
    public AudioSource jumpAudio;
    public AudioSource hitAudio;
    public AudioSource deathAudio;

    [Header("Etc.")]
    public float punchCooldown = 0.25f;
    public bool isPlayer2;

    public float maxVelocity;
    public float maxTorque;


    void Start()
    {
        health = maxHealth;
        leftPunchHitBox.gameObject.SetActive(false);
        rightPunchHitBox.gameObject.SetActive(false);
    }

    void Update()
    {
        //Gets input from user's keyboard
        Vector2 input = new Vector2(Input.GetAxisRaw(xMovementAxisName), Input.GetAxisRaw(yMovementAxisName));

        //If not punching and pressing button to punch
        if (currentAction == CurrentAction.Idle && input.magnitude > 0.25f)
        {
            /*
             * Get rotation data
             */

            //Adjust rotation from 0..360 to -180..180
            float adjustedRotation = transform.eulerAngles.z;

            if (adjustedRotation > 180f)
            {
                adjustedRotation = 360f - adjustedRotation;
            }

            //Convert to percent
            adjustedRotation /= 360f;

            //Schedule end of punch after a punch cooldown
            Invoke("StopPunch", punchCooldown);
            Invoke("DisablePunchColliders", 0.1f);

            if (input.y > 0.25f)
            {
                if (Math.Abs(adjustedRotation) >= 0.1f)
                {
                    rb.AddTorque(MaxRecoveryTorque * -adjustedRotation);
                }
                else
                {
                    rb.AddForce(transform.up * jumpForce);
                }

                currentAction = CurrentAction.Jump;
                jumpAudio.Play();

                return;
            }
            else if (input.x > 0.25f)
            {
                rightPunchHitBox.gameObject.SetActive(true);
                currentAction = CurrentAction.AttackRight;
                rightAttackAudio.Play();
            }
            else
            {
                leftPunchHitBox.gameObject.SetActive(true);
                currentAction = CurrentAction.AttackLeft;
                leftAttackAudio.Play();
            }

            //Perform the punch
            rb.AddForce(input * UnityEngine.Random.Range(minPunchForce, maxPunchForce) * transform.right);

            /*
             * Add rotation
             */

            //Add recovery rotation
            if (Mathf.Abs(adjustedRotation) >= 0.075f)
            {
                float torque = MaxRecoveryTorque;

                rb.AddTorque(adjustedRotation * torque * -input.x);
            }

            //Add random "tripping" rotation
            else 
            {
                rb.AddTorque(adjustedRotation * UnityEngine.Random.Range(MinPunchTorque, MaxPunchTorque) * -input.x);
            }
        }
    }

    private void FixedUpdate()
    {
        //Cap speeds
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxVelocity, maxVelocity), rb.velocity.y);
        rb.angularVelocity = Mathf.Clamp(rb.angularVelocity, -maxTorque, maxTorque);

        rb.gravityScale = Mathf.Lerp(            
            a: 1f, 
            b: maxGravityScale, 
            t: Mathf.InverseLerp(
                a: minGravityY, 
                b: maxGravityY, 
                value: transform.position.y
            )
        );
    }

    private void DisablePunchColliders()
    {
        leftPunchHitBox.gameObject.SetActive(false);
        rightPunchHitBox.gameObject.SetActive(false);
    }

    private void StopPunch()
    {
        //Stops a punch
        if (currentAction != CurrentAction.Stunned)
            currentAction = CurrentAction.Idle;
    }

    private void StopStun()
    {
        if (currentAction == CurrentAction.Stunned)
            currentAction = CurrentAction.Idle;
    }

    public void DealDamage(float damage)
    {
        if (isInvincible || health <= 0)
            return;

        health -= damage;
        currentAction = CurrentAction.Stunned;

        if (health <= 0)
        {
            foreach (Collider2D i in GetComponents<Collider2D>())
            {
                i.enabled = false;
            }

            currentAction = CurrentAction.Stunned;
            Time.timeScale = 0.5f;

            DeathLabelController deathLabelController = FindObjectOfType<DeathLabelController>();

            if (isPlayer2)
                deathLabelController.player1Win.SetActive(true);

            else
                deathLabelController.player2Win.SetActive(true);

            deathAudio.Play();
            Invoke("RestartGame", 3f);
        }
        else
        {
            hitAudio.Play();
        }

        StartCoroutine(CoInvicibilityFrames());
        Invoke("StopStun", 1f);
    }

    private IEnumerator CoInvicibilityFrames()
    {
        float invincibilityTimeLeft = invincibilityTime;
        isInvincible = true;

        while (invincibilityTimeLeft >= 0f)
        {
            sprite.color = new Color(invincibilityTimeLeft / invincibilityTime, 0f, 0f);

            yield return new WaitForFixedUpdate();
            invincibilityTimeLeft -= Time.fixedDeltaTime;

        }

        isInvincible = false;
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
    }

    public enum CurrentAction
    {
        Idle,
        Jump,
        AttackLeft,
        AttackRight,
        Stunned
    }
}
