using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineVirtualCamera CVC;
    public static CameraManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        mainCamera = GetComponentInChildren<Camera>();
        CVC = GetComponentInChildren<CinemachineVirtualCamera>();
    }

    public void SetCameraOnPlayer(GameObject Player)
    {
        CVC.Follow = Player.transform;
    }

    private void CameraShake(float timeShake)
    {

    }



}
