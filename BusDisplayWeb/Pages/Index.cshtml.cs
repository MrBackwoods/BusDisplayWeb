using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace BusDisplayWeb.Pages
{
    public class IndexModel : PageModel
    {
        // List for departures
        public List<Departure> Departures = new List<Departure>();

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

       // Get departures at page load
        public async Task<IActionResult> OnGetAsync()
        {
            Departures = await HTTPHandler.UpdateDepartures();
            return Page();
        }

        // Refresh departures when button is pressed
        public async Task<IActionResult> OnPostAsync()
        {
            Departures = await HTTPHandler.UpdateDepartures();
            return Page();
        }
    }
}