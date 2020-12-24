using Microsoft.AspNetCore.Mvc;

namespace RxFair.Models
{
    public class JsonResponse
    {
        public IActionResult GenerateJsonResult(int status, string message = null, object data = null)
        {
            return new JsonResult(new JsonResponseHelper
            {
                Status = status,
                Data = data,
                Message = message
            });
        }
    }
    public class DatatableResponseHelper
    {
        public int TotalElements { get; set; }
        public object Data { get; set; }
    }
    
    public class JsonResponseHelper
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
