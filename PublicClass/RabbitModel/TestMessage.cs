using System;
using EasyNetQ;
namespace PublicClass.RabbitModel
{
    public class TextMessage
    {
        public string From { get; set; }
        public string Text { get; set; }
    }


}
