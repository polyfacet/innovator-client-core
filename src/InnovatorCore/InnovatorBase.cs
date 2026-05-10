using Innovator.Client.IOM;


public class InnovatorBase
{
    protected readonly Innovator.Client.IOM.Innovator Inn;
    public InnovatorBase(Innovator.Client.IOM.Innovator inn) {
        Inn = inn;
    } 

    protected Item Identity {
        get {
            return Inn.GetIdentity();
        }
    }
}

