using Common_ClassLibrary;

namespace AutoInput
{
    public class Action
    {
        public ActionType Type { get; set; }
        public string[] Arguments { get; set; }
        public bool Enabled { get; set; }

        public override string ToString()
        {
            return Type + " - " + Enabled  + (Type == ActionType.SetStates ? "" :  " - " + Arguments.SerializeObject());
        }
    }
}