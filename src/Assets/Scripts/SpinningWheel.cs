using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpinningWheel : MonoBehaviour
{
    //The min/max speeds for the wheel
    [SerializeField] private float minSpeed = 800f;
    [SerializeField] private float maxSpeed = 1000f;

    //The min/max time it takes before the wheel begins to slowdown
    [SerializeField] private float minSlowdownTime = 1f;
    [SerializeField] private float maxSlowdownTime = 3f;

    //The min/max rate at which the wheel will slow down
    [SerializeField] private float minSlowdownRate = 5f;
    [SerializeField] private float maxSlowdownRate = 50f;

    //The current speed of the wheel
    [SerializeField] private float currentSpeed;

    //The time it takes before the wheel begins to slowdown to allow for wheel to spin freely for short period
    [SerializeField] private float slowdownTime;

    //The rate at which it takes for the wheel to slow down
    [SerializeField] private float slowdownRate;

    //Current angle of the wheel
    [SerializeField] private float currentAngle = 0f;

    [SerializeField] private bool isSpinning = false;
    [SerializeField] private bool allowSpin = true;

    public void SetAllowSpin (bool b) { allowSpin = b; }    //Used to stop spam after category selction

    void Start()
    {
        allowSpin = true;
    }

    void Update()
    {
        if (isSpinning)
        {
            currentAngle += currentSpeed * Time.deltaTime;  //Increase current angle by the current speed

            //Updates category UI whilst wheel is spinning
            GameManager.Instance.ChooseCategory(currentAngle);
            UIManager.Instance.SetCategoryText();
            
            transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);    //Set the rotation of the wheel based on the current angle

            //If the slowdown time has reached zero, start slowing down the wheel
            if (slowdownTime <= 0f)
            {
                currentSpeed -= (slowdownRate * Time.deltaTime) * 10; 

                SFXManager.Instance.AdjustSpinSound(currentSpeed / maxSpeed);   //Slows down spinning sound to adjust for wheel speed

                //If the current speed has reached zero, stop the wheel and choose category
                if (currentSpeed <= 0f)
                {
                    SFXManager.Instance.StopWheelSpin();
                    currentSpeed = 0f;
                    isSpinning = false;

                    StartCoroutine(GameManager.Instance.SpinBuffer()) ; 
                }
            }
            else
            {
                slowdownTime -= Time.deltaTime;
            }

            //Make sure angle is not outwidth required margins for checking category
            if (currentAngle >= 360.0f)
            {
                currentAngle = 0f;
            }
            else if (currentAngle <= 0.0f)
            {
                currentAngle = 360f;
            }
        }
    }

    //Start spinning the wheel and choose random values to be used during spin
    public void StartSpin()
    {
        if (!isSpinning && allowSpin)
        {
            //Set the current speed to a random value between the minimum and maximum speed
            currentSpeed = Random.Range(minSpeed, maxSpeed);

            //Set the slowdown time to a random value between the minimum and maximum slowdown time
            slowdownTime = Random.Range(minSlowdownTime, maxSlowdownTime);

            //Set the slowdown rate to a random value between the minimum and maximum slowdown rate
            slowdownRate = Random.Range(minSlowdownRate, maxSlowdownRate);

            SFXManager.Instance.AdjustSpinSound(1.0f);
            SFXManager.Instance.PlayWheelSpin();

            //Set the flag to indicate that the wheel is spinning and if it can be spun again
            isSpinning = true;
            allowSpin = false;
        }
    }
}