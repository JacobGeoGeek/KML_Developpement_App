using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Data.SQLite;  
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
namespace kml
{
    //logfile struct will represent each data while looping in the DB tables
    public struct logfile
    {
        public int ID { get; set; }
        public double bearing { get; set; }
        public int distance { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public int synced { get; set; }
        public string source { get; set; }
        public int land_use_type { get; set; }
        public List<string> photopath { get; set; }
        public logfile(int pID, double pBearing, int pDistance, double pLat, double pLng, int Psynced, string  pSource, int pLandUse, List<string> pPath )
        {
            ID = pID;
            bearing = pBearing;
            distance = pDistance;
            lat = pLat;
            lng = pLng;
            synced = Psynced;
            source = pSource;
            land_use_type = pLandUse;
            photopath = pPath;

        }

        public override string ToString()
        {
            return String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\n{8}", this.ID, this.bearing, this.distance, this.lat, this.lng, this.synced, this.source, this.land_use_type, this.source, this.filename());
           
        }

       private string filename()
        {
            string output="";

            foreach(string x in this.photopath)
            {
                output += x + "\n";
            }
            return output;
           
        }


    }

    public class database_connection
    {

        public SQLiteConnection my_DBCONNECTION;
   
        //method for connecting the DB 
        public void connect()
        {
            string connection = "Data Source="+ Path.Combine(AppDomain.CurrentDomain.BaseDirectory+ @"Example\Example\GlobalCroplands\DB.sqlite; Version =3");
            
           
                my_DBCONNECTION = new SQLiteConnection(connection);
            my_DBCONNECTION.Open();
        }

        //method for disconect the DB 
        public void distconnect()
        {
            if (my_DBCONNECTION.State == ConnectionState.Open)
                my_DBCONNECTION.Close();
        }


        //method for returning the data need for the kml or kmz file.
        public List<logfile> ExecuteSQL(string sqlcode)
        {
            List<logfile> results = new List<logfile>();
           
            logfile datareadline = new logfile();
            SQLiteCommand command = new SQLiteCommand(sqlcode, my_DBCONNECTION);
            SQLiteCommand photocommand;
             SQLiteDataReader reader = command.ExecuteReader();
            SQLiteDataReader photoreader;
            while (reader.Read())
            {
               
               if (datareadline.ID != int.Parse(reader["id"].ToString()))
                {
                    //get every data in the result of the SQL code
                    datareadline.ID = int.Parse(reader["id"].ToString());
                    JObject jsondata = JObject.Parse((string)reader["json"]);

                    datareadline.bearing = (double)jsondata["bearing"];
                    datareadline.distance = (int)jsondata["distance"];
                    datareadline.source = (string)jsondata["source"];
                    datareadline.land_use_type = (int)jsondata["records"].Values("land_use_type").Single();
                    datareadline.lat = (double)reader["lat"];
                    datareadline.lng = (double)reader["lng"];
                    datareadline.synced = int.Parse(reader["synced"].ToString());
                   
                    
                    //for the photos
                    List<string> photopath = new List<string>();

                    photocommand = new SQLiteCommand(String.Format("SELECT photo.filename from photo where location_id = {0}", datareadline.ID.ToString()), my_DBCONNECTION);
                    photoreader = photocommand.ExecuteReader();
                    while (photoreader.Read())
                    {
                        photopath.Add((string)photoreader["filename"]);
                    }
                    datareadline.photopath = photopath;
                 

                    results.Add(datareadline);

                 
                }
              
              
            }
            return results;
        }

    }
}
