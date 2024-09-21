using MailChimp;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;

namespace SmartNotificationService.Processor
{
    public class NotificationProcessor
    {
        private readonly TwilioRestClient _twillioRestClient;
        private readonly MailChimpManager _mailChimpManager;
        public NotificationProcessor()
        {
            _twillioRestClient = new TwilioRestClient("username", "password");
            _mailChimpManager = new MailChimpManager("apiKey");
        }

        public async Task SendSms(string to, string message)
        {
            MessageResource composeMessage = await MessageResource.CreateAsync
                (
                    body: message,
                    from: new Twilio.Types.PhoneNumber(""),
                    to: new Twilio.Types.PhoneNumber(to),
                    client: _twillioRestClient
                );
        }

        //public async Task SendMail(string to, string subject, string body)
        //{
        //    var composeMail = new EmailMessage
        //    {


        //    };
        //    await _mailChimpManager.Mess
        //}
    }
}
