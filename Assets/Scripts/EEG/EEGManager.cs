using UnityEngine;
using System;
using System.IO;
using System.Text;
using Looxid.Link;
using System.Collections;
using Assets.LSL4Unity.Scripts;


public class EEGManager : MonoBehaviour, IPhysiologyDevice
{
    [Header("Raw Signal")]
    [SerializeField] private string chartHostTag;
    [SerializeField] private LineChart AF3Chart = null;
    [SerializeField] private LineChart AF4Chart = null;
    [SerializeField] private LineChart Fp1Chart = null;
    [SerializeField] private LineChart Fp2Chart = null;
    [SerializeField] private LineChart AF7Chart = null;
    [SerializeField] private LineChart AF8Chart = null;

    private EEGCSVRecorder csvRec;
    public LSLOutlet lslOut;
    private bool lslReady = false;

    public string DeviceName => "EEG_VR";

    public bool DeviceIsReady { get; set; }

    private void Awake()
    {
        csvRec = new EEGCSVRecorder();
       
    }

    private void Start()
    {
        LooxidLinkManager.OnLinkCoreConnected += OnLinkCoreConnected;
        LooxidLinkManager.OnLinkCoreDisconnected += OnLinkCoreDisconnected;
        LooxidLinkManager.OnLinkHubConnected += OnLinkHubConnected;
        LooxidLinkManager.OnLinkHubDisconnected += OnLinkHubDisconnected;
        lslOut = GetComponent<LSLOutlet>();
    }

    public bool Initialize() => LooxidLinkManager.Instance.Initialize();
    public void Terminate() => LooxidLinkManager.Instance.Terminate();

    public void StartAcquisition(string fileName = null)
    {
        if(!Initialize())
        {
            Debug.Log("Initialize wasnt done doing now");
            Initialize();
        }
        else
        {
            Debug.Log("Initialize done previous ignoring now");
        }
        
        Debug.Log("Start aquisicion");
        lslReady = true;
        if (fileName != null)
        {
            csvRec.CreateFile(fileName);
        }
    }

    public void TestSignal()
    {
        Initialize();
    }


    public void StopAcquisition()
    {
        Terminate();
        StartCoroutine(WaitForTerminate());
    }

    IEnumerator WaitForTerminate()
    {
        yield return new WaitForSeconds(3);
        csvRec.CloseFile();
        Debug.Log("EEG file closed");
    }

    public void Confirm() 
    {
        DeviceIsReady = true;
    }

    



    private void Update()
    {
        if (!LooxidLinkManager.Instance.isLinkCoreConnected) return;
        if (!LooxidLinkManager.Instance.isLinkHubConnected) return;
    }

    private void OnEnable()
    {
        LooxidLinkData.OnReceiveEEGRawSignals += OnReceiveEEGRawSignals;
    }

    private void OnDisable()
    {
        LooxidLinkData.OnReceiveEEGRawSignals -= OnReceiveEEGRawSignals;
        LooxidLinkManager.Instance.Terminate();
    }

    // Raw
    // AF3, AF4, Fp1, Fp2, AF7, AF8 is the order received
    private void OnReceiveEEGRawSignals(EEGRawSignal rawSignalData)
    {
      
        csvRec.Write(rawSignalData);//excel save
        if(lslReady)
        {
            lslOut.SendDataEEG(rawSignalData);
        }       
        UpdateChart(rawSignalData);
    }

    private void UpdateChart(EEGRawSignal rawSignalData)
    {
        AF3Chart.SetValue(rawSignalData.FilteredRawSignal(EEGSensorID.AF3));
        AF4Chart.SetValue(rawSignalData.FilteredRawSignal(EEGSensorID.AF4));
        Fp1Chart.SetValue(rawSignalData.FilteredRawSignal(EEGSensorID.Fp1));
        Fp2Chart.SetValue(rawSignalData.FilteredRawSignal(EEGSensorID.Fp2));
        AF7Chart.SetValue(rawSignalData.FilteredRawSignal(EEGSensorID.AF7));
        AF8Chart.SetValue(rawSignalData.FilteredRawSignal(EEGSensorID.AF8));
    }


    private void OnLinkCoreConnected()
    {
        Debug.Log("Link Core is connected.");
    }
    private void OnLinkCoreDisconnected()
    {
        Debug.Log("Link Core is disconnected.");
    }
    private void OnLinkHubConnected()
    {
        Debug.Log("Link Hub is connected.");
    }
    private void OnLinkHubDisconnected()
    {
        Debug.Log("Link Hub is disconnected.");
    }

    /// <summary>
    /// Record event by id into seperate csv file, containing app time since record start and real world date time
    /// </summary>
    /// <param name="id"></param>
    public void WriteEvent(string id)
    {
        csvRec?.WriteEvent(id, PhysiologySignalsManager.Instance.AcquisitionTime);
    }

    private void OnApplicationQuit()
    {
        csvRec.CloseFile();
    }

    public void StartLSL()
    {
        lslOut.enabled = true;
        Debug.Log("" + lslOut.StreamName + "is now on");
    }
}
