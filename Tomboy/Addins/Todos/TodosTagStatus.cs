
namespace Tomboy.Todos
{
    public enum TodosTagStatus 
    {
        None = 0,
        NotDone = 1,
        Done = 2
    }

    public static class TodosTagStatusExtensions
    {
        public static string ToString(this TodosTagStatus me)
        {
            switch(me)
            {
            case TodosTagStatus.NotDone:
                return "Not done yet";
            case TodosTagStatus.Done:
                return "Done";
            default:
                return "Not set";
            }
        }

        public static char ToChar(this TodosTagStatus me)
        {
            switch(me)
            {
            case TodosTagStatus.NotDone:
                return '\u2715';
            case TodosTagStatus.Done:
                return '\u2714';
            default:
                return '\u2715';
            }
        }
    }
}