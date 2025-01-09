using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO;
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
			Console.WriteLine("helllo world ~~111111111111");
			string exedir = Directory.GetCurrentDirectory();
			Console.WriteLine(exedir);
			return View();
		}

		[HttpPost("sign")]
		public IActionResult Sign([FromBody] JObject post)
		{
			


			if (post == null || !post.ContainsKey("tbs") || !post.ContainsKey("pin"))
			{
				return BadRequest("require tbs and pin");
			}

			string exedir = Directory.GetCurrentDirectory();
			Console.WriteLine(exedir);
			

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
					byte[] buf = Convert.FromBase64String(post["tbs"].ToString());
					System.IO.File.WriteAllBytes(Path.Combine(logDir, "tbsTemp.txt"), buf);
				}
				else
				{
					System.IO.File.WriteAllText(Path.Combine(logDir, "tbsTemp.txt"), post["tbs"].ToString());
				}
				exereq["tbsFile"] = Path.Combine(logDir, "tbsTemp.txt");
			}
			else
			{
				if (post.ContainsKey("tbsEncoding") && post["tbsEncoding"].ToString() == "base64")
				{
					exereq["tbs"] = post["tbs"];
				}
				else
				{
					exereq["tbs"] = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(post["tbs"].ToString()));
				}
			}

			// Add other fields to exereq as needed
			if (post.ContainsKey("keyidb64") && post["keyidb64"].ToString() != "__NotExistedTag__")
			{
				exereq["keyidb64"] = post["keyidb64"];
			}
			if (post.ContainsKey("hashAlgorithm"))
			{
				exereq["hashAlgorithm"] = post["hashAlgorithm"];
			}
			if (post.ContainsKey("nonce") && post["nonce"].ToString() != "__NotExistedTag__")
			{
				exereq["nonce"] = post["nonce"];
			}
			if (post.ContainsKey("withCert") && post["withCert"].ToString() != "__NotExistedTag__")
			{
				exereq["withCert"] = post["withCert"].ToString() == "true";
			}
			if (post.ContainsKey("withKey") && post["withKey"].ToString() != "__NotExistedTag__")
			{
				exereq["withKey"] = post["withKey"].ToString() == "true";
			}
			if (post.ContainsKey("withSPKI") && post["withSPKI"].ToString() != "__NotExistedTag__")
			{
				exereq["withSPKI"] = post["withSPKI"].ToString() == "true";
			}
			if (post.ContainsKey("withSigningTime") && post["withSigningTime"].ToString() != "__NotExistedTag__")
			{
				exereq["withSigningTime"] = post["withSigningTime"].ToString() == "true";
			}
			if (post.ContainsKey("withCardSN") && post["withCardSN"].ToString() != "__NotExistedTag__")
			{
				exereq["withCardSN"] = post["withCardSN"].ToString() == "true";
			}
			if (post.ContainsKey("slotDescription"))
			{
				exereq["slotDescription"] = post["slotDescription"];
			}
			if (post.ContainsKey("signatureType"))
			{
				exereq["signatureType"] = post["signatureType"];
			}
			if (post.ContainsKey("checkValidity") && post["checkValidity"].ToString() != "__NotExistedTag__")
			{
				exereq["checkValidity"] = post["checkValidity"];
			}

			exereq["pkcs11"] = int.Parse(System.IO.File.ReadAllText(Path.Combine(exedir, "p11id.txt")).Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)[0]);


			//進入HiPKI.exe內部後應該還會再做一次json格式轉換所以傳進去的是json serial成string 在serial成string的東西，不然就會出現0x7600000B的錯誤
			string jsons = JsonConvert.SerializeObject(JsonConvert.SerializeObject(exereq));
			
			Console.WriteLine(jsons);


			var processStartInfo = new ProcessStartInfo
			{
				FileName = Path.Combine(exedir, "HiPKISign.exe"),
				Arguments = jsons,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};
			

			
			using (var process = new Process { StartInfo = processStartInfo })
			{
				Console.WriteLine("process start ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
				process.Start();
				string stdout = process.StandardOutput.ReadToEnd();
				string stderr = process.StandardError.ReadToEnd();
				process.WaitForExit();
				Console.WriteLine("process end ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
				if (System.IO.File.Exists(Path.Combine(logDir, "tbsTemp.txt")))
				{
					System.IO.File.Delete(Path.Combine(logDir, "tbsTemp.txt"));
				}

				if (process.ExitCode != 0)
				{
					Console.WriteLine("ExitCode !=0 ..");
					var errJ = new JObject
					{
						["ret_code"] = 0x7600000D,
						["func"] = "sign",
						["message"] = "MajorErrorReason",
						["serverVersion"] = "serverVersion"
					};
					return StatusCode(500, errJ.ToString());
				}
				else
				{
					var resultJson = JObject.Parse(stdout);
					Console.WriteLine(resultJson);
					if (resultJson["ret_code"]?.ToString() != "0")
					{
						Console.WriteLine("ret_code is not zero !...");
						resultJson["message"] = "MajorErrorReason";
						if (resultJson["last_error"] != null)
						{
							Console.WriteLine("last_error is not null !...");
							resultJson["message2"] = "MinorErrorReason";
						}
						return StatusCode(500, resultJson.ToString());
					}
					else
					{
						Console.WriteLine("everything is correct !....");
						// cacheCardSn = resultJson["cardSN"];
						// cacheCert = resultJson["certb64"];
						return Ok(resultJson.ToString());
					}
				}
			}
			Console.WriteLine("End HiPKISign  function ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
		}

		


		public class MyDataModel
		{
			public string? Name { get; set; }
			public int Age { get; set; }
		}


	}
}

