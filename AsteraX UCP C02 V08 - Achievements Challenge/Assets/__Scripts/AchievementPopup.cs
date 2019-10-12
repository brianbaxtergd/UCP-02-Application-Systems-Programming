using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementPopup : MonoBehaviour
{
    static private AchievementPopup _S;

    [Header("Set in Inspector")]
    public Text popUpText, popUpDescription;
    public Vector3 startPosition, offscreenPosition;
    public float offscreenAdj;
    public float moveSpeed = 10f;
    public float movePauseTime = 2f;

    [Header("Set Dynamically")]
    public bool bIsAlreadyPopping = false;
    public List<StringTuple> achievementList = new List<StringTuple>();

    void Start()
    {
        S = this;

        startPosition = transform.position;
        offscreenPosition = startPosition;
        offscreenPosition.y += offscreenAdj;
        transform.position = offscreenPosition;
    }

    public void PopUp(StringTuple st)
    {
        PopUp(st.a, st.b);
    }
    public void PopUp(string achievementName, string achievementDescription = "")
    {
        if (!bIsAlreadyPopping)
        {
            bIsAlreadyPopping = true;

            popUpText.text = achievementName;
            popUpDescription.text = achievementDescription;
            transform.position = offscreenPosition;

            StartCoroutine(MoveToPosition());
        }
        else
        {
            StringTuple st = new StringTuple(achievementName, achievementDescription);
            achievementList.Add(st);
            StartCoroutine(WaitYourTurn());
        }
    }

    IEnumerator WaitYourTurn()
    {
        while (bIsAlreadyPopping)
        {
            yield return new WaitForSeconds(0.5f);
        }
        if (achievementList.Count > 0)
        {
            StringTuple st = achievementList[0];
            achievementList.RemoveAt(0);

            PopUp(st);
        }
    }

    IEnumerator MoveToPosition()
    {
        float step = (moveSpeed / (offscreenPosition - startPosition).magnitude * Time.fixedDeltaTime);
        float t = 0;
        float u;
        while(t <= 1.0f)
        {
            t += step;
            u = 1 - (1 - t) * (1 - t); // This does some fancy easing on the Lerp.
            transform.position = Vector3.LerpUnclamped(offscreenPosition, startPosition, u);
            yield return new WaitForFixedUpdate();
        }
        transform.position = startPosition;

        yield return new WaitForSeconds(movePauseTime);

        t = 0;
        while(t < 1.0f)
        {
            t += step;
            u = t * t; // This does some fancy easing on the Lerp.
            transform.position = Vector3.Lerp(startPosition, offscreenPosition, u);
            yield return new WaitForFixedUpdate();
        }
        transform.position = offscreenPosition;

        bIsAlreadyPopping = false;
    }

    static public AchievementPopup S
    {
        get
        {
            return _S;
        }
        set
        {
            if (_S != null)
                Debug.LogError("AchievementPopup:S setter - Attempt to set S when it has already been set.");
            _S = value;
        }
    }
}

[System.Serializable]
public struct StringTuple
{
    public string a, b;

    public StringTuple(string sA = "", string sB = "")
    {
        a = sA;
        b = sB;
    }
}
