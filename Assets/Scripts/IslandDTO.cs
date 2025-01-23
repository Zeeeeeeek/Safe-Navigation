public class IslandDTO
{
    public string islandId;
    public Answer answer;
    public IslandDTO(string islandId, Answer answer)
    {
        this.islandId = islandId;
        this.answer = answer;
    }
}