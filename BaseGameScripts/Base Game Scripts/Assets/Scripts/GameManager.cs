using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameManager
{
    #region Variables

    public PlayerData playerData;

    #endregion

    #region Properties

    //pauseable object list to store all objects that need to be paused
    public List<PauseableObject> PauseableObjects { get; set; }
    private bool isPaused;

    //paused property for objects to see if the game is paused or not
    public bool Paused
    {
        get
        {
            return isPaused;
        }
        set
        {
            //see if the existing and incoming values are the same
            if (value && isPaused)
            {
                return;
            }

            //update isPaused to the new value
            isPaused = value;

            //check if the game is now paused
            if (isPaused)
            {
                //TODO: Pause Audio Manager

                //cycle through each object in th epauseable objects list and pause them
                foreach (PauseableObject pauseObject in PauseableObjects)
                {
                    pauseObject.PauseObject();
                }
            }
            else
            {
                //TODO: Unpause Audio Manager

                //cycle through each object in th epauseable objects list and un-pause them
                foreach (PauseableObject pauseObject in PauseableObjects)
                {
                    pauseObject.UnPauseObject();
                }
            }
        }
    }

    #endregion

    #region Singleton Constructor

    // create variable for storing singleton that any script can access
    private static GameManager instance;

    // create GameManager
    private GameManager()
    {
        //create internal updater object
        UnityEngine.Object.DontDestroyOnLoad(new GameObject("Updater", typeof(Updater)));

        //create list of pausable objects
        PauseableObjects = new List<PauseableObject>();
    }

    // Property for Singleton
    public static GameManager Instance
    {
        get { return instance ?? (instance = new GameManager()); }
    }

    #endregion

    #region Internal Updater Class

    //class to allow the gamemanager singleton to have an update method
    class Updater : MonoBehaviour
    {
        private void Start()
        {
            Instance.Start();
        }

        private void Update()
        {
            Instance.Update();
        }
    }

    #endregion

    #region Start

    private void Start()
    {
        playerData = new PlayerData(0, 0, 0);
    }

    #endregion

    #region Update

    private void Update()
    {

    }

    #endregion

    #region Public Methods

    //method to add an object to the pauseable objects list
    public void AddPausableObject(PauseableObject pauseObject)
    {
        PauseableObjects.Add(pauseObject);
    }

    //method to remmove an object to the pauseable objects list
    public void RemovePausableObject(PauseableObject pauseObject)
    {
        PauseableObjects.Remove(pauseObject);
    }

    public void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/PlayerInfo.dat", FileMode.OpenOrCreate);

        formatter.Serialize(file, playerData);
        file.Close();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/PlayerInfo.dat"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/PlayerInfo.dat", FileMode.Open);
            PlayerData data = (PlayerData)formatter.Deserialize(file);

            playerData.Level = data.Level;
            playerData.Experience = data.Experience;
            playerData.Score = data.Score;

            file.Close();
        }
    }

    #endregion
}

#region PauseableObject class

public class PauseableObject : MonoBehaviour
{
    #region Variables

    protected Rigidbody2D rb; //stores the rigidbody of the gameobject
    protected Animator anim;  //stores the animator of the object

    Vector2 storedVelocity; //stores the current velocity of the object
    bool isSimulated;       //stores the simulated value from the rigidbody

    #endregion

    #region Awake

    protected virtual void Awake()
    {
        //check if the object has a rigidbody, and if it does store it
        if (GetComponent<Rigidbody2D>())
        {
            rb = GetComponent<Rigidbody2D>();
        }

        //check if the object has an animator, and if it does store it
        if (GetComponent<Animator>())
        {
            anim = GetComponent<Animator>();
        }
        else if (GetComponentInChildren<Animator>())
        {
            anim = GetComponentInChildren<Animator>();
        }

        GameManager.Instance.AddPausableObject(this);
    }

    #endregion

    #region Private Methods

    //called when the object gets destroyed from the scene
    private void OnDestroy()
    {
        //remove this object from the pauseable objects list in gamemanager
        GameManager.Instance.RemovePausableObject(this);
    }

    #endregion

    #region Public Methods

    public void PauseObject()
    {
        if (rb)
        {
            storedVelocity = rb.velocity;
            isSimulated = rb.simulated;

            rb.velocity = Vector2.zero;
            rb.simulated = false;
        }

        if (anim)
        {
            anim.speed = 0f;
        }
    }

    public void UnPauseObject()
    {
        if (rb)
        {
            rb.simulated = isSimulated;
            rb.velocity = storedVelocity;
        }

        if (anim)
        {
            anim.speed = 1f;
        }
    }

    #endregion
}

#endregion

#region PlayerData Class

[Serializable]
public class PlayerData
{
    public int Level { get { return level; } set { level = value; } }
    public float Experience { get { return experience; } set { experience = value; } }
    public float Score { get { return score; } set { score = value; } }

    private int level;
    private float experience;
    private float score;

    public PlayerData(int level, float experience, int score)
    {
        this.level = level;
        this.experience = experience;
        this.score = score;
    }
}

#endregion