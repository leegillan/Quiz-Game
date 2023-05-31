using UnityEngine;

public class SFXManager : MonoBehaviour
{
    //Audio sources needed to be played
    public AudioSource wheelSpin;
    public AudioSource catSelect;
    public AudioSource wrongAns;
    public AudioSource correctAns;

    //Class singleton
    private static SFXManager SFXManInstance;
    public static SFXManager Instance { get { return SFXManInstance; } }
    private void Awake()
    {
        if (SFXManInstance != null && SFXManInstance != this)
        {
            Destroy(this);
        }
        else
        {
            SFXManInstance = this;
        }
    }

    public void PlayWheelSpin()
    {
        wheelSpin.Play();
    }

    public void StopWheelSpin()
    {
        wheelSpin.Stop();
    }

    //Pitches the wheel spin sound to create effect of wheel slowing down
    public void AdjustSpinSound(float speed)
    {
        wheelSpin.pitch = speed; //0.5f will reduce the speed so that it takes twice as long for the audio to play.

        wheelSpin.outputAudioMixerGroup.audioMixer.SetFloat("Pitch", 1f / speed);   //readjusts pitch slightly to keep spin not sounding too deep/slowed down
    }

    public void PlayCatSelect()
    {
        catSelect.Play();
    }

    public void PlayWrongAns()
    {
        wrongAns.Play();
    }

    public void PlayCorrectAns()
    {
        correctAns.Play();
    }
}
