using DentalAppointment.Data;
using DentalAppointment.DTOs;
using DentalAppointment.Models;
using DentalAppointment.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DentalAppointment.Tests.Repositories
{
    public class AppointmentRepositoryTests
    {
        private readonly ApplicationDbContext _context;
        private readonly AppointmentRepository _repository;

        public AppointmentRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "DentalAppointmentTestDb")
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new AppointmentRepository(_context);
        }

        [Fact]
        public async Task CreateAppointment_ShouldAddAppointment()
        {
            // Arrange
            var appointmentDto = new AppointmentDTO
            {
                Date = DateTime.Now,
                PatientN = "John Doe",
                Dentist = "Dr. Smith",
                Procedure = "Cleaning"
            };

            // Act
            await _repository.CreateAppointment(appointmentDto);

            // Assert
            var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.PatientName == "John Doe");
            Assert.NotNull(appointment);
            Assert.Equal("John Doe", appointment.PatientName);
            Assert.Equal("Dr. Smith", appointment.Dentist);
            Assert.Equal("Cleaning", appointment.Procedure);
        }

        [Fact]
        public async Task DeleteAppointmentAsync_ShouldRemoveAppointment()
        {
            // Arrange
            var appointment = new Appointment
            {
                Date = DateTime.Now,
                PatientName = "John Doe",
                Dentist = "Dr. Smith",
                Procedure = "Cleaning"
            };
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteAppointmentAsync(appointment.Id);

            // Assert
            var deletedAppointment = await _context.Appointments.FindAsync(appointment.Id);
            Assert.Null(deletedAppointment);
        }

        [Fact]
        public async Task GetAllAppointmentsAsync_ShouldReturnAllAppointments()
        {
            // Arrange
            var appointment1 = new Appointment { Date = DateTime.Now, PatientName = "Patient 1", Dentist = "Dentist 1", Procedure = "Procedure 1" };
            var appointment2 = new Appointment { Date = DateTime.Now, PatientName = "Patient 2", Dentist = "Dentist 2", Procedure = "Procedure 2" };
            _context.Appointments.AddRange(appointment1, appointment2);
            await _context.SaveChangesAsync();

            // Act
            var appointments = await _repository.GetAllAppointmentsAsync();

            // Assert
            Assert.Equal(2, appointments.Count());
        }

        [Fact]
        public async Task GetAppointmentByIdAsync_ShouldReturnAppointment()
        {
            // Arrange
            var appointment = new Appointment
            {
                Date = DateTime.Now,
                PatientName = "John Doe",
                Dentist = "Dr. Smith",
                Procedure = "Cleaning"
            };
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // Act
            var retrievedAppointment = await _repository.GetAppointmentByIdAsync(appointment.Id);

            // Assert
            Assert.NotNull(retrievedAppointment);
            Assert.Equal(appointment.Id, retrievedAppointment.Id);
        }

        [Fact]
        public async Task GetAppointmentsByDentistAsync_ShouldReturnAppointments()
        {
            // Arrange
            var appointment1 = new Appointment { Date = DateTime.Now, PatientName = "Patient 1", Dentist = "Dentist 1", Procedure = "Procedure 1" };
            var appointment2 = new Appointment { Date = DateTime.Now, PatientName = "Patient 2", Dentist = "Dentist 1", Procedure = "Procedure 2" };
            _context.Appointments.AddRange(appointment1, appointment2);
            await _context.SaveChangesAsync();

            // Act
            var appointments = await _repository.GetAppointmentsByDentistAsync("Dentist 1");

            // Assert
            Assert.Equal(3, appointments.Count());
        }

        [Fact]
        public async Task GetAppointmentsByPatientNameAsync_ShouldReturnAppointments()
        {
            // Arrange
            var appointment1 = new Appointment { Date = DateTime.Now, PatientName = "Patient 1", Dentist = "Dentist 1", Procedure = "Procedure 1" };
            var appointment2 = new Appointment { Date = DateTime.Now, PatientName = "Patient 1", Dentist = "Dentist 2", Procedure = "Procedure 2" };
            _context.Appointments.AddRange(appointment1, appointment2);
            await _context.SaveChangesAsync();

            // Act
            var appointments = await _repository.GetAppointmentsByPatientNameAsync("Patient 1");

            // Assert
            Assert.Equal(4, appointments.Count());
        }

        [Fact]
        public async Task UpdateAppointmentAsync_ShouldModifyAppointment()
        {
            // Arrange
            var appointment = new Appointment
            {
                Date = DateTime.Now,
                PatientName = "John Doe",
                Dentist = "Dr. Smith",
                Procedure = "Cleaning"
            };
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            var appointmentDto = new AppointmentDTO
            {
                Date = DateTime.Now.AddDays(1),
                PatientN = "Jane Doe",
                Dentist = "Dr. Brown",
                Procedure = "Filling"
            };

            // Act
            await _repository.UpdateAppointmentAsync(appointmentDto, appointment.Id);

            // Assert
            var updatedAppointment = await _context.Appointments.FindAsync(appointment.Id);
            Assert.NotNull(updatedAppointment);
            Assert.Equal("Jane Doe", updatedAppointment.PatientName);
            Assert.Equal("Dr. Brown", updatedAppointment.Dentist);
            Assert.Equal("Filling", updatedAppointment.Procedure);
        }
    }
}
