using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    static private AchievementManager _S; // A private singleton.

    [Header("Set in Inspector")]
    public AchievementPopup popUp;
    public StepRecord[] stepRecords;
    public Achievement[] achievements;

    static private Dictionary<Achievement.eStepType, StepRecord> STEP_REC_DICT;

    private void Awake()
    {
        S = this;

        STEP_REC_DICT = new Dictionary<Achievement.eStepType, StepRecord>();
        foreach (StepRecord sRec in stepRecords)
        {
            STEP_REC_DICT.Add(sRec.type, sRec);
        }
    }

    void TriggerPopUp(string achievementName, string achievementDescription = "")
    {
        popUp.PopUp(achievementName, achievementDescription);
    }

    // Statics.

    static public AchievementManager S
    {
        get
        {
            return _S;
        }
        set
        {
            _S = value;
        }
    }

    static public void AchievementStep(Achievement.eStepType stepType, int num = 1)
    {
        StepRecord sRec = STEP_REC_DICT[stepType];
        if (sRec != null)
        {
            sRec.Progress(num);
            // Iterate through all possible Achievements and see if the step completes the Achievement.
            foreach (Achievement ach in S.achievements)
            {
                if (!ach.complete)
                {
                    // Pass the step information to the Achievement, to see if it is completed.
                    if (ach.CheckCompletion(stepType, sRec.num))
                        // The result is true if the Achievement was completed.
                        AnnounceAchievementCompletion(ach);
                }
            }
        }
        else
        {
            Debug.LogWarning("AchievementManager:AchievmentStep( " + stepType + " , " + num + " )" + " was passed a stepType that is not in STEP_REC_DICT.");
        }
    }

    static public void AnnounceAchievementCompletion(Achievement ach)
    {
        string desc = ach.description.Replace("#", ach.stepCount.ToString("N0"));
        S.TriggerPopUp(ach.name, desc);
    }

    static public StepRecord[] GetStepRecords()
    {
        return S.stepRecords;
    }

    static public Achievement[] GetAchievements()
    {
        return S.achievements;
    }
}

/// <summary>
/// <para>The Achievement class allows definition of Achievements for the player to complete.</para>
/// </summary>
[System.Serializable]
public class Achievement
{
    public enum eStepType
    {
        levelUp, bulletFired, hitAsteroid, luckyShot, scoreAttained,
    }

    public string name;
    [Tooltip("A # in the description will be replaced by the stepCount value.")]
    public string description;
    public eStepType stepType;
    public int stepCount;
    public Image unlockablePartImage;
    [SerializeField]
    private bool _complete = false;

    // This property with an internal set clause protects _complete.
    public bool complete
    {
        get { return _complete; }
        internal set { _complete = value; }
    }

    public bool CheckCompletion(eStepType type, int num)
    {
        if (type != stepType || complete)
            return false;

        // Did this complete the Achievement?
        if (num >= stepCount)
        {
            // Unlock Achievement.
            complete = true;
            return true;
        }
        return false;
    }
}

[System.Serializable]
public class StepRecord
{
    public Achievement.eStepType type;
    public bool cumulative = false;
    [SerializeField]
    private int _num = 0;

    public void Progress(int n)
    {
        if (cumulative)
            _num += n;
        else
            _num = n;
    }

    public int num
    {
        get { return _num; }
        internal set { _num = value; }
    }
}


