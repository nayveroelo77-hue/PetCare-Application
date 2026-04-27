using SQLite;
using PetCare.Model;

namespace PetCare.Service
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection? _database;

        private async Task Init()
        {
            if (_database is not null)
                return;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "PetCare.db3");
            _database = new SQLiteAsyncConnection(dbPath);

            await _database.CreateTableAsync<UserAccount>();

            // FORCE SCHEMA RESET: If the existing table was created without PrimaryKey/AutoIncrement, 
            // the IDs will all be 0. We must drop and recreate to apply the new schema.
            try
            {
                // Check if any existing pet has an ID of 0 (invalid for the new schema)
                var tableInfo = await _database.GetTableInfoAsync("Pet");
                if (tableInfo.Count > 0)
                {
                    var hasZeroId = await _database.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Pet WHERE Id = 0");
                    if (hasZeroId > 0)
                    {
                        await _database.DropTableAsync<Pet>();
                        await _database.DropTableAsync<Appointment>();
                    }
                }
            }
            catch { /* Table might not exist yet */ }

            await _database.CreateTableAsync<Pet>();
            await _database.CreateTableAsync<Appointment>();

            await SeedAdminAsync();
            // Database is now initialized and clean for production.
        }

        private async Task SeedAdminAsync()
        {
            if (_database is null) return;

            var admin = await _database.Table<UserAccount>().Where(u => u.Role == "Admin").FirstOrDefaultAsync();
            if (admin == null)
            {
                var defaultAdmin = new UserAccount
                {
                    FullName = "System Administrator",
                    Email = "admin@petcare.com",
                    Password = "admin123",
                    Role = "Admin",
                    MobileNumber = "0000000000",
                    CreatedAt = DateTime.Now
                };
                await _database.InsertAsync(defaultAdmin);
            }
        }

        public async Task<UserAccount?> GetUserAsync(string email, string password)
        {
            await Init();
            return await _database!.Table<UserAccount>()
                .Where(u => u.Email == email && u.Password == password)
                .FirstOrDefaultAsync();
        }

        public async Task<int> SaveUserAsync(UserAccount user)
        {
            await Init();
            // Check if email already exists
            var existing = await _database!.Table<UserAccount>().Where(u => u.Email == user.Email).FirstOrDefaultAsync();
            if (existing != null) return -1; // Specific error for email taken

            return await _database.InsertAsync(user);
        }

        public async Task<bool> IsEmailTakenAsync(string email)
        {
            await Init();
            var count = await _database!.Table<UserAccount>().Where(u => u.Email == email).CountAsync();
            return count > 0;
        }

        public async Task<List<UserAccount>> GetUsersAsync()
        {
            await Init();
            return await _database!.Table<UserAccount>().OrderByDescending(u => u.CreatedAt).ToListAsync();
        }

        public async Task<int> DeleteUserAsync(UserAccount user)
        {
            await Init();
            return await _database!.DeleteAsync(user);
        }

        public async Task<int> DeleteUserByIdAsync(int userId)
        {
            await Init();

            if (userId <= 0)
            {
                return 0;
            }

            return await _database!.ExecuteAsync("DELETE FROM UserAccount WHERE Id = ? AND Role <> 'Admin'", userId);
        }

        public async Task<int> DeleteUserByEmailAsync(string email)
        {
            await Init();

            if (string.IsNullOrWhiteSpace(email))
            {
                return 0;
            }

            return await _database!.ExecuteAsync("DELETE FROM UserAccount WHERE Email = ? AND Role <> 'Admin'", email.Trim());
        }

        public async Task<int> UpdateUserAsync(UserAccount user)
        {
            await Init();
            return await _database!.UpdateAsync(user);
        }

        public async Task<int> UpdateClientUserAsync(UserAccount user)
        {
            await Init();

            if (user == null || string.IsNullOrWhiteSpace(user.Email) || user.Role == "Admin")
            {
                return 0;
            }

            if (user.Id > 0)
            {
                return await _database!.ExecuteAsync(
                    "UPDATE UserAccount SET FullName = ?, Email = ?, MobileNumber = ?, Password = ?, Role = ? WHERE Id = ? AND Role <> 'Admin'",
                    user.FullName,
                    user.Email,
                    user.MobileNumber,
                    user.Password,
                    user.Role,
                    user.Id);
            }

            return await _database!.ExecuteAsync(
                "UPDATE UserAccount SET FullName = ?, Email = ?, MobileNumber = ?, Password = ?, Role = ? WHERE Email = ? AND Role <> 'Admin'",
                user.FullName,
                user.Email,
                user.MobileNumber,
                user.Password,
                user.Role,
                user.Email);
        }

        // --- DASHBOARD METHODS ---
        public async Task<int> GetTotalCountAsync<T>() where T : new()
        {
            await Init();
            return await _database!.Table<T>().CountAsync();
        }

        public async Task<List<Appointment>> GetRecentAppointmentsAsync(int limit = 5)
        {
            await Init();
            return await _database!.Table<Appointment>()
                .OrderByDescending(a => a.DateTime)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<List<Pet>> GetRecentPetsAsync(int limit = 5)
        {
            await Init();
            return await _database!.Table<Pet>()
                .OrderByDescending(p => p.Id)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<List<Pet>> GetPetsAsync()
        {
            await Init();
            return await _database!.Table<Pet>().OrderByDescending(p => p.Id).ToListAsync();
        }

        public async Task<Pet?> GetPetAsync(int id)
        {
            await Init();
            return await _database!.Table<Pet>().Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> SavePetAsync(Pet pet)
        {
            await Init();
            return await _database!.InsertAsync(pet);
        }

        public async Task<int> UpdatePetAsync(Pet pet)
        {
            await Init();
            return await _database!.UpdateAsync(pet);
        }

        public async Task<int> DeletePetAsync(Pet pet)
        {
            await Init();
            return await _database!.DeleteAsync(pet);
        }

        public async Task<int> DeletePetByIdAsync(int petId)
        {
            await Init();

            if (petId <= 0)
            {
                return 0;
            }

            await _database!.ExecuteAsync("DELETE FROM Appointment WHERE PetId = ?", petId);
            return await _database.ExecuteAsync("DELETE FROM Pet WHERE Id = ?", petId);
        }

        public async Task<int> DeletePetByDetailsAsync(Pet pet)
        {
            await Init();

            if (pet == null)
            {
                return 0;
            }

            return await _database!.ExecuteAsync(
                "DELETE FROM Pet WHERE Name = ? AND Species = ? AND Breed = ? AND OwnerId = ?",
                pet.Name,
                pet.Species,
                pet.Breed,
                pet.OwnerId);
        }

        public async Task<List<UserAccount>> GetClientsAsync()
        {
            await Init();
            return await _database!.Table<UserAccount>().Where(u => u.Role == "Client").ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetMonthlyAppointmentTrendsAsync(int months = 6)
        {
            await Init();
            var startDate = DateTime.Now.AddMonths(-months);
            var appointments = await _database!.Table<Appointment>()
                .Where(a => a.DateTime >= startDate)
                .ToListAsync();

            var trends = new Dictionary<string, int>();
            for (int i = months - 1; i >= 0; i--)
            {
                var monthDate = DateTime.Now.AddMonths(-i);
                var monthKey = monthDate.ToString("MMM");
                var count = appointments.Count(a => a.DateTime.Month == monthDate.Month && a.DateTime.Year == monthDate.Year);
                trends[monthKey] = count;
            }
            return trends;
        }

        public async Task<List<Appointment>> GetAppointmentsAsync()
        {
            await Init();
            return await _database!.Table<Appointment>()
                .OrderByDescending(a => a.DateTime)
                .ToListAsync();
        }

        public async Task<List<Pet>> GetPetsByOwnerIdAsync(int ownerId)
        {
            await Init();
            return await _database!.Table<Pet>()
                .Where(p => p.OwnerId == ownerId)
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetAppointmentsByOwnerIdAsync(int ownerId)
        {
            await Init();
            // Fetch pet IDs first to filter appointments by owner
            var pets = await GetPetsByOwnerIdAsync(ownerId);
            var petIds = pets.Select(p => p.Id).ToList();

            if (!petIds.Any()) return new List<Appointment>();

            var appointments = await _database!.Table<Appointment>()
                .OrderByDescending(a => a.DateTime)
                .ToListAsync();
                
            return appointments.Where(a => petIds.Contains(a.PetId)).ToList();
        }

        public async Task<int> UpdateAppointmentStatusAsync(int appointmentId, string status)
        {
            await Init();
            var appointment = await _database!.Table<Appointment>().Where(a => a.Id == appointmentId).FirstOrDefaultAsync();
            if (appointment != null)
            {
                appointment.Status = status;
                return await _database.UpdateAsync(appointment);
            }
            return 0;
        }

        public async Task<int> CompleteAppointmentWithNotesAsync(int appointmentId, string notes)
        {
            await Init();
            var appointment = await _database!.Table<Appointment>().Where(a => a.Id == appointmentId).FirstOrDefaultAsync();
            if (appointment != null)
            {
                appointment.Status = "Completed";
                appointment.Notes = notes;
                return await _database.UpdateAsync(appointment);
            }
            return 0;
        }

        public async Task<int> DeleteAppointmentAsync(int appointmentId)
        {
            await Init();
            return await _database!.ExecuteAsync("DELETE FROM Appointment WHERE Id = ?", appointmentId);
        }

        public async Task<int> SaveAppointmentAsync(Appointment appointment)
        {
            await Init();
            return await _database!.InsertAsync(appointment);
        }

        public async Task<int> CancelAppointmentAsync(int appointmentId)
        {
            await Init();
            return await UpdateAppointmentStatusAsync(appointmentId, "Cancelled");
        }
    }
}
