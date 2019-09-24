
namespace TechCertain.WebUI.Models.Programme
{ 
    public class DropdownList
    {

        public DropdownList( string text, string value)
        {
            Text = text;
            Value = value;
        }

       

        public virtual string Text { get;  set; }

        public virtual string Value { get;  set; }
    }
}
