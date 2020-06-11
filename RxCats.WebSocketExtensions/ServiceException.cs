using System;

namespace RxCats.WebSocketExtensions
{
    public class ServiceException : Exception
    {
        public WebSocketMessageResultCode ResultCode { get; }
        
        public ServiceException(WebSocketMessageResultCode resultCode = WebSocketMessageResultCode.Error)
        {
            ResultCode = resultCode;
        }

        public ServiceException(string message, WebSocketMessageResultCode resultCode = WebSocketMessageResultCode.Error) : base(message)
        {
            ResultCode = resultCode;
        }
    }
}