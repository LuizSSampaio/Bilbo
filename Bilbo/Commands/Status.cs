using Bilbo.Models;
using Bilbo.Services;

namespace Bilbo.Commands;

public class Status : CommandFramework
{
    public Status(string command, string description, Option option, bool isGlobal) : base(command, description, option,
        isGlobal)
    {
    }

    public override void CommandAction()
    {
        throw new NotImplementedException();
    }
}