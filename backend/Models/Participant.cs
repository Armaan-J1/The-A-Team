namespace backend.Models;

public class Participant
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public int DepartmentId { get; set; }
    public Department Department { get; set; } = null!;

    public int? SessionId { get; set; }        // nullable - null means unassigned
    public Session? Session { get; set; }

    // Computed, not stored. Convenience for reading.
    public bool Assigned => SessionId != null;
}