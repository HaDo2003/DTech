using DTech.Models.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DTech.Controllers
{
    [ApiController]
    [Route("api/location")]
    public class LocationController(
        EcommerceWebContext context
    ) : ControllerBase
    {
        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces()
        {
            var provinces = await context.Provinces
                .Select(p => new { p.Id, p.Name })
                .ToListAsync();
            return Ok(provinces);
        }

        [HttpGet("districts/{provinceId}")]
        public async Task<IActionResult> GetDistrictsByProvince(int provinceId)
        {
            var districts = await context.Districts
                .Where(d => d.ProvinceId == provinceId)
                .Select(d => new { d.Id, d.Name })
                .ToListAsync();
            return Ok(districts);
        }

        [HttpGet("wards/{districtId}")]
        public async Task<IActionResult> GetWardsByDistrict(int districtId)
        {
            var wards = await context.Wards
                .Where(w => w.DistrictId == districtId)
                .Select(w => new { w.Id, w.Name })
                .ToListAsync();
            return Ok(wards);
        }
    }
}
