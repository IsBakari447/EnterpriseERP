using Microsoft.AspNetCore.Authorization;
using EnterpriseERP.ApiModels;
using EnterpriseERP.Data;
using EnterpriseERP.Shared.DTOs.Supplier;
using EnterpriseERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseERP.Controllers.Api
{
    [Route("api/mobile/suppliers")]
    [ApiController]
[Authorize]
    public class SuppliersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SuppliersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/mobile/suppliers
        [HttpGet]
        public async Task<IActionResult> GetSuppliers()
        {
            var suppliers = await _context.Suppliers
                .OrderByDescending(s => s.Id)
                .Select(s => new SupplierDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    ContactPerson = s.ContactPerson,
                    Email = s.Email,
                    Phone = s.Phone,
                    Address = s.Address,
                    Category = s.Category,
                    CreatedAt = s.CreatedAt
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<SupplierDto>>
            {
                Success = true,
                Message = "Liste des fournisseurs récupérée avec succès.",
                Data = suppliers
            });
        }

        // GET: api/mobile/suppliers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSupplier(int id)
        {
            var supplier = await _context.Suppliers
                .Where(s => s.Id == id)
                .Select(s => new SupplierDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    ContactPerson = s.ContactPerson,
                    Email = s.Email,
                    Phone = s.Phone,
                    Address = s.Address,
                    Category = s.Category,
                    CreatedAt = s.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (supplier == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Fournisseur introuvable.",
                    Data = null
                });
            }

            return Ok(new ApiResponse<SupplierDto>
            {
                Success = true,
                Message = "Fournisseur récupéré avec succès.",
                Data = supplier
            });
        }

        // POST: api/mobile/suppliers
        [HttpPost]
        public async Task<IActionResult> CreateSupplier([FromBody] SupplierDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Le nom du fournisseur est obligatoire.",
                    Errors = new List<string> { "Name est obligatoire." }
                });
            }

            var supplier = new Supplier
            {
                Name = dto.Name,
                ContactPerson = dto.ContactPerson,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                Category = dto.Category,
                CreatedAt = DateTime.Now
            };

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            dto.Id = supplier.Id;
            dto.CreatedAt = supplier.CreatedAt;

            return Ok(new ApiResponse<SupplierDto>
            {
                Success = true,
                Message = "Fournisseur créé avec succès.",
                Data = dto
            });
        }

        // PUT: api/mobile/suppliers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSupplier(int id, [FromBody] SupplierDto dto)
        {
            var supplier = await _context.Suppliers.FindAsync(id);

            if (supplier == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Fournisseur introuvable."
                });
            }

            supplier.Name = dto.Name;
            supplier.ContactPerson = dto.ContactPerson;
            supplier.Email = dto.Email;
            supplier.Phone = dto.Phone;
            supplier.Address = dto.Address;
            supplier.Category = dto.Category;

            await _context.SaveChangesAsync();

            dto.Id = supplier.Id;
            dto.CreatedAt = supplier.CreatedAt;

            return Ok(new ApiResponse<SupplierDto>
            {
                Success = true,
                Message = "Fournisseur modifié avec succès.",
                Data = dto
            });
        }

        // DELETE: api/mobile/suppliers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);

            if (supplier == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Fournisseur introuvable."
                });
            }

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Fournisseur supprimé avec succès.",
                Data = new { Id = id }
            });
        }
    }
}