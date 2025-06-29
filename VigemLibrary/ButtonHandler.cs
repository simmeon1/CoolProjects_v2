using VigemLibrary.Mappings;

namespace VigemLibrary
{
    public class ButtonHandler(StopwatchControllerUser controllerUser)
    {
        private bool shoulderRightPressed = false; 
        private bool squarePressed = false;
        
        public bool HoldButton(ButtonMappings button)
        {
            return DoWork(button, true);
        }

        public bool ReleaseButton(ButtonMappings button)
        {
            return DoWork(button, false);
        }

        private bool DoWork(ButtonMappings button, bool buttonHeld)
        {
            if (button == ButtonMappings.ShoulderRight)
            {
                shoulderRightPressed = buttonHeld;
            } 
            else if (button == ButtonMappings.Square)
            {
                squarePressed = buttonHeld;
            }
            if (shoulderRightPressed && squarePressed)
            {
                controllerUser.PressButton(ButtonMappings.Square);
                return true;
            }
            return false;
        }
    }
}