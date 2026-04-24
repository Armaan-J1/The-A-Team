using backend.Models;

namespace backend.Data;

public static class SeedData
{
    public static void Run(AppDbContext db)
    {
        // Only seed if the DB is empty
        if (db.Departments.Any()) return;

        // ─── Departments ─────────────────────────────────────
        var divA = new Department { Name = "Division A", MaxPerSession = 8 };
        var divB = new Department { Name = "Division B", MaxPerSession = 6 };
        var divC = new Department { Name = "Division C", MaxPerSession = 6 };
        db.Departments.AddRange(divA, divB, divC);
        db.SaveChanges();  // save so IDs are assigned

        // ─── Coordinators ────────────────────────────────────
        db.Coordinators.AddRange(
            new Coordinator { Name = "Sarah Johnson", Email = "sarah@co.com", DepartmentId = divA.Id },
            new Coordinator { Name = "John Smith",    Email = "john@co.com",  DepartmentId = divB.Id },
            new Coordinator { Name = "Mike Chen",     Email = "mike@co.com",  DepartmentId = divC.Id }
        );

        // ─── Sessions ────────────────────────────────────────
        db.Sessions.AddRange(
            new Session { Slot = SessionSlot.Morning,   TimeRange = "09:00 - 10:30", Capacity = 20 },
            new Session { Slot = SessionSlot.Midday,    TimeRange = "11:00 - 12:30", Capacity = 20 },
            new Session { Slot = SessionSlot.Afternoon, TimeRange = "13:00 - 14:30", Capacity = 20 }
        );

        // ─── Participants ────────────────────────────────────
        // 24 Division A, 18 Division B, 18 Division C → 60 total
        int n = 1;
        for (int i = 0; i < 24; i++)
            db.Participants.Add(new Participant { Name = $"Participant {n++:D2}", DepartmentId = divA.Id });
        for (int i = 0; i < 18; i++)
            db.Participants.Add(new Participant { Name = $"Participant {n++:D2}", DepartmentId = divB.Id });
        for (int i = 0; i < 18; i++)
            db.Participants.Add(new Participant { Name = $"Participant {n++:D2}", DepartmentId = divC.Id });

        db.SaveChanges();
        Console.WriteLine($"Seeded: 3 departments, 3 coordinators, 3 sessions, {n - 1} participants");
    }
}