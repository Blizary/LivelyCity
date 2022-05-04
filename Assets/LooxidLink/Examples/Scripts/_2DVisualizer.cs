using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Looxid.Link;

public class _2DVisualizer : MonoBehaviour
{
    [Header("Raw Signal")]
    public LineChart AF3Chart;
    public LineChart AF4Chart;
    public LineChart Fp1Chart;
    public LineChart Fp2Chart;
    public LineChart AF7Chart;
    public LineChart AF8Chart;

    void Start()
    {
        LooxidLinkManager.Instance.SetDebug(true);
        LooxidLinkManager.Instance.Initialize();
    }

    void OnEnable()
    {
        LooxidLinkData.OnReceiveEEGRawSignals += OnReceiveEEGRawSignals;
    }
    void OnDisable()
    {
        LooxidLinkData.OnReceiveEEGRawSignals -= OnReceiveEEGRawSignals;
    }
    void OnReceiveEEGRawSignals(EEGRawSignal rawSignalData)
    {
        Debug.Log("Got here");
        AF3Chart.SetValue(rawSignalData.FilteredRawSignal(EEGSensorID.AF3));
        AF4Chart.SetValue(rawSignalData.FilteredRawSignal(EEGSensorID.AF4));
        Fp1Chart.SetValue(rawSignalData.FilteredRawSignal(EEGSensorID.Fp1));
        Fp2Chart.SetValue(rawSignalData.FilteredRawSignal(EEGSensorID.Fp2));
        AF7Chart.SetValue(rawSignalData.FilteredRawSignal(EEGSensorID.AF7));
        AF8Chart.SetValue(rawSignalData.FilteredRawSignal(EEGSensorID.AF8));
    }
}