using Services.Implementations;
using Xunit;

namespace MrTest.Services.Tests
{
    public class CircuitBreakerServiceTests
    {
        [Fact]
        public void IsOpen_ShouldBeFalse_OnInitialization()
        {
            var service = new CircuitBreakerService();
            Assert.False(service.IsOpen);
            Assert.Equal(0, service.ConsecutiveFailures);
        }

        [Fact]
        public void RegisterFailure_ShouldIncrementFailures_AndOpenCircuit_WhenThresholdReached()
        {
            var service = new CircuitBreakerService(3);
            Assert.False(service.IsOpen);

            service.RegisterFailure();
            Assert.Equal(1, service.ConsecutiveFailures);
            Assert.False(service.IsOpen);

            service.RegisterFailure();
            Assert.Equal(2, service.ConsecutiveFailures);
            Assert.False(service.IsOpen);

            service.RegisterFailure();
            Assert.Equal(3, service.ConsecutiveFailures);
            Assert.True(service.IsOpen);
        }

        [Fact]
        public void Reset_ShouldCloseCircuit_AndResetFailures()
        {
            var service = new CircuitBreakerService(2);
            service.RegisterFailure();
            service.RegisterFailure();
            Assert.True(service.IsOpen);

            service.Reset();
            Assert.False(service.IsOpen);
            Assert.Equal(0, service.ConsecutiveFailures);
        }

        [Fact]
        public void GetStatus_ShouldReturnCurrentState()
        {
            var service = new CircuitBreakerService(2);
            service.RegisterFailure();
            var status = service.GetStatus();

            Assert.False(status.IsOpen);
            Assert.Equal(1, status.ConsecutiveFailures);
            Assert.Equal(2, status.FailureThreshold);

            service.RegisterFailure();
            status = service.GetStatus();
            Assert.True(status.IsOpen);
            Assert.Equal(2, status.ConsecutiveFailures);
        }
    }
}