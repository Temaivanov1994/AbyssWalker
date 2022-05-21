using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineVirtualCamera CVC;
    private void Awake()
    {
        mainCamera = GetComponentInChildren<Camera>();
        CVC = GetComponentInChildren<CinemachineVirtualCamera>();
    }

    private void SetCameraOnPlayer(GameObject Player)
    {
        CVC.Follow = Player.transform;
    }

    private void CameraShake(float timeShake)
    {

    }



}
