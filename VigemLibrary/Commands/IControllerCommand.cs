using VigemLibrary.Controllers;

namespace VigemLibrary.Commands
{
    public interface IControllerCommand
    {
        void ExecuteCommand(IController controller);
    }
}
