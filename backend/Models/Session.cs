namespace backend.Models;

public class Session
{
    public int Id { get; set; }
    public SessionSlot Slot { get; set; }
    public string TimeRange { get; set; } = "";
    public int Capacity { get; set; }

    public List<Participant> Participants { get; set; } = new();
}