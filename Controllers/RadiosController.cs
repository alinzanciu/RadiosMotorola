using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using Microsoft.AspNetCore.Mvc;
using RadiosMotorola.Models;
using Microsoft.EntityFrameworkCore;

namespace RadiosMotorola.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RadiosController : ControllerBase
    {

        /// <summary>
        /// The variable is used to access the content of the DB
        /// </summary>
        private readonly RadioContext _context;

        //Constructor where I instantiate the _context (using contructor injection)
        public RadiosController(RadioContext context) => _context = context; //context comes directly from the Startup class, where I have registered the DB connection

        /// <summary>
        /// This list creates a list of available locations from a single string of multiple locations
        /// </summary>
        /// <param name="str">Passes the whole string of available locations</param>
        /// <returns></returns>
        public IList<string> AvailableLocations(string str)
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


        //Creating the action results

        /// <summary>
        ///             1. GET : api/radios/{id}/location
        //              Expects an ID attribute to be passed threw the URI
        /// </summary>
        [HttpGet("{id}/location")]
        public ActionResult<string> GetRadioItem(int id)
        {
            var radioItem = _context.RadioItems.Find(id);
            var location = radioItem.location;


            if (radioItem == null) return NotFound();
            else return "Return: 200 OK { \"location\": \" " + location + " \" }";
        }



        /// <summary>
        ///             2. POST : /radios/{id}
        //              Expects an ID attribute to be passed threw the URI
        /// </summary>
        [HttpPost("{id}")]
        public ActionResult<string> PostRadioItemId_and_AllowedLocations(Radio radio)
        {
            //adding to DB and saving changes
            _context.RadioItems.Add(radio);
            _context.SaveChanges(); //it is important to save changes on the DB (DB Changes flushes the changes to the DataStore)

            //creating the Payload message
            var alias = radio.alias;
            var allowedLocations = radio.allowed_locations;

            //Returning the required Payload
            return "Payload { \"alias\": \" " + alias + " \"," + PayloadFormattingFor_AllowedLocation(allowedLocations);
        }

        /// <summary>
        ///             2. POST : /radios/{id}/location
        //              Expects an ID attribute to be passed threw the URI
        /// </summary>
        [HttpPost("{id}/location")]
        public ActionResult<string> PostRadioItem_and_LocationPermission(int id, Radio radio)//[FromBody] string location)
        {
            //look for the specific item in the DB
            var radioItem = _context.RadioItems.Find(id);

            if (radioItem == null) NotFound();
            if (radioItem != null) NotFound();
            {
                //creating the variables required to see if we can add a field or not
                var allowedLocations = radioItem.allowed_locations;
                IList<string> allowedLocationsList = AvailableLocations(allowedLocations);

                //Returning the required Payload
                if (IsLocation_PartOf_AvailableLocations(radio.location, allowedLocationsList))
                {
                    radioItem.location = radio.location;

                    //Adding modification to DB and saving changes
                    _context.Entry(radioItem).State = EntityState.Modified;
                    _context.SaveChanges(); //it is important to save changes on the DB (DB Changes flushes the changes to the DataStore)

                    //Payload return
                    return "Payload { \"location\": \" " + radio.location + " \"}" + "\nReturn: 200 OK";
                }

                else
                    return "Payload { \"location\": \" " + radio.location + " \"}" + "\nReturn: 403 FORBIDDEN";
            }
        }



    }
}
