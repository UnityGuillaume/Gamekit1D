using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class Map : MonoBehaviour
{
    protected static Map _instance;
    public static Map Instance
    {
        get { return _instance; }
    }

    [System.Serializable]
    public class MapEntry
    {
        public Behaviour1D content;
        public int position;
    }

    protected Dictionary<int, List<MapEntry>> map = new Dictionary<int, List<MapEntry>>();

    private void Awake()
    {
        _instance = this;
    }

    public MapEntry Register(int position, Behaviour1D content)
    {
        if (!map.ContainsKey(position))
        {
            map.Add(position, new List<MapEntry>());
        }

        MapEntry entry = new MapEntry();
        entry.content = content;
        entry.position = position;

        map[position].Add(entry);

        return entry;
    }

    public void Unregister(MapEntry entry)
    {
        map[entry.position].Remove(entry);
    }

    public List<MapEntry> GetEntries(int position)
    {
        if (!map.ContainsKey(position))
            return null;

        return map[position];
    }

    public void MoveEntry(MapEntry entry, int position)
    {
        map[entry.position].Remove(entry);

        if (!map.ContainsKey(position))
        {
            map.Add(position, new List<MapEntry>());
        }

        map[position].Add(entry);
        entry.position = position;
    }
}
