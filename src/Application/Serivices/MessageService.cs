using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Models;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.Serivices;

/// <inheritdoc/>
public sealed class MessageService(IMessageRepository repository, IMapper mapper) : IMessageService
{
    /// <inheritdoc/>
    public async Task<MessageModel> GetLastUserMessageAsync(long chatId)
    {
        var message = await repository.FindLastOrDefaultAsync(x => x.ChatId == chatId);

        return mapper.Map<MessageModel>(message);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<MessageModel>> GetAllUserMessagesAsync(long chatId)
    {
        var messages = await repository.FindManyAsync(x => x.ChatId == chatId);

        var messagesModels = messages.Select(mapper.Map<MessageModel>);

        return messagesModels;
    }

    /// <inheritdoc/>
    public async Task InsertAsync(MessageModel message)
    {
        var entity = mapper.Map<Message>(message);

        await repository.InsertOneAsync(entity);
    }

    /// <inheritdoc/>
    public Task DeleteAllUserMessagesAsync(long chatId)
        => repository.DeleteManyAsync(x => x.ChatId == chatId);

    /// <inheritdoc/>
    public Task DeleteMessagesByIdsAsync(IEnumerable<int> ids)
        => repository.DeleteManyAsync(x => ids.Contains(x.MessageId));
}
