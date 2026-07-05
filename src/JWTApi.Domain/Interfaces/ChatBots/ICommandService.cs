using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Interfaces.ChatBots
{
    public interface ICommandService
    {
        bool IsSpecialCommand(string message);
        Task<string> ExecuteCommand(string message, Guid userId);
    }
}
