using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Models;
using AutoMapper;
using Domain.Entities;
using OfficeOpenXml;

namespace Application.Serivices;

/// <inheritdoc/>
public sealed class DirectionService(IDirectionRepository repository, IMapper mapper) : IDirectionService
{
    /// <inheritdoc/>
    public async Task UploadDirectionsAsync(Stream stream)
    {
        var directions = GetDirectionsFromFileAsync(stream);

        await repository.DeleteManyAsync(x => true);
        await repository.InsertManyAsync(directions);
    }

    public async Task<IEnumerable<DirectionModel>> GetAllDirectionsAsync()
    {
        var directions = await repository.FindManyAsync(x => true);

        return mapper.Map<IEnumerable<DirectionModel>>(directions);
    }

    private static IEnumerable<Direction> GetDirectionsFromFileAsync(Stream stream)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var excelPackege = new ExcelPackage(stream);

        var sheet = excelPackege.Workbook.Worksheets[1];
        var cells = sheet.Cells;

        var directions = new List<Direction>();
        int row = 2;

        while (cells[row, 7].Value != null)
        {
            var direction = new Direction()
            {
                Name = cells[row, 7].Value.ToString()!.Trim(),
                PhotoUrl = cells[row, 8].Value?.ToString()?.Trim()
            };

            directions.Add(direction);

            row++;
        }

        return directions;
    }
}