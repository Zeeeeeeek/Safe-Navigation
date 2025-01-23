public class IslandDTO
{
    public string islandId;
    public Answer answer;
    public JsonResourcesReader.Content content;
    public IslandDTO(string islandId, Answer answer, JsonResourcesReader.Content content)
    {
        this.islandId = islandId;
        this.answer = answer;
        this.content = content;
    }
}