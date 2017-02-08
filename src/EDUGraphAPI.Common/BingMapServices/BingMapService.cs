/*   
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.  
 *   * See LICENSE in the project root for license information.  
 */
using Microsoft.Education.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace EDUGraphAPI
{
    public class BingMapService
    {
        private string BingMapApiURL = 
            "http://dev.virtualearth.net/REST/v1/Locations/US/{0}?output=json&key="+ WebConfigurationManager.AppSettings["BingMapKey"];


        /// <summary>
        /// Get longitude and latitude based on address.
        /// Reference URL: https://msdn.microsoft.com/en-us/library/ff701711.aspx.
        /// </summary>
        /// <param name="address">Address to get longitude and latitude.</param>
        /// <returns></returns>
        public async Task<List<string>> GetLongitudeAndLatitudeByAddress(string address)
        {
            List<string> result =new List<string>(2) ;
            var client = new HttpClient();
            var uri = string.Format(BingMapApiURL, address);
            try
            {
                var response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                JObject json = (JObject)JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                var coordinates = json["resourceSets"][0]["resources"][0]["point"]["coordinates"];
                result.Add(coordinates[0].ToString());
                result.Add(coordinates[1].ToString());
            }
            catch
            {
            }
            return result;
        }
    }
}
