using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlatformScene : MonoBehaviour
{

    public static PlatformScene Me;
    public MarthaController Martha;
    public bool IsGameOn;
    public bool AllowEvilStuff = true;
    public bool AllowEnergyBalls = true;
    public bool MuteSound = false;
    public Transform zombieSpawnPoint;
    static int waveZombieMultiplier = 4;
    int zombiesSpawned;
    int zombiesKilled;
    public bool AllowPresentation = true;
    public int Wave = 1;


    public int ZombiesKilled
    {
        get
        {
            return zombiesKilled;
        }
        set
        {
            zombiesKilled = value;

            if (zombiesKilled == Wave * waveZombieMultiplier)
            {

                StartCoroutine(WaveCleared_cr());
            }
        }
    }


    void Awake()
    {

        MainGame.state = MainGameState.PlatformGame;
        Me = this;

    }

    void Start()
    {
        if (MuteSound)
        {
            SFXMan.MusicVolume = 0;

            SFXMan.SfxVolume = 0;
        }

        if (!AllowPresentation)
        {
            GUI.Me.messageDisplay.gameObject.SetActive(false);
        }

        StartCoroutine(InitiateWave_cr());//	InitiateWave ();

    }




    IEnumerator InitiateWave_cr()
    {

        //Initiate game variables       
        ZombiesKilled = 0;
        zombiesSpawned = 0;

        if (AllowPresentation)
        {
            //PRESENT WAVE
            SFXMan.sfx_StartWave.Play();
            string presentation = string.Format("Wave {0}", Wave);
            GUI.Me.ShowPresentation(presentation);
            yield return new WaitForSeconds(3);

            presentation = string.Format("Kill {0} Zombies! ", Wave * 3);
            GUI.Me.ShowPresentation(presentation);
            yield return new WaitForSeconds(1);
        }

        // ONLY ON FIRST WAVE
        if (Wave == 1)
        {

            Debug.Log("Creating a Martha");
            //SETUP PLAYER
            if (!Martha)
            {
                Martha = (Instantiate(Prefabs.Martha) as GameObject).GetComponent<MarthaController>() as MarthaController;
            }

            //START EVIL PLANS
            InvokeRepeating("DoEvilStuff", 1.0f, 2.0f);

            //START PICKUP PLAN
            InvokeRepeating("SpawnPickups", 1.0f, 4.0f);

            SFXMan.PlayAsSong(SFXMan.sng_PlatformSong);

        }
        Martha.transform.position = new Vector3(10, 10);
        Martha.Initialize();
        IsGameOn = true;

    }

    void SpawnPickups()
    {
        if (AllowEnergyBalls)
        {
            GameObject pickup = Instantiate(Prefabs.Pickup, new Vector2(0, 0), Quaternion.identity) as GameObject;
            SFXMan.sfx_Shot.Play();
        }
    }

    IEnumerator WaveCleared_cr()
    {
        IsGameOn = false;
        //	EnableAllCharacters (false);
        SFXMan.StopSong();
        yield return new WaitForSeconds(0.5f);
        RemoveAllZombies(false);

        SFXMan.sfx_ZombieWaveCleared.Play();
        GUI.Me.ShowPresentation("Wave " + Wave + " Cleared! ");
        yield return new WaitForSeconds(2);
        Wave++;
        yield return StartCoroutine(InitiateWave_cr());

    }

    private static void RemoveAllZombies(bool blodlessRemoval)
    {
        var killList = new List<ZombieBoyController>();
        foreach (var ac in ActiveCharacters)
        {
            if (ac)
            {
                ZombieBoyController zombieBoy = ac.GetComponent<ZombieBoyController>();
                if (zombieBoy)
                {
                    killList.Add(zombieBoy.GetComponent<ZombieBoyController>());
                }
            }
        }


        foreach (var zombieBoy in killList)
        {
            if (zombieBoy)
            {
                if (blodlessRemoval)
                {
                    DestroyImmediate(zombieBoy.gameObject);
                }
                else
                {
                    zombieBoy.Die();
                }
            }
        }
    }

    public void MarthaDies()
    {
        StartCoroutine(GameOver_cr());
    }

    public IEnumerator GameOver_cr()
    {
        if (IsGameOn)
        {
            PlatformScene.Me.IsGameOn = false;
            Martha.GetComponent<Collider2D>().enabled = false;
            SFXMan.StopSong();
            SFXMan.sfx_MarthaDeath.Play();
            yield return new WaitForSeconds(1);
            SFXMan.sfx_GameOver.Play();
            GUI.Me.ShowPresentation("U R Looser !");
            yield return new WaitForSeconds(3);
            RemoveAllZombies(true);
            Martha.gameObject.SetActive(false);
            StartCoroutine(InitiateWave_cr());
        }
    }

    void DoEvilStuff()
    {

        if (AllowEvilStuff && IsGameOn)
        {
            if (zombiesSpawned < Wave * waveZombieMultiplier)
            {

                GameObject zombie = Instantiate(Prefabs.ZombieBoy, zombieSpawnPoint.transform.position, Quaternion.identity) as GameObject;

                var newX = Random.Range(-10, 10);
                var newY = Random.Range(3, 10);
                transform.position = new Vector3(newX, newY, 0);
                Blip2D.BlipAtPosition(transform.position, 0.5f);

                // Give push to random direction
                zombie.GetComponent<ZombieBoyController>().GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 500));

                zombiesSpawned++;
            }
        }
    }


    #region STATIC MEMBERS
    static List<MonoBehaviour> ActiveCharacters = new List<MonoBehaviour>();


    public static void AddActiveCharacter(MonoBehaviour character)
    {
        ActiveCharacters.Add(character);
    }




    public static void RemoveCharacterFromActiveList(MonoBehaviour character)
    {
        ActiveCharacters.Remove(character);
    }
}
#endregion