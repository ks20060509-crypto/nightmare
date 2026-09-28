using UnityEngine;

public class Weapon : MonoBehaviour
{
    public AudioSource weapon;
    public AudioClip reloadingSe, fireSe, triggerSe;

    public static Weapon instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FireSe()
    {
        weapon.clip = fireSe;
        weapon.Play();
    }

    public void ReloadingSe()
    { weapon.clip = reloadingSe;
        weapon.Play();
    }

    public void TriggerSe()
    {
        weapon.clip = triggerSe;
        weapon.Play();



    }

}
