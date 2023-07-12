namespace Fiorello.Utilities.EmailSender.Abstract
{
    public interface IEmailSender
    {
        void SendEmail(Message message);
    }
}
