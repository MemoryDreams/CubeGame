using UnityEngine;

public class ParticleSystemController : MonoBehaviour
{
    public ParticleSystem particleMachine;
    private float onDuration = 0.2f;
    private float offDuration = 0.8f;

    public float timer = 0f;
    public bool isParticleSystemActive;
    public bool turnedOn = false;
    public float SystemController;

    private void Start()
    {
    }

    private void Update()
    {
        if (particleMachine == null)
        {
            particleMachine = gameObject.GetComponent<ParticleSystem>();
        }
        if (turnedOn)
        {
            timer += Time.deltaTime;
            if (isParticleSystemActive && timer>onDuration)
            {
                turnOff();
            }
            else if (!isParticleSystemActive && timer>offDuration)
            {
                turnOn();
            }
        } 
        else 
        {
            turnOff();
        }
    }

    public void turnOff()
    {
        particleMachine.Stop();
        isParticleSystemActive = false;
        timer = 0f;
    }

    public void turnOn()
    {
        particleMachine.Play();
        isParticleSystemActive = true;
        timer = 0f;
    }
}
