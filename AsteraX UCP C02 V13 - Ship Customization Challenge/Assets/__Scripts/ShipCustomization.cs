using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCustomization : MonoBehaviour
{
    static private ShipCustomization _S; // Protected Singleton. See S property below.

    Dictionary<ShipPart.eShipPartType, Transform> currPartsDict;

    private void Start()
    {
        S = this;

        // Find replaceable parts on this ship or its children.
        currPartsDict = new Dictionary<ShipPart.eShipPartType, Transform>();
        ShipPart[] parts = GetComponentsInChildren<ShipPart>();
        foreach (ShipPart sp in parts)
        {
            currPartsDict.Add(sp.type, sp.transform);
        }
    }

    static public bool SetPart(ShipPart.eShipPartType type, int num)
    {
        return S.SetPartNS(type, num);
    }
    
    bool SetPartNS(ShipPart.eShipPartType type, int num)
    {
        // Ensure that this is asking for an extant ship part.
        if (!ShipPartsDictionary.DICT.ContainsKey(type))
        {
            Debug.LogError("ShipCustomization:SetPartNS - ShipPartsDictionary.DICT does not contain type: " + type);
            return false;
        }
        if (!currPartsDict.ContainsKey(type))
        {
            Debug.LogError("ShipCustomization:SetPartNS - currPartDict does not contain type: " + type);
            return false;
        }
        if (num < 0 || ShipPartsDictionary.DICT[type].partInfos.Length <= num)
        {
            Debug.LogError("ShipCustomization:SetPartNS - Attempt to choose nonextant " + type + ": " + num);
            return false;
        }

        // Now we know this is a valid type and part num.
        // Pull the information from the current part in that place.
        Transform currTrans = currPartsDict[type];
        Vector3 lPos = currTrans.localPosition;
        Quaternion lRot = currTrans.localRotation;
        Transform parentTrans = currTrans.parent;

        // Generate a new part and position it correctly.
        Transform newTrans = Instantiate<GameObject>(ShipPartsDictionary.DICT[type].partInfos[num].prefab).transform;
        newTrans.SetParent(parentTrans);
        newTrans.localPosition = lPos;
        newTrans.localRotation = lRot;

        // Replace the currTrans with the newTrans in the currPartsDict.
        currPartsDict[type] = newTrans;

        // Destroy the old one.
        Destroy(currTrans.gameObject);

        // Save the game.
        SaveGameManager.Save();

        return true;
    }

    static private ShipCustomization S
    {
        get
        {
            if (_S == null)
            {
                Debug.LogError("ShipCustomization:S getter - Attempt to get value of S before it has been set.");
                return null;
            }
            return _S;
        }
        set
        {
            if (_S != null)
            {
                Debug.LogError("ShipCustomization:S setter - Attempt to set value of S after it has already been set.");
            }
            _S = value;
        }
    }
}
