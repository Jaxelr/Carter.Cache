using System;

namespace Sample.Carter.Cache.Redis.Application.Entities
{
    public class FailedResponse
    {
        public FailedResponse(Exception ex)
        {
            Message = ex.Message;
        }

        public string Message { get; set; }
    }
}
