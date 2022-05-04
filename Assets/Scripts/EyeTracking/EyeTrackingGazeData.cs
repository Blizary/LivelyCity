using System.Collections.Generic;
using UnityEngine;
using Assets.LSL4Unity.Scripts;
using System.IO;
using System.Text;


public class EyeTrackingGazeData : MonoBehaviour
{
    [SerializeField] private EyeTrackingManager eyeTrackManager = default;
    [SerializeField] [Range(0f, 1000f)] private float maxRayDistance = default;
    [SerializeField] private LayerMask pointsOfInterestMask = default;
    [SerializeField] private LSLMarkerStream lslout;

    // POI = Point of Interest
    private Transform gazedPOI;
    private RaycastHit rayHit;

    private bool recInitialized;
    private EyeTrackingCSVRecorder csvRec;

    // Dictionary that holds tuples with the points of interest names, the number of looks and total stare time
    // The key is the instance ID of each object, that way we can guarantee uniqueness even when
    // different objects have the same name
    public Dictionary<int, (string poiName, int numOfLooks, float totalStareTime)> GazeData { get; private set; }
        = new Dictionary<int, (string, int, float)>();

    // Total time spent looking at POI
    public float TotalPOIGaze { get; private set; }

    private void Awake()
    {
        csvRec = new EyeTrackingCSVRecorder();
    }

    public void StartRecording(string fileName = null)
    {
        Debug.Log("called eye track with file: " + fileName);
        if (fileName != null)
        {
            recInitialized = true;
            //csvRec.CreateFile();
        }
    }

    public void StopRecording()
    {
        recInitialized = false;
        //csvRec.Write(GazeData);
        //csvRec.CloseFile();
    }

    private void Update()
    {
        if (recInitialized)
        {
            GetGazeData();
        }
    }

    private void GetGazeData()
    {
        if (Physics.Raycast(Camera.main.transform.position,
            eyeTrackManager.GazeDirectionWorld,
            out rayHit, maxRayDistance, pointsOfInterestMask))
        {
            TotalPOIGaze += Time.deltaTime;

            lslout.Write(rayHit.transform.name);
            Debug.Log("Send info to lsl name: " + rayHit.transform.name);

            if (rayHit.transform != gazedPOI)
            {
                int objID;

                gazedPOI = rayHit.transform;
                objID = gazedPOI.GetInstanceID();

                //lslout.Write(gazedPOI.name);
                //Debug.Log("Send info to lsl name: " + gazedPOI.name);

                if (!GazeData.ContainsKey(objID))
                {
                    GazeData.Add(objID, (gazedPOI.name, 1, Time.deltaTime));
                    
                }
                else if (gazedPOI != null)
                {
                    GazeData[objID] =
                        (GazeData[objID].poiName,
                        GazeData[objID].numOfLooks + 1,
                        GazeData[objID].totalStareTime + Time.deltaTime);
                }
            }
            else
            {
                int objID = gazedPOI.GetInstanceID();

                GazeData[objID] =
                    (GazeData[objID].poiName,
                    GazeData[objID].numOfLooks,
                    GazeData[objID].totalStareTime + Time.deltaTime);
            }
        }
        else
        {
            gazedPOI = null;
        }
    }
}
