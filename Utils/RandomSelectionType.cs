using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

[Serializable]
public class RandomSelectionType
{
    [Serializable]
    public enum Type
    {
        Random,
        Sequential
    }
    
    public int DontRepeatFor = 0; // 0 = totally random, 1 = don''t repeat immediately, 2 = wait at least 2 before repeat if possible, etc
    
    List<int> lastPlayed = new List<int>();

    public Type SelectionType;
    
    public T Select<T>(List<T> list)
    {
        T selected;

        if (list.Count == 0)
        {
            selected = default(T);
            return selected;
        }

        if (SelectionType == Type.Sequential)
        {
            if (lastPlayed.Count == list.Count)
            {
                lastPlayed.Clear();
            }
            
            var index = lastPlayed.Any() ? lastPlayed.Last() + 1: 0;
            
            lastPlayed.Add(index);
            return list[index];
        }
        else
        {

            if (DontRepeatFor == 0)
            {
                selected = list[Random.Range(0, list.Count)];
                return selected;
            }

            if (lastPlayed.Count == list.Count)
            {
                lastPlayed.Clear();
            }

            if (DontRepeatFor > -1)
            {
                while (lastPlayed.Count > DontRepeatFor)
                {
                    lastPlayed.RemoveAt(0);
                }
            }

            int index = Random.Range(0, list.Count);
            while (lastPlayed.Contains(index))
            {
                index = Random.Range(0, list.Count);
            }

            lastPlayed.Add(index);
            selected = list[index];
            return selected;
        }
    }

}