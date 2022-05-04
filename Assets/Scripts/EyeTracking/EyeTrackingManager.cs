using UnityEngine;
using ViveSR.anipal.Eye;
using System.Runtime.InteropServices;
using UnityEngine.Assertions;
using Assets.LSL4Unity.Scripts;

public class EyeTrackingManager : MonoBehaviour, IPhysiologyDevice
{
    [SerializeField] private LineRenderer gazeLineRenderer = default;

    private bool eyeCallbackRegistered = false;
    private Vector3 gazeOriginLocal;
    private Vector3 gazeDirectionLocal;
    private Vector3 gazeOriginWorld;
    private Vector3 gazeDirectionWorld;

    public static EyeData MyEyeData { get; private set; } = new EyeData();

    // Local Coordinates Properties
    public Vector3 GazeOriginLocal => gazeOriginLocal;
    public Vector3 GazeDirectionLocal => gazeDirectionLocal;

    // World Coordinates Properties
    public Vector3 GazeOriginWorld => gazeOriginWorld;
    public Vector3 GazeDirectionWorld => gazeDirectionWorld;

    public string DeviceName => "EYE_TRACKING";

    public bool DeviceIsReady { get; set; }

    private EyeTrackingGazeData trackingData;
    private LSLMarkerStream lslOut;

    private void Awake()
    {
        if (!SRanipal_Eye_Framework.Instance.EnableEye)
        {
            enabled = false;
            return;
        }

        Assert.IsNotNull(gazeLineRenderer);
    }

    private void Start()
    {
        lslOut = GetComponent<LSLMarkerStream>();
    }
    private void Update()
    { 
        
        if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING &&
            SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) return;

        if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eyeCallbackRegistered == false)
        {
            SRanipal_Eye.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
            eyeCallbackRegistered = true;
        }
        else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eyeCallbackRegistered == true)
        {
            SRanipal_Eye.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
            eyeCallbackRegistered = false;
        }

        if (eyeCallbackRegistered)
        {
            if (SRanipal_Eye.GetGazeRay(GazeIndex.COMBINE, out gazeOriginLocal, out gazeDirectionLocal, MyEyeData)) { }
            else if (SRanipal_Eye.GetGazeRay(GazeIndex.LEFT, out gazeOriginLocal, out gazeDirectionLocal, MyEyeData)) { }
            else if (SRanipal_Eye.GetGazeRay(GazeIndex.RIGHT, out gazeOriginLocal, out gazeDirectionLocal, MyEyeData)) { }
            else return;
        }
        else
        {
            if (SRanipal_Eye.GetGazeRay(GazeIndex.COMBINE, out gazeOriginLocal, out gazeDirectionLocal)) { }
            if (SRanipal_Eye.GetGazeRay(GazeIndex.LEFT, out gazeOriginLocal, out gazeDirectionLocal)) { }
            if (SRanipal_Eye.GetGazeRay(GazeIndex.RIGHT, out gazeOriginLocal, out gazeDirectionLocal)) { }
            else return;
        }

        // Get world space direction coords instead of local
        gazeDirectionWorld = Camera.main.transform.TransformDirection(gazeDirectionLocal);

        gazeLineRenderer.SetPosition(0, Camera.main.transform.position - Camera.main.transform.up * 0.05f);
        gazeLineRenderer.SetPosition(1, Camera.main.transform.position + GazeDirectionWorld * 1000f);
    }

    public void Calibrate()
    {
        SRanipal_Eye_v2.LaunchEyeCalibration();
        DeviceIsReady = true;
    }

    public void SkipCalibrate()
    {
        DeviceIsReady = true;

    }

    private void Release()
    {
        if (eyeCallbackRegistered == true)
        {
            SRanipal_Eye.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
            eyeCallbackRegistered = false;
        }
    }
    private static void EyeCallback(ref EyeData eyeData)
    {
        MyEyeData = eyeData;
    }

    public void StartAcquisition(string fileName)
    {
       foreach(Transform child in transform)
        {
            if(child.gameObject.GetComponent<EyeTrackingGazeData>())
            {
                trackingData = child.gameObject.GetComponent<EyeTrackingGazeData>();
                trackingData.StartRecording("EyeTrackingData");
                Debug.Log("Eye tracking data start");
            }
        }
    }

    public void StopAcquisition()
    {
        trackingData.StopRecording();
    }

    public void WriteEvent(string id)
    {
        Debug.Log("write event eye tracking");
    }

    public void StartLSL()
    {
        lslOut.enabled = true;
        Debug.Log("" + lslOut.lslStreamName + "is now on");
    }
}
