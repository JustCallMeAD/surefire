﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Identity;
using Mantis.Data;
using Mantis.Domain.Carriers.Models;


namespace Mantis.Domain.Carriers.Services
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarrierController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CarrierController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public List<Carrier> GetCarriersList()
        {
            return _context.Carriers.ToList();
        }

        [HttpPost]
        public object Post([FromBody] DataManagerRequest DataManagerRequest)
        {
            IEnumerable<Carrier> DataSource = GetCarriersList();

            if (DataManagerRequest.Search != null && DataManagerRequest.Search.Count > 0)
            {
                DataSource = DataOperations.PerformSearching(DataSource, DataManagerRequest.Search);
            }

            if (DataManagerRequest.Where != null && DataManagerRequest.Where.Count > 0)
            {
                DataSource = DataOperations.PerformFiltering(DataSource, DataManagerRequest.Where, DataManagerRequest.Where[0].Operator);
            }

            if (DataManagerRequest.Sorted != null && DataManagerRequest.Sorted.Count > 0)
            {
                DataSource = DataOperations.PerformSorting(DataSource, DataManagerRequest.Sorted);
            }
            int TotalRecordsCount = DataSource.Cast<Carrier>().Count();
            if (DataManagerRequest.Skip != 0)
            {
                DataSource = DataOperations.PerformSkip(DataSource, DataManagerRequest.Skip);
            }
            if (DataManagerRequest.Take != 0)
            {
                DataSource = DataOperations.PerformTake(DataSource, DataManagerRequest.Take);
            }


            
            return new { result = DataSource, count = TotalRecordsCount };
        }

        [HttpPost("Insert")]
        public void Insert([FromBody] CRUDModel<Carrier> Value)
        {
            _context.Carriers.Add(Value.Value);
            _context.SaveChangesAsync();
        }

        [HttpPost("Update")]
        public async void Update([FromBody] CRUDModel<Carrier> Value)
        {
            var existingOrder = _context.Carriers.Find(Value.Value.CarrierId);
            
            //existingOrder.CreatedBy = currentUser;
            if (existingOrder != null)
            {
                var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
                existingOrder.CreatedBy = currentUser;
                _context.Entry(existingOrder).CurrentValues.SetValues(Value.Value);
                _context.SaveChanges();
            }
        }

        [HttpPost("Delete")]
        public void Delete([FromBody] CRUDModel<Carrier> Value)
        {
            //BRTEAKPOINT DOES NOT HIT HERE
            var existingOrder = _context.Carriers.Find(Value.Value.CarrierId);
            //Delete Code
        }
    }
}
