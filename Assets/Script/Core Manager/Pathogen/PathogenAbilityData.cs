using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class PathogenAbilityData
{
    public PathogenAbilityType abilityType;
    public int turnInterval = 1; // How often this ability triggers
    public int value = 0; // Damage, healing, or other numeric value
    public List<CardSO> targetCards = new List<CardSO>(); // Direct CardSO references for blocking
    
    // Overload constructors for flexibility
    public PathogenAbilityData(PathogenAbilityType type, int interval, int abilityValue)
    {
        abilityType = type;
        turnInterval = interval;
        value = abilityValue;
    }
    
    public PathogenAbilityData(PathogenAbilityType type, int interval, int abilityValue, List<CardSO> cards)
    {
        abilityType = type;
        turnInterval = interval;
        value = abilityValue;
        targetCards = cards ?? new List<CardSO>();
    }
}

[System.Serializable]
public class PathogenAbilityDictionary : SerializableDictionary<PathogenAbilityType, PathogenAbilityData> {}



[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> keys = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        
        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();

        if (keys.Count != values.Count)
        {
            Debug.LogError("Keys and values count mismatch in SerializableDictionary!");
            return;
        }

        for (int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }
    }
}