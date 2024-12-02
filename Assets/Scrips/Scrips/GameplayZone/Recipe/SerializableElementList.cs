using System.Collections.Generic;

[System.Serializable]
public class SerializableElementList
{
    public List<string> elements;

    public SerializableElementList(HashSet<string> elementSet)
    {
        elements = new List<string>(elementSet);
    }
}
