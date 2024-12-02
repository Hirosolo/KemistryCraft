[System.Serializable]
public class Recipe
{
    public string element1;
    public string element2;
    public string result;

    public Recipe(string element1, string element2, string result)
    {
        this.element1 = element1;
        this.element2 = element2;
        this.result = result;
    }

    public bool Matches(string e1, string e2)
    {
        return (element1 == e1 && element2 == e2) || (element1 == e2 && element2 == e1);
    }
}
