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

		[HttpPost("sign")]
		public IActionResult Sign([FromBody] JObject post)
		{
			Console.WriteLine("start Sign function ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

			
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

			// Add cacheCardSn and cacheCert if available
			//exereq["cardSN"] = "0002200325000079";
			//exereq["certb64"] = "MIIFpzCCBI+gAwIBAgIQX1ntQW/Pfo1ZFTqqydNuDDANBgkqhkiG9w0BAQsFADBgMQswCQYDVQQGEwJUVzEjMCEGA1UECgwaQ2h1bmdod2EgVGVsZWNvbSBDby4sIEx0ZC4xLDAqBgNVBAsMI1B1YmxpYyBDZXJ0aWZpY2F0aW9uIEF1dGhvcml0eSAtIEcyMB4XDTIzMDUzMTA2NTY0N1oXDTI4MDUzMTA2NTY0N1owWzELMAkGA1UEBhMCVFcxJzAlBgNVBAoMHuS4reiPr+mbu+S/oeiCoeS7veaciemZkOWFrOWPuDESMBAGA1UEAwwJ6YSt5a6H6LuSMQ8wDQYDVQQFEwY5MDExMjUwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQC8s4q+E+CBcsVtAGWWq5Y9S2g65IEKQXsO83fqUjHqqIXETyZrxypXoye7z+MoyNnzOTwHbUFeVjedVqyA3FzSAKcFDHlsyrIXkJGvI8RHlHx9hvxCsI5b8Axrjk+mLpoVIXPD0qj2BN3AOPo5fG2ItvLj2x38VZExawqc5EGE594lrPSdG2HnYnyy+G0lsJ9ZkRXaXHG2BTvVJCRmtNEPNmSbGJ5ZeRex0394KGZ4m7zeZJ2sBPzRFVmOMwtGaZUF/UUueIHuGg2Q50YDWjIyWza5HhC5sYmYQnXEloYjGG73BgLEZYshpJ4zgB4p6J44GEJ5s0xW3FOZdwkh6Yj1AgMBAAGjggJgMIICXDAfBgNVHSMEGDAWgBTLg31lFa+pyfOoqfRkfHlSBXRAYTAdBgNVHQ4EFgQUKcw1/I8NLCcaeVjQfJVVhpApXR8wgZwGA1UdHwSBlDCBkTBKoEigRoZEaHR0cDovL3JlcG9zaXRvcnkucHVibGljY2EuaGluZXQubmV0L2NybC9QdWJDQUcyLzEwMDAtMS9jb21wbGV0ZS5jcmwwQ6BBoD+GPWh0dHA6Ly9yZXBvc2l0b3J5LnB1YmxpY2NhLmhpbmV0Lm5ldC9jcmwvUHViQ0FHMi9jb21wbGV0ZS5jcmwwgZMGCCsGAQUFBwEBBIGGMIGDMEkGCCsGAQUFBzAChj1odHRwOi8vcmVwb3NpdG9yeS5wdWJsaWNjYS5oaW5ldC5uZXQvY2VydHMvSXNzdWVkVG9UaGlzQ0EucDdiMDYGCCsGAQUFBzABhipodHRwOi8vb2NzcC5wdWJsaWNjYS5oaW5ldC5uZXQvT0NTUC9vY3NwRzIwJwYDVR0gBCAwHjANBgsrBgEEAYG3I2QAAzANBgsrBgEEAYG3I2QACTAOBgNVHQ8BAf8EBAMCBsAwNAYDVR0lBC0wKwYIKwYBBQUHAwIGCCsGAQUFBwMEBgorBgEEAYI3CgMMBgkqhkiG9y8BAQUwCQYDVR0TBAIwADAlBgNVHREEHjAcgRpqb2hubnljaGVuZzk4OThAY2h0LmNvbS50dzBEBgNVHQkEPTA7MBAGCGCGdgFkAoFIMQQCAgPoMBEGCGCGdgFkAoFJMQUCAwGGoTAUBghghnYBZAKBSjEIDAY5MDExMjUwDQYJKoZIhvcNAQELBQADggEBAL4dCXCeqdt7zNhpR5xpEJERpBEsqLOAqjpdwD5jqB+AfW8BMtaU+vIwVzkBIE3kOpB/vs5/igh9yG6q4jf+MVOl0nVuz8Y1ol0QDWJ4ZYP8OJ22L8JTSPnhIOiDTkhvnwdpDqH1xELcT+pCmO6oG9VZyZHP3MalLIkUSev8+QaQx9KFOmYxFNpY7fz6H7l7cGUQhB8MmF1SWHfRQArxW5C2bR+je43u4mf1l2qoTR1rvq+WmU4p9FCvLICyR6LKm2yB7Eipfu4DL4pR391CXGKGk4AEGltxRycTWbTVTuITcBsYKctuj1xRRfY04EsFHC+/2kljZs+s83N0R0AfcoE=";
			exereq["pkcs11"] = int.Parse(System.IO.File.ReadAllText(Path.Combine(exedir, "p11id.txt")).Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)[0]);

			string jsons = JsonConvert.SerializeObject(exereq);
			Console.WriteLine(exereq);

			
			var processStartInfo = new ProcessStartInfo
			{
				FileName = Path.Combine(exedir, "HiPKISign.exe"),
				Arguments = jsons,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};
			

			Console.WriteLine("start HiPKISign  function ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
			using (var process = new Process { StartInfo = processStartInfo })
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

