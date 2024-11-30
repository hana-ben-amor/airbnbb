using airbnbb.Data;
using airbnbb.Models;
using Microsoft.EntityFrameworkCore;

public class AmenityService
{
    private readonly AppDbContext _context;

    public AmenityService(AppDbContext context)
    {
        _context = context;
    }

    // Create Amenity
    public async Task<Amenity> CreateAmenityAsync(Amenity amenity)
    {
        _context.Amenities.Add(amenity);
        await _context.SaveChangesAsync();
        return amenity;
    }

    // Get Amenity by Id
    public async Task<Amenity> GetAmenityByIdAsync(int id)
    {
        return await _context.Amenities.FirstOrDefaultAsync(a => a.Id == id);
    }

    // Get All Amenities
    public async Task<List<Amenity>> GetAllAmenitiesAsync()
    {
        return await _context.Amenities.ToListAsync();
    }

    // Update Amenity
    public async Task<Amenity> UpdateAmenityAsync(Amenity amenity)
    {
        _context.Amenities.Update(amenity);
        await _context.SaveChangesAsync();
        return amenity;
    }

    // Delete Amenity
    public async Task DeleteAmenityAsync(int id)
    {
        var amenity = await _context.Amenities.FindAsync(id);
        if (amenity != null)
        {
            _context.Amenities.Remove(amenity);
            await _context.SaveChangesAsync();
        }
    }
}
