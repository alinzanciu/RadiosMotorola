using Microsoft.EntityFrameworkCore;

namespace RadiosMotorola.Models
{
    public class RadioContext : DbContext
    {
        /// <summary>
        /// Constructor that opens the DB
        /// </summary>
        /// <param name="options">Connecting using a connection stream</param>
        public RadioContext(DbContextOptions<RadioContext> options) : base(options)
        {

        }

        //Creating a DB sett that relates to the Radio.cs script in the Model
        //this tells entity frameworks that the Radio class is something I want to replicate as a dataset threw our data context
        public DbSet<Radio> RadioItems { get; set; }

        //Now I supply a connection string in the appsettings.json in order to create a connection to the specific DB

    }
}
