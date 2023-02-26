using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Data.Interfaces;
using Data.Models.Meessages;
using Data.Models.Messasges;

namespace Data.Repositories;
public class MessagesRepository : IMessagesRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public MessagesRepository(IConnectionFactory connectionFactory) =>
        _connectionFactory = connectionFactory;

    public async Task<IEnumerable<Message>> GetNewMessagesAsync()
    {
        const string sql = "select id, user, msg, date, type from messages where status = 0";
        
        using var connection = _connectionFactory.Build();
        connection.Open();
        using var transaction = await connection.BeginTransactionAsync();
        try
        {
            var messages = (await connection.QueryAsync<Message>(sql, transaction: transaction)).ToList();        
            await SetMessagesStatusAsync(messages, MessageStatus.InProgress, connection, transaction);
            transaction.Commit();

            return messages;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public Task SetMessagesStatusAsync(IEnumerable<Message> messages, MessageStatus status)
    {
        using var connection = _connectionFactory.Build();
        return SetMessagesStatusAsync(messages, status, connection, transaction: null);
    }

    private async Task SetMessagesStatusAsync(
        IEnumerable<Message> messages, 
        MessageStatus status,
        DbConnection connection,
        DbTransaction? transaction)
    {
        const string sql = "update messages set status = @status where id in @ids";

        await connection.ExecuteAsync(
            sql: sql, 
            param: new 
            {
                status = status, 
                ids = messages.Select(x => x.Id) 
            },
            transaction: transaction);
    }
}