using System.Collections.Generic;

[System.Serializable]
public class SerializableList<T>
{
    public List<T> items;

    public SerializableList(IEnumerable<T> items)
    {
        this.items = new List<T>(items);
    }
}