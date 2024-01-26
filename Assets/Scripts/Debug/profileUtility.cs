using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class profileUtility : MonoBehaviour 
{
    public float AverageTimePerOperation = 0;
    public float MaximumTimePerOperation = 0;
    public float OpsRecorded = 0;
    public float mostRecentOp = 0 ;

    private long startTime = 0;
    private long endTime = 0;
    public void startTimer()
    {
        if (startTime == 0)
        {
            startTime = DateTime.UtcNow.Ticks;
            OpsRecorded++;
        }
    }
    public void stopTimer()
    {
        if (endTime == 0)
        {
            endTime = DateTime.UtcNow.Ticks;
            mostRecentOp = (endTime - startTime) ;
            if (mostRecentOp > MaximumTimePerOperation) MaximumTimePerOperation = mostRecentOp;
            AverageTimePerOperation = (AverageTimePerOperation * (OpsRecorded - 1) + mostRecentOp) / OpsRecorded;
            startTime = 0;
            endTime = 0;
        }
    }
}
