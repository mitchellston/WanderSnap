using Microsoft.AspNetCore.Mvc;

namespace P4_Vacation_photos.Classes.api;
class ApiResponse<T>
{
    public bool success { get; set; }
    public string message { get; set; }
    public T data { get; set; }
    public ApiResponse(bool success, string message, T data)
    {
        this.success = success;
        this.message = message;
        this.data = data;
    }
    public JsonResult CreateJsonResult(bool success, string message, T data)
    {
        this.success = success;
        this.message = message;
        this.data = data;
        return new JsonResult(this);
    }
}

