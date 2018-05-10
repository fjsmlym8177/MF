using MF.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Core
{
    public class ApiResponse
    {
        public static ApiResponse CreateSuccess(string message = "Success")
        {
            return new ApiResponse()
            {
                Msg = message,
                Code = CodeEnum.Success
            };
        }

        public static ApiResponse CreateError(string message)
        {
            return new ApiResponse()
            {
                Msg = message,
                Code = CodeEnum.Error
            };
        }

        public ApiResponse() { }

        public ApiResponse(object data)
        {
            Data = data;
            Code = CodeEnum.Success;
            Msg = "Success";
        }

        public string Msg { get; set; }

        public CodeEnum Code { get; set; }

        public object Data { get; set; }
    }

    public class ApiResponsePager<T> : ApiResponse<Pager<T>>
    {
        public ApiResponsePager(IPagedList<T> pagerData)
        {
            this.Data.TotalCount = pagerData.TotalCount;
            this.Data.Data = pagerData;
        }

        public ApiResponsePager(IList<T> data, int totalCount)
        {
            this.Data.Data = data;
            this.Data.TotalCount = totalCount;
        }

    }

    public class ApiResponse<T> : ApiResponse
    {
        public ApiResponse()
        {

        }

        public ApiResponse(T data)
        {
            Data = data;
            Msg = "Success";
            Code = CodeEnum.Success;
        }

        public new T Data { get; set; }
    }


    public enum CodeEnum
    {
        Success = 200,
        Error = 400,
        Fault = 500
    }
}
