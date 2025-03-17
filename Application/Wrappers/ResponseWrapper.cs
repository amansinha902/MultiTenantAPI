using javax.xml.ws;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Wrappers
{
    public class ResponseWrapper: IResponseWrapper
    {
        public List<string> Messages { get; set; } = [];
        public bool IsSuccessful { get; set; }
        public ResponseWrapper() 
        {

        }
        public static IResponseWrapper Fail() 
        {
            return new ResponseWrapper { IsSuccessful = false };
        }
        public static IResponseWrapper Fail(string messgae)
        {
            return new ResponseWrapper { IsSuccessful = false, Messages = [messgae] };
        }
        public static IResponseWrapper Fail(List<string> messgae)
        {
            return new ResponseWrapper { IsSuccessful = false, Messages = messgae };
        }
        public static Task<IResponseWrapper> FailAsync()
        {
            return Task.FromResult(Fail());
        }
        public static Task<IResponseWrapper> FailAsync(string message)
        {
            return Task.FromResult(Fail(message));
        }
        public static Task<IResponseWrapper> FailAsync(List<string> message)
        {
            return Task.FromResult(Fail(message));
        }
        public static IResponseWrapper Success()
        {
            return new ResponseWrapper { IsSuccessful = true };
        }
        public static IResponseWrapper Success(string messages)
        {
            return new ResponseWrapper { IsSuccessful = true, Messages = [messages] };
        }
        public static Task<IResponseWrapper> SuccessAsync(string message)
        {
            return Task.FromResult(Success(message));
        }
        public static Task<IResponseWrapper> SuccessAsync()
        {
            return Task.FromResult(Success());
        }
        public static IResponseWrapper Success(List<string> messages)
        {
            return new ResponseWrapper { IsSuccessful = true, Messages = messages };
        }
        public static Task<IResponseWrapper> SuccessAsync(List<string> message)
        {
            return Task.FromResult(Success(message));
        }
    }
    public class ResposeWrapper<T> : ResponseWrapper, IResponseWrapper<T> 
    {
        public ResposeWrapper()
        {
            
        }
        public T Data { get; set; }
        public static new IResponseWrapper<T> Fail()
        {
            return new ResposeWrapper<T> { IsSuccessful = false };
        }
        public static new IResponseWrapper<T> Fail(string message)
        {
            return new ResposeWrapper<T> { IsSuccessful = false, Messages = [message] };
        }
        public static new IResponseWrapper<T> Fail(List<string> message)
        {
            return new ResposeWrapper<T> { IsSuccessful = false, Messages = message };
        }
        public static new Task<IResponseWrapper<T>> FailAsync()
        {
            return Task.FromResult(Fail());
        }
        public static new Task<IResponseWrapper<T>> FailAsync(string message)
        {
            return Task.FromResult(Fail(message));
        }
        public static new Task<IResponseWrapper<T>> FailAsync(List<string> message)
        {
            return Task.FromResult(Fail(message));
        }
        public static new IResponseWrapper<T> Success()
        {
            return new ResposeWrapper<T> { IsSuccessful = true };
        }
        public static new IResponseWrapper<T> Success(string message)
        {
            return new ResposeWrapper<T> { IsSuccessful = true, Messages = [message] };
        }
        public static new IResponseWrapper<T> Success(T data)
        {
            return new ResposeWrapper<T> { IsSuccessful = true, Data = data };
        }
        public static new IResponseWrapper<T> Success(string message, T data)
        {
            return new ResposeWrapper<T> { IsSuccessful = true, Messages = [message], Data = data };
        }
        public static new IResponseWrapper<T> Success(List<string> message, T data)
        {
            return new ResposeWrapper<T> { IsSuccessful = true, Messages = message, Data = data };
        }
        public static new Task<IResponseWrapper<T>> SuccessAsync()
        {
            return Task.FromResult(Success());
        }
        public static new Task<IResponseWrapper<T>> SuccessAsync(string message)
        {
            return Task.FromResult(Success(message));
        }
        public static new Task<IResponseWrapper<T>> SuccessAsync(T data)
        {
            return Task.FromResult(Success(data));
        }
        public static new Task<IResponseWrapper<T>> SuccessAsync(string message, T data)
        {
            return Task.FromResult(Success(message, data));
        }
        public static new Task<IResponseWrapper<T>> SuccessAsync(List<string> message, T data)
        {
            return Task.FromResult(Success(message, data));
        }
    }
}
