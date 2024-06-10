using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Models;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Models;
using OfficeOpenXml;

namespace Application.Serivices;

/// <inheritdoc/>
public sealed class VacancyService(
    IVacancyRepository repository,
    IUserService userService,
    IMapper mapper) : IVacancyService
{
    /// <inheritdoc/>
    public async Task<IEnumerable<VacancyModel>> GetAllAsync()
    {
        var vacancies = await repository.FindManyAsync(x => true);

        return mapper.Map<IEnumerable<VacancyModel>>(vacancies);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<VacancyModel>> GetFilterdVacanciesAsync(long userTelegramId)
    {
        var user = await userService.GetOrCreateUserByTelegramIdAsync(userTelegramId);
        var filter = user.VacancyFilter;

        var vacancies = await repository.FindManyAsync(x =>
            (filter.Format == null || x.Format == null || x.Format == JobFormat.Hybrid || x.Format == filter.Format) &&
            (filter.City == null || x.City == filter.City) &&
            (filter.Type == null || x.Type == filter.Type) &&
            (filter.Direction == null || x.Direction == filter.Direction) &&
            (filter.ForStudents == x.ForStudents));

        return mapper.Map<IEnumerable<VacancyModel>>(vacancies);
    }

    /// <inheritdoc/>
    public async Task<VacancyModel?> GetVacancyByIdAsync(string id)
    {
        var vacancy = await repository.FindFirstOrDefaultAsync(x => x.Id == id);

        return mapper.Map<VacancyModel>(vacancy);
    }

    /// <inheritdoc/>
    public async Task<VacancyModel?> GetVacancyByNumberAsync(int number)
    {
        var vacancy = await repository.FindFirstOrDefaultAsync(x => x.Number == number);

        return mapper.Map<VacancyModel>(vacancy);
    }

    /// <inheritdoc/>
    public async Task UploadVacanciesAsync(Stream stream)
    {
        var vacancies = GetVacanciesFromFileAsync(stream);

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

    /// <inheritdoc/>
    public async Task<bool> ValidateCityAsync(string city)
    {
        return await repository.FindFirstOrDefaultAsync(x => x.City == city) != null;
    }

    /// <inheritdoc/>
    public async Task<bool> ValidateDirectionAsync(string direction)
    {
        return await repository.FindFirstOrDefaultAsync(x => x.Direction == direction) != null;
    }

    private static IEnumerable<Vacancy> GetVacanciesFromFileAsync(Stream stream)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var excelPackege = new ExcelPackage(stream);

        var sheet = excelPackege.Workbook.Worksheets.First();
        var cells = sheet.Cells;

        var vacancies = new List<Vacancy>();
        int row = 2;

        while (cells[row, 1].Value != null)
        {
            var additionalAttributes = new List<AdditionalAtribute>();
            const int additionalAttributesStartColumn = 8;
            var attributeColumn = additionalAttributesStartColumn;
            var attributeName = cells[1, attributeColumn]?.Value?.ToString()?.Trim();

            while (!string.IsNullOrEmpty(attributeName))
            {
                var columnValue = cells[row, attributeColumn]?.Value?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(columnValue))
                {
                    additionalAttributes.Add(new AdditionalAtribute()
                    {
                        Name = attributeName,
                        Value = columnValue
                    });
                }

                attributeColumn++;
                attributeName = cells[1, attributeColumn]?.Value?.ToString()?.Trim();
            }

            vacancies.Add(new Vacancy()
            {
                Name = cells[row, 1].Value.ToString()!.Trim(),
                City = cells[row, 2].Value?.ToString()?.Trim(),
                Format = GetJobFormat(cells[row, 3].Value?.ToString()?.Trim()),
                Type = cells[row, 4].Value?.ToString()?.Trim(),
                Direction = cells[row, 5].Value?.ToString()?.Trim(),
                Salary = cells[row, 6].Value?.ToString()?.Trim(),
                ForStudents = cells[row, 7].Value?.ToString()?.Trim() == "Да",
                Number = row - 1,
                AdditionalAtributes = additionalAttributes
            });

            row++;
        }

        return vacancies;
    }

    private static JobFormat? GetJobFormat(string? str)
        => str switch
        {
            "Оффлайн" => JobFormat.Offline,
            "Онлайн" => JobFormat.Online,
            "Гибрид" => JobFormat.Hybrid,
            _ => null
        };
}
