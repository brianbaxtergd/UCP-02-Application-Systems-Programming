using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class LevelAdvancePanel : MonoBehaviour
{
    static private LevelAdvancePanel S;

    public enum eLevelAdvanceState
    {
        none, idle, fadeIn, fadeIn2, fadeIn3, display, fadeOut, fadeOut2, fadeOut3
    }

    [Header("Set in Inspector")]
    public float fadeTime = 1f;
    public float displayTime = 1f;

    [Header("Set dynamically")]
    [SerializeField]
    private eLevelAdvanceState state = eLevelAdvanceState.none;

    Image imgBG, imgFrame;
    Text levelText, infoText;
    RectTransform levelRT;
    float stateStartTime, stateDuration;
    eLevelAdvanceState nextState;
    AsteraX.CallbackDelegate idleCallback;
    AsteraX.CallbackDelegate displayCallback;

    private void Awake()
    {
        S = this;

        imgBG = GetComponent<Image>();

        // Find the levelText child.
        Transform levelT = transform.Find("LevelText");
        if (levelT == null)
        {
            Debug.LogWarning("LevelAdvancePanel:Awake() - LevelAdvancePanel lacks a child named LevelText.");
            return;
        }
        levelRT = levelT.GetComponent<RectTransform>();
        levelText = levelT.GetComponent<Text>();
        if (levelText == null)
        {
            Debug.LogWarning("LevelAdvancePanel:Awake() - LevelAdvancePanel child LevelText lacks a text component.");
            return;
        }

        // Find the infoText child.
        Transform infoT = transform.Find("InfoText");
        if (infoT == null)
        {
            Debug.LogWarning("LevelAdvancePanel:Awake() - LevelAdvancePanel lack a child named InfoText.");
            return;
        }
        infoText = infoT.GetComponent<Text>();
        if (infoText == null)
        {
            Debug.LogWarning("LevelAdvancePanel:Awake() - LevelAdvancePanel child InfoText lacks a Text component.");
            return;
        }

        // Find the InfoTextFrame child.
        Transform infoFrame = transform.Find("InfoTextFrame");
        if (infoFrame == null)
        {
            Debug.LogWarning("LevelAdvancePanel:Awake() - LevelAdvancePanel lacks a child named InfoTextFrame.");
            return;
        }
        imgFrame = infoFrame.GetComponent<Image>();
        if (imgFrame == null)
        {
            Debug.LogWarning("LevelAdvancePanel:Awake() - LevelAdvancePanel child InfoTextFrame lacks an Image component.");
            return;
        }

        SetState(eLevelAdvanceState.idle);
    }

    private void Update()
    {
        if (state == eLevelAdvanceState.none)
            return;

        float u = (realTime - stateStartTime) / stateDuration;
        bool moveNext = false;
        if (u >= 1)
        {
            u = 1;
            moveNext = true;
        }
        float u1 = 1 - u;
        float n;
        Color col;
        switch (state)
        {
            case eLevelAdvanceState.fadeIn: // Background.
                imgBG.color = new Color(0, 0, 0, u);
                break;

            case eLevelAdvanceState.fadeIn2: // LevelText.
                n = levelTextYScaleEffect(u);
                levelRT.localScale = new Vector3(1, n, 1);
                break;

            case eLevelAdvanceState.fadeIn3: // InfoText.
                n = u * u;
                col = new Color(1, 1, 1, n);
                infoText.color = col;
                //imgFrame.color = col;
                break;

            case eLevelAdvanceState.display:
                // Trigger the game to advance to the next level.
                break;

            case eLevelAdvanceState.fadeOut: // InfoText.
                n = u1 * u1;
                col = new Color(1, 1, 1, n);
                infoText.color = col;
                //imgFrame.color = col;
                break;

            case eLevelAdvanceState.fadeOut2: // LevelText.
                n = levelTextYScaleEffect(u1);
                levelRT.localScale = new Vector3(1, n, 1);
                break;

            case eLevelAdvanceState.fadeOut3: // Background.
                n = u1 * u1;
                imgBG.color = new Color(0, 0, 0, n);
                break;

            default:
                break;
        }

        if (moveNext)
            SetState(nextState);
    }

    void SetState(eLevelAdvanceState newState)
    {
        stateStartTime = realTime;

        switch (newState)
        {
            case eLevelAdvanceState.idle:
                gameObject.SetActive(false);
                if (idleCallback != null)
                {
                    idleCallback();
                    idleCallback = null;
                }
                nextState = eLevelAdvanceState.fadeIn;
                break;

            case eLevelAdvanceState.fadeIn:
                gameObject.SetActive(true);
                // Set text.
                levelText.text = "Level " + AsteraX.GAME_LEVEL;
                levelRT.localScale = new Vector3(1, 0, 1);
                AsteraX.LevelInfo lvlInfo = AsteraX.GetLevelInfo();
                infoText.text = "Asteroids: " + lvlInfo.numInitialAsteroids + "\tChildren: " + lvlInfo.numSubAsteroids;
                infoText.color = Color.clear;
                // Set initial state.
                imgBG.color = Color.clear;
                levelRT.localScale = new Vector3(1, 0, 1);
                infoText.color = Color.clear;
                // Set timing and advancement.
                stateDuration = fadeTime * 0.2f;
                nextState = eLevelAdvanceState.fadeIn2;
                break;

            case eLevelAdvanceState.fadeIn2:
                // Set initial state.
                imgBG.color = Color.black;
                levelRT.localScale = new Vector3(1, 0, 1);
                infoText.color = Color.clear;
                // Set timing and advancement.
                stateDuration = fadeTime * 0.6f;
                nextState = eLevelAdvanceState.fadeIn3;
                break;

            case eLevelAdvanceState.fadeIn3:
                // Set initial state.
                imgBG.color = Color.black;
                levelRT.localScale = new Vector3(1, 1, 1);
                infoText.color = Color.clear;
                // Set timing and advancement.
                stateDuration = fadeTime * 0.2f;
                nextState = eLevelAdvanceState.display;
                break;

            case eLevelAdvanceState.display:
                stateDuration = displayTime;
                nextState = eLevelAdvanceState.fadeOut;
                if (displayCallback != null)
                {
                    displayCallback();
                    displayCallback = null;
                }
                break;

            case eLevelAdvanceState.fadeOut:
                // Set initial state.
                imgBG.color = Color.black;
                levelRT.localScale = new Vector3(1, 1, 1);
                infoText.color = Color.white;
                // Set timing and advancement.
                stateDuration = fadeTime * 0.2f;
                nextState = eLevelAdvanceState.fadeOut2;
                break;

            case eLevelAdvanceState.fadeOut2:
                // Set initial state.
                imgBG.color = Color.black;
                levelRT.localScale = new Vector3(1, 1, 1);
                infoText.color = Color.clear;
                // Set timing and advancement.
                stateDuration = fadeTime * 0.6f;
                nextState = eLevelAdvanceState.fadeOut3;
                break;

            case eLevelAdvanceState.fadeOut3:
                // Set initial state.
                imgBG.color = Color.black;
                levelRT.localScale = new Vector3(1, 0, 1);
                infoText.color = Color.clear;
                // Set timing and advancement.
                stateDuration = fadeTime * 0.2f;
                nextState = eLevelAdvanceState.idle;
                break;
        }

        state = newState;
    }

    float levelTextYScaleEffect(float u)
    {
        return u + Mathf.Sin(Mathf.PI * u);
    }

    float realTime
    {
        get
        {
            return Time.realtimeSinceStartup;
        }
    }

    static public void AdvanceLevel(AsteraX.CallbackDelegate displayCB, AsteraX.CallbackDelegate idleCB)
    {
        if (S == null)
        {
            Debug.LogError("LevelAdvancePanel:AdvanceLevel() - Called while S is null.");
            return;
        }

        S.displayCallback = displayCB;
        S.idleCallback = idleCB;
        
        S.SetState(eLevelAdvanceState.fadeIn);
    }
}
