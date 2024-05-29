using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Models;
using AutoMapper;
using Domain.Entities;
using OfficeOpenXml;

namespace Application.Serivices;

public sealed class VacancyService(IVacancyRepository repository, IUserService userService, IMapper mapper) : IVacancyService
{
    /// <inheritdoc/>
    public async Task<IEnumerable<VacancyModel>> GetFilterdVacanciesAsync(long userTelegramId)
    {
        var user = await userService.GetOrCreateUserByTelegramIdAsync(userTelegramId);
        var filter = user.VacancyFilter;

        var vacancies = await repository.FindManyAsync(x =>
            (filter.IsOnline == null || x.IsOnline == filter.IsOnline) &&
            (filter.City == null || x.City == filter.City) &&
            (filter.Type == null || x.Type == filter.Type) &&
            (filter.Direction == null || x.Direction == filter.Direction));

        return mapper.Map<IEnumerable<VacancyModel>>(vacancies);
    }

    /// <inheritdoc/>
    public async Task UploadVacanciesAsync(Stream stream)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var excelPackege = new ExcelPackage(stream);

        var sheet = excelPackege.Workbook.Worksheets.First();
        var cells = sheet.Cells;

        var vacancies = new List<Vacancy>();
        int row = 2;

        while (cells[row, 1].Value != null)
        {
            vacancies.Add(new Vacancy()
            {
                Name = cells[row, 1].Value.ToString()!.Trim(),
                City = cells[row, 2].Value.ToString()?.Trim(),
                Format = cells[row, 3].Value.ToString()?.Trim(),
                IsOnline = cells[row, 3].Value.ToString()?.Trim() == "Онлайн",
                Type = cells[row, 4].Value.ToString()?.Trim(),
                Schedule = cells[row, 5].Value.ToString()?.Trim(),
                Salary = cells[row, 6].Value.ToString()?.Trim(),
                Direction = cells[row, 7].Value.ToString()?.Trim(),
                DirectionDescription = cells[row, 8].Value.ToString()?.Trim()
            });

            row++;
        }

        await repository.DeleteManyAsync(x => true);

        await repository.InsertManyAsync(vacancies);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<string>> GetAllCitiesAsync()
    {
        var vacancies = await repository.FindManyAsync(x => x.City != null);

        var cities = vacancies.Select(x => x.City!).Distinct();

        return cities;
    }

    public async Task<bool> ValidateCityAsync(string city)
    {
        return await repository.FindOneAsync(x => x.City == city) != null;
    }
}
