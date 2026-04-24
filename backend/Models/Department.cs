namespace backend.Models;

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int MaxPerSession { get; set; }

    // Navigation properties - let EF load related data
    public List<Participant> Participants { get; set; } = new();
    public List<Coordinator> Coordinators { get; set; } = new();
}