using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using MapAlarm.Models;
using MapAlarm.Models;

namespace MapAlarm.Data
{
  public class AlarmZoneDatabase
  {
    readonly SQLiteAsyncConnection database;

    public AlarmZoneDatabase(string dbPath)
    {
      database = new SQLiteAsyncConnection(dbPath);
      database.CreateTableAsync<AlarmZone>().Wait();
    }

    public Task<List<AlarmZone>> GetAlarmZonesAsync()
    {
      //Get all alarmZones.
      return database.Table<AlarmZone>().ToListAsync();
    }

    public Task<AlarmZone> GetAlarmZoneAsync(int id)
    {
      // Get a specific alarmZone.
      return database.Table<AlarmZone>().FirstAsync();
    }

    public Task<int> SaveAlarmZoneAsync(AlarmZone alarmZone)
    {
      if (alarmZone.ID != 0)
      {
        // Update an existing alarmZone.
        return database.UpdateAsync(alarmZone);
      }
      else
      {
        // Save a new alarmZone.
        return database.InsertAsync(alarmZone);
      }
    }

    public Task<int> DeleteAlarmZoneAsync(AlarmZone alarmZone)
    {
      // Delete a alarmZone.
      return database.DeleteAsync(alarmZone);
    }
  }
}