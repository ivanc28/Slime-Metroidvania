[System.Serializable]
public class Dialogue
{
    public enum Speaker { NPC, Player }
    public Speaker speaker;
    public string[] strList;
}
