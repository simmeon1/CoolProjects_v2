namespace FlightConnectionsDotCom_ClassLibrary
{
    public class ClosePrivacyPopupCommands
    {
        public string GetAllButtonsOnPage { get; } = "return document.querySelectorAll('button')";
        public string GetButtonText { get; } = "return arguments[0].innerText";
    }
}