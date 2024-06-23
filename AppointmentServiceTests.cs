using AutoMapper;
using DentalAppointment.DTOs;
using DentalAppointment.Models;
using DentalAppointment.Repositories.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DentalAppointment.Tests.Services
{
    public class AppointmentServiceTests
    {
        private readonly Mock<IAppointmentRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;

        public AppointmentServiceTests()
        {
            // Mocking the repository
            _mockRepository = new Mock<IAppointmentRepository>();

            // Mocking the mapper
            _mockMapper = new Mock<IMapper>();
        }

        [Fact]
        public async Task GetAllAppointmentsAsync_ShouldReturnAllAppointments()
        {
            // Arrange
            var appointments = new List<Appointment>
            {
                new Appointment { Id = 1, PatientName = "Patient 1", Dentist = "Dentist 1", Procedure = "Procedure 1" },
                new Appointment { Id = 2, PatientName = "Patient 2", Dentist = "Dentist 2", Procedure = "Procedure 2" }
            };
            _mockRepository.Setup(repo => repo.GetAllAppointmentsAsync()).ReturnsAsync(appointments);

            // Mocking IMapper
            _mockMapper.Setup(mapper => mapper.Map<AppointmentDTO>(It.IsAny<Appointment>()))
                       .Returns((Appointment source) =>
                       {
                           return new AppointmentDTO
                           {
                               Id = source.Id,
                               PatientN = source.PatientName,
                               Dentist = source.Dentist,
                               Procedure = source.Procedure
                           };
                       });

            var service = new AppointmentService(_mockRepository.Object, _mockMapper.Object);

            // Act
            var result = await service.GetAllAppointmentsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, ((List<AppointmentDTO>)result).Count);
        }

        [Fact]
        public async Task GetAppointmentByIdAsync_ShouldReturnAppointment()
        {
            // Arrange
            var appointment = new Appointment { Id = 1, PatientName = "Patient 1", Dentist = "Dentist 1", Procedure = "Procedure 1" };
            _mockRepository.Setup(repo => repo.GetAppointmentByIdAsync(appointment.Id)).ReturnsAsync(appointment);

            var service = new AppointmentService(_mockRepository.Object, _mockMapper.Object);

            // Act
            var result = await service.GetAppointmentByIdAsync(appointment.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(appointment.Id, result.Id);
        }

        [Fact]
        public async Task GetAppointmentsByPatientNameAsync_ShouldReturnAppointments()
        {
            // Arrange
            var patientName = "Patient 1";
            var appointments = new List<Appointment>
            {
                new Appointment { Id = 1, PatientName = patientName, Dentist = "Dentist 1", Procedure = "Procedure 1" },
                new Appointment { Id = 2, PatientName = patientName, Dentist = "Dentist 2", Procedure = "Procedure 2" }
            };
            _mockRepository.Setup(repo => repo.GetAppointmentsByPatientNameAsync(patientName)).ReturnsAsync(appointments);

            var service = new AppointmentService(_mockRepository.Object, _mockMapper.Object);

            // Act
            var result = await service.GetAppointmentsByPatientNameAsync(patientName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, ((List<Appointment>)result).Count);
            Assert.All(result, a => Assert.Equal(patientName, a.PatientName));
        }

        [Fact]
        public async Task GetAppointmentsByDentistAsync_ShouldReturnAppointments()
        {
            // Arrange
            var dentist = "Dentist 1";
            var appointments = new List<Appointment>
            {
                new Appointment { Id = 1, PatientName = "Patient 1", Dentist = dentist, Procedure = "Procedure 1" },
                new Appointment { Id = 2, PatientName = "Patient 2", Dentist = dentist, Procedure = "Procedure 2" }
            };
            _mockRepository.Setup(repo => repo.GetAppointmentsByDentistAsync(dentist)).ReturnsAsync(appointments);

            var service = new AppointmentService(_mockRepository.Object, _mockMapper.Object);

            // Act
            var result = await service.GetAppointmentsByDentistAsync(dentist);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, ((List<Appointment>)result).Count);
            Assert.All(result, a => Assert.Equal(dentist, a.Dentist));
        }

        [Fact]
        public async Task CreateAppointment_ShouldInvokeRepositoryMethod()
        {
            // Arrange
            var appointmentDTO = new AppointmentDTO
            {
                PatientN = "John Doe",
                Dentist = "Dr. Smith",
                Procedure = "Cleaning"
            };

            var service = new AppointmentService(_mockRepository.Object, _mockMapper.Object);

            // Act
            await service.CreateAppointment(appointmentDTO);

            // Assert
            _mockRepository.Verify(repo => repo.CreateAppointment(It.IsAny<AppointmentDTO>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAppointmentAsync_ShouldInvokeRepositoryMethod()
        {
            // Arrange
            var id = 1;
            var appointmentDTO = new AppointmentDTO
            {
                PatientN = "John Doe",
                Dentist = "Dr. Smith",
                Procedure = "Cleaning"
            };

            var service = new AppointmentService(_mockRepository.Object, _mockMapper.Object);

            // Act
            await service.UpdateAppointmentAsync(appointmentDTO, id);

            // Assert
            _mockRepository.Verify(repo => repo.UpdateAppointmentAsync(appointmentDTO, id), Times.Once);
        }

        [Fact]
        public async Task DeleteAppointmentAsync_ShouldInvokeRepositoryMethod()
        {
            // Arrange
            var id = 1;

            var service = new AppointmentService(_mockRepository.Object, _mockMapper.Object);

            // Act
            await service.DeleteAppointmentAsync(id);

            // Assert
            _mockRepository.Verify(repo => repo.DeleteAppointmentAsync(id), Times.Once);
        }
    }
}
