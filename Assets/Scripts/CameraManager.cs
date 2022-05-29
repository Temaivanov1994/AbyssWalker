using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineVirtualCamera CVC;
    [SerializeField] private CinemachineBasicMultiChannelPerlin CBMCP;
    [SerializeField] private Image blowImage;
    public static CameraManager instance;

   

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        mainCamera = GetComponentInChildren<Camera>();
        CVC = GetComponentInChildren<CinemachineVirtualCamera>();
        CBMCP = GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();
        blowImage = GetComponentInChildren<Image>();
    }

    public void SetCameraOnPlayer(GameObject Player)
    {
        CVC.Follow = Player.transform;
    }

    public IEnumerator CameraShake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            CBMCP.m_AmplitudeGain = magnitude;

            elapsed += Time.deltaTime;
            yield return null;
        }
        CBMCP.m_AmplitudeGain = 0;

    }

    public void CameraBlow(float duration, float frequancy)
    {
       
    }

}
