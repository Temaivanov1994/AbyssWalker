using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameState gameState;
    [SerializeField] private GameObject currentPlayer;
    [SerializeField] private Transform playerSpawnerPoint;
    private void Awake()
    {
        // createPlayerPoint =  найти место где спавнить
    }
    private void Start()
    {
        CreatePlayer(currentPlayer);
    }



    private void CreatePlayer(GameObject Charter)
    {
        GameObject CreatedCharter = Instantiate(Charter, playerSpawnerPoint.position, Quaternion.identity);
        CameraManager.instance.SetCameraOnPlayer(CreatedCharter);
    }
   


}

    enum GameState
    {
        MainMenu,
        GamePlay
    }