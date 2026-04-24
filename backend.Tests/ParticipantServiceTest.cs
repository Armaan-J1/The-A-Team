using backend.Data;
using backend.Models;
using backend.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace backend.Tests
{
    public class ParticipantServiceTests
    {
        // Helper method to generate a fresh, empty in-memory database for every test
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test
                .Options;
            
            return new AppDbContext(options);
        }

        [Fact] // This attribute tells xUnit that this is a test method
        public async Task AssignParticipant_ShouldFail_WhenAlreadyAssigned()
        {
            // ------------------
            // 1. ARRANGE
            // ------------------
            using var context = GetInMemoryDbContext();
            
            // Seed our fake database
            var department = new Department { Id = 1, Name = "Division A", MaxPerSession = 8 };
            var coordinator = new Coordinator { Id = 1, Name = "Admin", DepartmentId = 1 };
            var session = new Session { Id = 1, Capacity = 20, Slot = SessionSlot.Morning };
            
            // Create a participant who is ALREADY assigned to Session 1
            var participant = new Participant { Id = 5, Name = "John", DepartmentId = 1, SessionId = 1 };

            context.Departments.Add(department);
            context.Coordinators.Add(coordinator);
            context.Sessions.Add(session);
            context.Participants.Add(participant);
            await context.SaveChangesAsync();

            // Instantiate the service with the in-memory context
            var service = new ParticipantService(context);

            // ------------------
            // 2. ACT
            // ------------------
            // Try to assign John (Id 5) to a session again
            var result = await service.AssignParticipantAsync(
                coordinatorId: 1, 
                participantId: 5, 
                sessionId: 1
            );

            // ------------------
            // 3. ASSERT
            // ------------------
            Assert.False(result.Success);
            Assert.Equal("already_assigned", result.Reason);
        }
    }
}