using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Models.Meessages;
using Data.Models.Messasges;

namespace Data.Repositories;
public interface IMessagesRepository
{
    Task<IEnumerable<Message>> GetNewMessagesAsync();
    Task SetMessagesStatusAsync(IEnumerable<Message> messages, MessageStatus status);
}