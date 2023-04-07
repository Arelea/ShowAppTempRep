using System;

namespace AppNov14.Models.Base
{
    public class MethodResult
    {
        public string Message { get; set; }

        public bool IsSuccess { get; set; }

        public int Id { get; set; }

        public MethodResult(bool isSuccess, string message, int id)
        {
            this.Message = message;
            this.IsSuccess = isSuccess;
            this.Id = id;
        }

        public MethodResult(bool isSuccess, string message)
        {
            this.Message = message;
            this.IsSuccess = isSuccess;
        }

        public MethodResult(bool isSuccess)
        {
            this.IsSuccess = isSuccess;
        }

        public MethodResult(bool isSuccess, int id)
        {
            this.IsSuccess = isSuccess;
            this.Id = id;
        }

        public MethodResult()
        {
        }
    }
}