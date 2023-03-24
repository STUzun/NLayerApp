using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.DTOs.Custom
{
    public class CustomResponseDto<T>
    {
        public T Data { get; set; }

        [JsonIgnore]
        public int StatusCode { get; set; }
        public List<string> Errors { get; set; }
        public static CustomResponseDto<T> Success(int satusCode, T data)
        {
            return new CustomResponseDto<T> { Data = data, StatusCode = satusCode };
        }
        public static CustomResponseDto<T> Success(int satusCode)
        {
            return new CustomResponseDto<T> { StatusCode = satusCode };
        }
        public static CustomResponseDto<T> Fail(int satusCode, List<string> errors)
        {
            return new CustomResponseDto<T> { StatusCode = satusCode, Errors = errors };
        }
        public static CustomResponseDto<T> Fail(int satusCode, string error)
        {
            return new CustomResponseDto<T> { StatusCode = satusCode, Errors = new List<string> { error } };
        }
    }
}
