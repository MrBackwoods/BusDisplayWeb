using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using GraphQL;
using System.Linq;

namespace BusDisplayWeb
{
    public static class HTTPHandler
    {
        // HTTP Client
        static HttpClient client = new HttpClient();

        // API URL
        public const string queryURL = "https://api.digitransit.fi/routing/v1/routers/finland/index/graphql";

        // Numbers of departures retrieved per stop
        public const int numberOfDeparturesRetrievedPerStop = 10;

        // List of bus stop IDs that are followed
        public static List<string> busStopIDs = new List<string>() { "tampere:4082", "tampere:4087" };

        // Function to set up HTTP client
        public static void SetHTTPClient()
        {
            client.BaseAddress = new Uri(queryURL);
            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Function for getting departures
        public static async Task<List<Departure>> UpdateDepartures()
        {
            // New list for departures
            List<Departure> departures = new List<Departure>();

            // Get departures from API
            foreach (string busStopID in busStopIDs)
            {
                // Form query
                string graphQLquery = "{ stops(ids: \"" + busStopID + "\") { name id stoptimesWithoutPatterns(numberOfDepartures: " + numberOfDeparturesRetrievedPerStop + ") { scheduledDeparture realtimeDeparture departureDelay realtime realtimeState serviceDay headsign realtimeDeparture trip { routeShortName }}}}";
                JObject queryAsJOBject = new JObject();
                queryAsJOBject.Add(new JProperty("query", graphQLquery));

                // Make POST call
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "");
                request.Content = new StringContent(queryAsJOBject.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request.RequestUri, request.Content);

                // Handle response data to departure objects
                JObject responseAsJobject = JObject.Parse(await response.Content.ReadAsStringAsync());
                JArray stops = (JArray)responseAsJobject["data"]["stops"];

                foreach (var stopItem in stops.Children())
                {
                    if (stopItem["stoptimesWithoutPatterns"].GetValue() != null)
                    {
                        JArray departuresArray = (JArray)stopItem["stoptimesWithoutPatterns"];

                        foreach (var departureItem in departuresArray.Children())
                        {
                            Departure departure = new Departure();
                            departure.headsign = departureItem["headsign"].GetValue().ToString();
                            departure.route = departureItem["trip"]["routeShortName"].GetValue().ToString();
                            departure.departureTime = TimeHandler.ConvertToLocalTimeDateTime((int)departureItem["realtimeDeparture"].GetValue() + (int)departureItem["serviceDay"].GetValue()).ToString("HH:mm");
                            departure.stopName = stopItem["name"].GetValue().ToString();
                            departures.Add(departure);
                        }
                    }
                }
            }

            // Order departure info and return them
            departures = departures.OrderBy(o => o.departureTime).ToList();
            return departures;
        }
    }
}
