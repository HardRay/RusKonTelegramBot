using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Models;
using AutoMapper;
using Domain.Entities;
using OfficeOpenXml;

namespace Application.Serivices;

/// <inheritdoc/>
public sealed class CityService(ICityRepository repository, IMapper mapper) : ICityService
{
    /// <inheritdoc/>
    public async Task UploadCitiesAsync(Stream stream)
    {
        var cities = GetCitiesFromFileAsync(stream);

        await repository.DeleteManyAsync(x => true);
        await repository.InsertManyAsync(cities);
    }

    public async Task<IEnumerable<CityModel>> GetAllCitiesAsync()
    {
        var cities = await repository.FindManyAsync(x => true);

        return mapper.Map<IEnumerable<CityModel>>(cities);
    }

    private static IEnumerable<City> GetCitiesFromFileAsync(Stream stream)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var excelPackege = new ExcelPackage(stream);

        var sheet = excelPackege.Workbook.Worksheets[1];
        var cells = sheet.Cells;

        var cities = new List<City>();
        int row = 2;

        while (cells[row, 4].Value != null)
        {
            var city = new City()
            {
                Name = cells[row, 4].Value.ToString()!.Trim(),
                PhotoUrl = cells[row, 5].Value?.ToString()?.Trim()
            };

            cities.Add(city);

            row++;
        }

        return cities;
    }
}
