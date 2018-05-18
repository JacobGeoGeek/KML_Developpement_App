using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Import all these librairys note that the Newtonsoft, SQLite and Sharpkml are opens sources librayries from the Nuget package.
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Xml.Linq;
using System.Data.SQLite;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;

namespace kml
{
    class Program
    {
        
        static void Main(string[] args)
        {
            
      
            //create list of object named logfile. the logfile is a public struct in the file database_connection
            List<logfile> dataresults = new List<logfile>();


            database_connection conecct_toDB = new database_connection();

            //connect the database.
            conecct_toDB.connect();

            //list constaint all the element of the DB with their properties and values
            List<logfile> test = new List<logfile>();
           
            //the ExecuteSQL will return all element need for displaying into the Google earth
            test = conecct_toDB.ExecuteSQL("SELECT location.id, location.json, location.lat, location.lng, location.synced, photo.filename from location join photo on location.id = photo.location_id");

        
            //begin of the creation of kml file 
           
            Kml kml = new Kml();
            Document document = new Document();
            kml.Feature = document; 

            foreach (logfile x in test)
            {
              
                Point point = new Point();
                Placemark placemark = new Placemark();
                ExtendedData extenddata = new ExtendedData();
                Data databearing = new Data();
                Data datadistance = new Data();
                Data datalat = new Data();
                Data datalon = new Data();
                Data datasource = new Data();
                Data datasynced = new Data();
                Data dataID = new Data();
                Data dataLandUseType = new Data();

                    
                databearing.Name = "bearing";
                databearing.Value = x.bearing.ToString();

                datadistance.Name = "distance";
                datadistance.Value = x.distance.ToString();

                datalat.Name = "lat";
                datalat.Value = x.lat.ToString();

                datalon.Name = "lon";
                datalon.Value = x.lng.ToString();

                datasource.Name = "source";
                datasource.Value = x.source;

                datasynced.Name = "synced";
                datasynced.Value = x.synced.ToString();

                dataID.Name = "id";
                dataID.Value = x.ID.ToString();

                dataLandUseType.Name = "land_use_type";
                dataLandUseType.Value = x.land_use_type.ToString();

                extenddata.AddData(databearing);
                extenddata.AddData(datadistance);
                extenddata.AddData(datalat);
                extenddata.AddData(datalon);
                extenddata.AddData(datasource);
                extenddata.AddData(datasynced);
                extenddata.AddData(dataID);
                extenddata.AddData(dataLandUseType);

                int index = 1;
                foreach (string singlephoto in x.photopath)
                {
                    
                    Data des = new Data();
                    string filename = Path.GetFileName(singlephoto);
                    filename = filename.Remove(0, 4);
                    string pathv2 = Path.GetFullPath(filename);
                    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + @"Example\Example\NewCamera\" + filename);
                    path = path.Replace("\\", "/");

                    des.Name = "photo " + index.ToString();
                  des.Value  = string.Format("<img src=\"file:///{0}\" style=\"width:60%;height:60%; \"/>", path);
                    extenddata.AddData(des);
                    index++;
                    
                }



                point.Coordinate = new Vector(x.lat, x.lng);


            

                placemark.Geometry = point;
                placemark.ExtendedData = extenddata;
                
                document.AddFeature(placemark);
                

            }


            //write the kml file
            KmlFile kmlfile = KmlFile.Create(kml, false);
            using (var stream = File.OpenWrite(Path.Combine(AppDomain.CurrentDomain.BaseDirectory + @"Example\Example\" + "kmlfile.kml")))
            {
                kmlfile.Save(stream);
            }

            //write the kmz file


            Console.WriteLine("Finish");

            KmzFile kmzfile = KmzFile.Create(kmlfile);
            using (var stream = File.OpenWrite(Path.Combine(AppDomain.CurrentDomain.BaseDirectory + @"Example\Example\" + "kmzfile.kmz")))
            {
                kmzfile.Save(stream);
            }


            Console.ReadLine();

        }
       

    }
}
