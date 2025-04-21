using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{

    public List<PlayerInput> players = new List<PlayerInput>();
    [SerializeField]
    private List<Transform> startingPoints;
    [SerializeField]
    private List<Transform> deathArea;

    [SerializeField]
    private GameObject player1Join;
    [SerializeField]
    private GameObject player2Join;

    private PlayerInputManager playerInputManager;


    // SCENE DEPENDENCY ALERT: If ANY scene has player manager, it MUST also have 
    // a respawn object with children
    private PlayerRespawnRandomizer playerRespawnRandomizer;

    private void Awake()
    {
        playerInputManager = FindObjectOfType<PlayerInputManager>();
        playerRespawnRandomizer = FindObjectOfType<PlayerRespawnRandomizer>();
        player1Join.SetActive(true);
        player2Join.SetActive(true);
    }

    private void Start()
    {
        EventBus.Publish<PlayerEvent>(new PlayerEvent(players.Count));
    }
    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += AddPlayer;
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= AddPlayer;
    }

    public void AddPlayer(PlayerInput player)
    {
        if(players.Count == 2){
            return;
        }
        players.Add(player);
        player.transform.position = startingPoints[players.Count - 1].position;
        player.transform.rotation = startingPoints[players.Count - 1].rotation;
        if(players.Count == 1)
        {
            // Set camera to the left side of the screen
            player.transform.GetChild(0).GetComponent<Camera>().rect = new Rect(0, 0, .5f, 1);
            player1Join.SetActive(false);
            player.transform.Find("Player Camera").GetComponent<Camera>().cullingMask |= 1 << 9;
        }
        else if(players.Count == 2)
        {
            // Set camera to the right side of the screen
            player.transform.GetChild(0).GetComponent<Camera>().rect = new Rect(.5f, 0, .5f, 1);
            player2Join.SetActive(false);
            player.transform.Find("Player Camera").GetComponent<Camera>().cullingMask |= 1 << 8;
        }
        EventBus.Publish<PlayerEvent>(new PlayerEvent(players.Count));
    }

    public IEnumerator RespawnPlayer(int playerNum)
    {
        PlayerInput deadPlayer;
        deadPlayer = players[playerNum];
        var originalConstraints = deadPlayer.GetComponent<Rigidbody>().constraints;
        

        //making player fall and not be able to be touched
        deadPlayer.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        deadPlayer.transform.Rotate(-10,-5,5);

        //maybe redundant
        deadPlayer.GetComponent<PlayerLook>().enabled = false;
        deadPlayer.GetComponent<PlayerAction>().enabled = false;
        deadPlayer.GetComponent<HasPlayerController>().enabled = false;
        
        //moving camera
        GameObject cam = deadPlayer.GetComponent<PlayerLook>().cam;
        Vector3 inital_position = new Vector3(0,0.65f,0) + deadPlayer.transform.position;
        Vector3 final_position = new Vector3(0,5,0) + deadPlayer.transform.position;
        cam.transform.rotation = Quaternion.Euler(90, 0, 0);
        float duration = 3;

        float inital_time = Time.time;
        float progress = (Time.time - inital_time) / duration;

        while(progress < 1.0f){
            progress = (Time.time - inital_time) / duration;

            Vector3 new_position = Vector3.Lerp(inital_position, final_position, progress);

            cam.transform.position = new_position;

            cam.transform.rotation = Quaternion.Euler(90, 0, 0);

            yield return null;
        }
        
        //resetting player
        //maybe redundant
        deadPlayer.GetComponent<PlayerLook>().enabled = true;
        deadPlayer.GetComponent<PlayerAction>().enabled = true;
        deadPlayer.GetComponent<HasPlayerController>().enabled = true;
        
        deadPlayer.GetComponent<Rigidbody>().constraints = originalConstraints;
        deadPlayer.transform.rotation = Quaternion.identity;

        cam.transform.position = new Vector3(0,0.65f,0);
        cam.transform.rotation = Quaternion.identity;

        deadPlayer.transform.position = playerRespawnRandomizer.GetRandomRespawnLocation();

        yield return null;
    }
}

public class PlayerEvent
{
    public int playerCount;

    public PlayerEvent(int _playerCount) { playerCount = _playerCount; }

    public override string ToString()
    {
        return "There are " + playerCount + " players.";
    }
}