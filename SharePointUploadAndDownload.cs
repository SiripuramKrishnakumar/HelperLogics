  public FileUploadResponse UploadFile(string folderName, string fileName, IFormFile fileToUpload)
        {
            FileUploadResponse uploadResponse = new();
            using (var ms = new MemoryStream())
            {
                fileToUpload.CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var buffer = new byte[ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                ms.Position = 0;
                var _graphServiceClient = _configuration.GetGraphServiceClientService();
                string fileNameWithpath = string.Empty;
                if (string.IsNullOrEmpty(folderName)) fileNameWithpath = fileName;
                else fileNameWithpath = $"{folderName}/{fileName}";
                var response = _graphServiceClient.Sites["root"]
                    .Drives[_keys.SharePointDriveId].Root.ItemWithPath(fileNameWithpath).Content
                    .Request().PutAsync<DriveItem>(ms).Result;
                if (response != null && response != null)
                {
                    uploadResponse = DownloadFile(folderName, fileName);
                    uploadResponse.Result = "Success";
                    uploadResponse.Message = "Uploaded Successfuly";
                }
                else
                {
                    uploadResponse.Result = "Failed";
                    uploadResponse.Message = "Failed to Uploaded";
                }
                ms.Dispose();
                return uploadResponse;
            }
        }
        public FileUploadResponse UploadFile(string folderName, string fileName, Stream content)
        {
            FileUploadResponse uploadResponse = new();
            var _graphServiceClient = _configuration.GetGraphServiceClientService();
            string fileNameWithpath = string.Empty;
            if (string.IsNullOrEmpty(folderName)) fileNameWithpath = fileName;
            else fileNameWithpath = $"{folderName}/{fileName}";
            var response = _graphServiceClient.Sites["root"]
                .Drives[_keys.SharePointDriveId].Root.ItemWithPath(fileNameWithpath).Content
                .Request().PutAsync<DriveItem>(content);
            if (response != null)
            {
                uploadResponse = DownloadFile(folderName, fileName);
                uploadResponse.Result = "Success";
                uploadResponse.Message = "Uploaded Successfuly";
            }
            else
            {
                uploadResponse.Result = "Failed";
                uploadResponse.Message = "Failed to Uploaded";
            }
            return uploadResponse;
        }
        public FileUploadResponse DownloadFile(string folderName, string fileName)
        {
            FileUploadResponse uploadResponse = new();
            try
            {
                string driveId = _keys.SharePointDriveId;
                string fileNameWithpath = string.Empty;
                if (string.IsNullOrEmpty(folderName)) fileNameWithpath = fileName;
                else fileNameWithpath = $"{folderName}/{fileName}";
                string downlaodUrl = "https://graph.microsoft.com/v1.0/drives/" + driveId + "/root:/" + fileNameWithpath;

                using HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = _configuration.GetGraphAuthenticationHeaderValue().Result;
                HttpResponseMessage responseMessage = client.GetAsync(downlaodUrl).Result;
                if (responseMessage.IsSuccessStatusCode)
                {
                    string responseStr = responseMessage.Content.ReadAsStringAsync().Result;
                    string jsonStr = responseStr.Replace("@microsoft.graph.downloadUrl", "downlaodURL");
                    uploadResponse = JsonConvert.DeserializeObject<FileUploadResponse>(jsonStr);
                    uploadResponse.Result = "Success";
                }
                else if(responseMessage != null && responseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    uploadResponse.Result = "Failed";
                    uploadResponse.Message = "File Not Found";
                }
                else
                {
                    uploadResponse.Result = "Failed";
                    uploadResponse.Message = "Failed to Fetch File Details";
                }
            }
            catch
            {
                uploadResponse.Result = "Failed";
                uploadResponse.Message = "Failed to Fetch File Details";
            }
            return uploadResponse;
        }
        
        
        
        
          public static GraphServiceClient GetGraphServiceClientService(this IConfiguration configuration)
        {
            var scopes = new string[] { "https://graph.microsoft.com/.default" };
            string tenantId = configuration.GetValue<string>("Keys:TenantId");
            string clientId = configuration.GetValue<string>("Keys:SharePointClientId");
            string clientSecret = configuration.GetValue<string>("Keys:SharePointClientSecret");
            string tokenURL = "https://login.microsoftonline.com/" + tenantId + "/v2.0";

            return new GraphServiceClient(new DelegateAuthenticationProvider(async (requestMessage) =>
               {
                   var authResult = await ConfidentialClientApplicationBuilder.Create(clientId).WithAuthority(tokenURL)
                .WithClientSecret(clientSecret).Build().AcquireTokenForClient(scopes).ExecuteAsync();
                   requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);
               }));
        }
     
        public static async Task<AuthenticationHeaderValue> GetGraphAuthenticationHeaderValue(this IConfiguration configuration)
        {
            var scopes = new string[] { "https://graph.microsoft.com/.default" };
            string tenantId = configuration.GetValue<string>("Keys:TenantId");
            string clientId = configuration.GetValue<string>("Keys:SharePointClientId");
            string clientSecret = configuration.GetValue<string>("Keys:SharePointClientSecret");
            string tokenURL = "https://login.microsoftonline.com/" + tenantId + "/v2.0";

            var authResult = await ConfidentialClientApplicationBuilder.Create(clientId).WithAuthority(tokenURL)
                .WithClientSecret(clientSecret).Build().AcquireTokenForClient(scopes).ExecuteAsync();
            return new AuthenticationHeaderValue("Bearer", authResult.AccessToken);
        }
