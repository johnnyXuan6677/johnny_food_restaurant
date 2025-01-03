using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

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
		/*
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
	*/
		private readonly ILogger<HiPKI> _logger;

		public HiPKI(ILogger<HiPKI> logger)
		{
			_logger = logger;
		}

		[HttpPost("sign")]
		public IActionResult Sign([FromBody] JObject post)
		{
			if (post == null || !post.ContainsKey("tbs") || !post.ContainsKey("pin"))
			{
				return BadRequest("require tbs and pin");
			}

			string exedir = Directory.GetCurrentDirectory();
			if (Environment.CurrentDirectory.ToUpper() == "C:\\Windows\\system32".ToUpper())
			{
				exedir = Environment.GetEnvironmentVariable("HOME");
			}

			var exereq = new JObject
			{
				["pin"] = post["pin"]
			};

			string tbs = post["tbs"].ToString();
			int tbsLen = tbs.Length;
			string logDir = "C:\\Users\\user\\Downloads"; // Adjust as needed

			if (tbsLen > 4000)
			{
				if (post.ContainsKey("tbsEncoding") && post["tbsEncoding"].ToString() == "base64")
				{
					byte[] buf = Convert.FromBase64String(tbs);
					System.IO.File.WriteAllBytes(Path.Combine(logDir, "tbsTemp.txt"), buf);
				}
				else
				{
					System.IO.File.WriteAllText(Path.Combine(logDir, "tbsTemp.txt"), tbs);
				}
				exereq["tbsFile"] = Path.Combine(logDir, "tbsTemp.txt");
			}
			else
			{
				if (post.ContainsKey("tbsEncoding") && post["tbsEncoding"].ToString() == "base64")
				{
					exereq["tbs"] = tbs;
				}
				else
				{
					exereq["tbs"] = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(tbs));
				}
			}

			// Add other fields from post to exereq as needed
			foreach (var field in new[] { "keyidb64", "hashAlgorithm", "nonce", "withCert", "withKey", "withSPKI", "withSigningTime", "withCardSN", "slotDescription", "signatureType", "checkValidity" })
			{
				if (post.ContainsKey(field) && post[field].ToString() != "__NotExistedTag__")
				{
					exereq[field] = post[field];
				}
			}

			// Add cacheCardSn and cacheCert if available
			string cacheCardSn = null; // Retrieve from cache if available
			string cacheCert = null; // Retrieve from cache if available
			if (!string.IsNullOrEmpty(cacheCardSn) && !string.IsNullOrEmpty(cacheCert))
			{
				exereq["cardSN"] = cacheCardSn;
				exereq["certb64"] = cacheCert;
			}

			exereq["pkcs11"] = int.Parse(System.IO.File.ReadAllText(Path.Combine(exedir, "p11id.txt")).Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)[0]);

			string jsons = exereq.ToString();
			System.IO.File.AppendAllText("C:\\Users\\user\\Downloads\\log.json", jsons + "\n");

			_logger.LogInformation(jsons);

			ProcessStartInfo startInfo = new ProcessStartInfo
			{
				FileName = Path.Combine(exedir, "HiPKISign.exe"),
				Arguments = jsons,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};

			using (Process process = new Process { StartInfo = startInfo })
			{
				process.Start();
				string stdout = process.StandardOutput.ReadToEnd();
				string stderr = process.StandardError.ReadToEnd();
				process.WaitForExit();

				if (System.IO.File.Exists(Path.Combine(logDir, "tbsTemp.txt")))
				{
					System.IO.File.Delete(Path.Combine(logDir, "tbsTemp.txt"));
				}

				if (process.ExitCode != 0)
				{
					_logger.LogError(stderr);
					return StatusCode(500, new { ret_code = "EXEC_FAIL_TIMEOUT", func = "sign", message = "MajorErrorReason", serverVersion = "serverVersion" });
				}
				else
				{
					_logger.LogInformation(stdout);
					var resultJson = JObject.Parse(stdout);
					if (resultJson["ret_code"].ToString() != "0")
					{
						resultJson["message"] = "MajorErrorReason";
						if (resultJson.ContainsKey("last_error"))
						{
							resultJson["message2"] = "MinorErrorReason";
						}
						return StatusCode(500, resultJson);
					}
					else
					{
						cacheCardSn = resultJson["cardSN"].ToString();
						cacheCert = resultJson["certb64"].ToString();
						return Ok(resultJson);
					}
				}
			}
		}

	}


	public class MyDataModel
	{
		public string? Name { get; set; }
		public int Age { get; set; }
	}



}

