using Microsoft.AspNetCore.Mvc;

namespace johnny_food_restaurant.Controllers
{
	[ApiController]
	[Route("api/HiPKI")]
	public class HiPKI : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

		[HttpPost]
		public IActionResult PostData([FromBody] MyDataModel data)
		{
			if (data == null)
			{
				return BadRequest("數據無效");
			}

			// 使用 Console.WriteLine 輸出數據
			Console.WriteLine($"接收到的數據: Name = {data.Name}, Age = {data.Age}");



			// 處理數據
			return Ok(new { message = "數據已成功接收", receivedData = data });
		}
	}


	public class MyDataModel
	{
		public string? Name { get; set; }
		public int Age { get; set; }
	}



}

