using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Models;
using AutoMapper;
using Domain.Entities;

namespace Application.Serivices;

/// <inheritdoc/>
public sealed class MessageService(IMessageRepository repository, IMapper mapper) : IMessageService
{
    /// <inheritdoc/>
    public async Task<IEnumerable<MessageModel>> GetAllUserMessages(long chatId)
    {
        var messages = await repository.FindManyAsync(x => x.ChatId == chatId);

        var messagesModels = messages.Select(mapper.Map<MessageModel>);

        return messagesModels;
    }

    /// <inheritdoc/>
    public async Task InsertAsync(long chatId, int messageId)
    {
        var message = new Message()
        {
            ChatId = chatId,
            MessageId = messageId
        };

        await repository.InsertOneAsync(message);
    }

    /// <inheritdoc/>
    public Task DeleteAllUserMessages(long chatId)
        => repository.DeleteManyAsync(x => x.ChatId == chatId);
}
