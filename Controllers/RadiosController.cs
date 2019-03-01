using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using Microsoft.AspNetCore.Mvc;
using RadiosMotorola.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.CodeAnalysis;


namespace RadiosMotorola.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RadiosController : ControllerBase
    {

        #region Initialization of DB and contructor
        /// <summary>
        /// The variable is used to access the content of the DB
        /// </summary>
        private readonly RadioContext _context;

        //Constructor where I instantiate the _context (using contructor injection)
        public RadiosController(RadioContext context) => _context = context; //context comes directly from the Startup class, where I have registered the DB connection

        #endregion

        #region Parsing Strings methods - string comparison methods
        /// <summary>
        /// This list creates a list of available locations from a single string of multiple locations
        /// </summary>
        /// <param name="str">Passes the whole string of available locations</param>
        /// <returns></returns>
        public IList<string> AvailableLocationsList(string str)
        {
            IList<string> listofLocations = str.Split(',').ToList<string>();
            listofLocations.Reverse();

            return listofLocations;
        }

        public string PayloadFormattingFor_AllowedLocation(string availableLocations)
        {
            string result=null;

            //parsing of the allowedLocations into a list
            IList<string> listofLocations = availableLocations.Split(',').ToList<string>();
            listofLocations.Reverse();

            //If the list has items, then I create the first part of the output string 
            if (listofLocations!=null)
                result= "\"allowed_locations\": ["; //creates the first part of the string

            //Adding all elements in the list in the wanted formating for payload output 
            foreach (string availableLocation in listofLocations)
            {
                //I make the location case insensitive
                result = result + " \" " + availableLocation.ToString() + " \", ";
            }

            //adding the last string elements for the payload output
            return result + "]}";
        }

        /// <summary>
        /// Checks if the location exists in the available locations
        /// </summary>
        /// <param name="location">location filed in DB</param>
        /// <param name="availableLocations">Available location field in DB, separated into a list of items</param>
        /// <returns></returns>
        public bool IsLocation_PartOf_AvailableLocations(string location, IList<string> availableLocations)
        {
            bool res = false;

            foreach (string availableLocation in availableLocations)
            {
                //I make the location case insensitive
                if (availableLocation.Equals(location, StringComparison.OrdinalIgnoreCase)) res = true;
            }

            return res;
        }
#endregion

        #region Payload Construction
            /// <summary>
            /// Payload for the : /radios/{id}/location
            /// </summary>
            public void Json_GetId(string loc)
            {
            //creating the object that we will add in the Json string later
            PayloadGet_ID getId = new PayloadGet_ID()
                {
                    location = loc
                };

            //creates the address
            string filePath = @"Temp\Getid.json";

            //writing the Json File
            WriteJsonFile(filePath, JsonConvert.SerializeObject(getId));
        }

            /// <summary>
            /// Payload for the : /radios/{id}
            /// </summary>
            public void Json_PostId(string str, IList<string> strList)
            {
            //creating the object that we will add in the Json string later
            PayloadPost_ID postId = new PayloadPost_ID()
                {
                    alias= str,
                    allowed_location = strList
                };

            //creates the address
            string filePath = @"Temp\PostId.json";

            //writing the Json File
            WriteJsonFile(filePath, JsonConvert.SerializeObject(postId));
            }



            /// <summary>
            /// Payload for the : /radios/{id}/location
            /// </summary>
            /// <param name="loc"></param>
            public void Json_Post_Id_Location(string loc)
            {
                //creating the object that we will add in the Json string later
                PayloadPost_ID_Location postIdLoc = new PayloadPost_ID_Location()
                {
                    location = loc
                };

                //creates the address
                string filePath = @"Temp\PostId_Location.json";

               //writing the Json File
               WriteJsonFile(filePath, JsonConvert.SerializeObject(postIdLoc));
            }

        /// <summary>
        /// Writes Json file
        /// </summary>
        /// <param name="filePath">Location the file is saven at</param>
        /// <param name="stringResultJson">The Json string generated in each Post/Get action</param>
        public void WriteJsonFile(string filePath, string stringResultJson)
        {
            //Saving the Json file
            try
            {
                //This file replaces the file for every postId action
                System.IO.File.WriteAllText(filePath, stringResultJson);

            }

            catch (Exception ex)
            {
                //Error in case the file could not be written
                Console.WriteLine("Error occured when trying to write Json file : " + ex.Message);
            }
        }
        #endregion

        #region Action results creation

        /// <summary>
        ///             1. GET : /radios/{id}/location
        ///              Expects an ID attribute to be passed threw the URI
        /// </summary>
        [HttpGet("{id}/location")]
        public ActionResult GetRadioItem(int id)
        {
            var radioItem = _context.RadioItems.Find(id);
            var location = radioItem.location;

            //I am checking to see if either the item does not exist or if the location is undefined (either null, empty or by using the undefined keyword)
            if (radioItem.location == "" || radioItem.location == "undefined" || radioItem.location == null) return NotFound();
            else
            {

                Json_GetId(location);  //Payload returned in Json format
                return Ok();           //Action result
            }

        }



        /// <summary>
        ///             2. POST : /radios/{id}
        ///              Expects an ID attribute to be passed threw the URI
        /// </summary>
        [HttpPost("{id}")]
        public void PostRadioItemId_and_AllowedLocations(Radio radio)
        {
            //Adding to DB and saving changes
            _context.RadioItems.Add(radio);
            _context.SaveChanges(); //it is important to save changes on the DB (DB Changes flushes the changes to the DataStore)

            //Creating the Payload message
            var alias = radio.alias;
            var allowedLocations = radio.allowed_locations;

            //Returning the required Payload in Json format
            Json_PostId(alias, AvailableLocationsList(allowedLocations));
        }

        /// <summary>
        ///             3. POST : /radios/{id}/location
        ///              Expects an ID attribute to be passed threw the URI
        /// </summary>
        [HttpPost("{id}/location")]
        public ActionResult PostRadioItem_and_LocationPermission(int id, Radio radio)
        {
            //Looking for the specific item in the DB
            var radioItem = _context.RadioItems.Find(id);

            //if (radioItem == null) return NotFound();
            //else    
            //{
                //creating the variables required to see if we can add a field or not
                var allowedLocations = radioItem.allowed_locations;
                IList<string> allowedLocationsList = AvailableLocationsList(allowedLocations);

                //Returning the required Payload
                if (IsLocation_PartOf_AvailableLocations(radio.location, allowedLocationsList))
                {
                    Console.WriteLine("!!! This is the new location "+ radio.location);
                    radioItem.location = radio.location;

                    //Adding modification to DB and saving changes
                    _context.Entry(radioItem).State = EntityState.Modified;
                    _context.SaveChanges(); //it is important to save changes on the DB (DB Changes flushes the changes to the DataStore)

                    //Payload return in Json
                   Json_Post_Id_Location(radio.location);
                    
                    //Action return 200 OK
                    return Ok();
                }
                //Action return 404 OK
                else return NotFound();
           // }
        }

        #endregion

    }
}
