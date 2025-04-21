using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] List<TutorialZone> zones = new List<TutorialZone>();

    int zoneIndex = 0;
    TutorialZone currentZone;

    // Start is called before the first frame update
    void Start()
    {
        currentZone = zones[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnterZone(TutorialZone zone)
    {
        if (zones.Contains(zone))
        {
            // lower cam priority of old zone
            currentZone.GetCam().Priority = 0;

            zoneIndex = zones.IndexOf(zone);
            currentZone = zone;

            // change to new cam
            currentZone.GetCam().Priority = 100;

            // start dialogue
            Fungus.Flowchart.BroadcastFungusMessage(currentZone.GetSignal());
        }
    }
}
