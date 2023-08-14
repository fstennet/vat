# VAT (Vault Access Tool)
Small .NET 6 project that interfaces with Google Cloud Storage. Authentication/authorization flows are provided by Microsoft Identity Platform using Oauth and OpenId. It also runs Swagger for documentation.

## Table of Contents
- [Usage](#Usage)
- [Endpoints](#Endpoints)
- [Contributing](#contributing)
- [License](#license)

## Setup

### GCP
In order to connect to your GCP Storage bucket, enter the bucket name in `appsettings.json` file:

```
  "Cloud": {
    "Storage": {
      "BucketName": "my-bucket-name"
    }
  }
```

If you run this service inside GCP, you don't need to authenticate. Otherwise, download the service account JSON file and setup the `GOOGLE_APPLICATION_CREDENTIALS` env variable to refer to the JSON. See more [here](https://cloud.google.com/dotnet/docs/reference/Google.Cloud.Storage.V1/latest#authentication).

### Microsoft Identity Platform
1. To use Microsoft Identity Platform, create your Azure Active Directory tenant:
[https://learn.microsoft.com/en-us/azure/active-directory/develop/quickstart-create-new-tenant](https://learn.microsoft.com/en-us/azure/active-directory/develop/quickstart-create-new-tenant)

2. Register the service with Microsoft Identity Platform:
[https://learn.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app](https://learn.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app)

3. Expose scopes:
[https://learn.microsoft.com/en-us/azure/active-directory/develop/quickstart-configure-app-expose-web-apis](https://learn.microsoft.com/en-us/azure/active-directory/develop/quickstart-configure-app-expose-web-apis)

4. Grant scoped permissions to web API:
[https://learn.microsoft.com/en-us/azure/active-directory/develop/quickstart-configure-app-access-web-apis](https://learn.microsoft.com/en-us/azure/active-directory/develop/quickstart-configure-app-access-web-apis)

Make sure you add the `instance`, `domain`, `tenantId`, `clientId` and `scopes` to the service's config file or via environme variables.


## Endpoints

### List Files
Retrieves a list of files.  
Method: `GET`  
URL: `api/file/list`  
Response: an array of filenames with their content type and labels.  
```
[
  {
    "fileName": "string",
    "contentType": "string",
    "labels": {
      "additionalProp1": "string",
      "additionalProp2": "string",
      "additionalProp3": "string"
    }
  }
]
```

### Upload File
Stores a new file  
Method: `POST`  
URL: `api/file?filename=`  
Response: an object with the stored file metadata (filename, path, content type and labels)  
```
{
  "fileName": "string",
  "contentType": "string",
  "labels": {
    "additionalProp1": "string",
    "additionalProp2": "string",
    "additionalProp3": "string"
  },
  "fileString": "string"
}
```

### Get File
Fetches a new file  
Method: `GET`  
URL: `api/file`  
Response: a base64 string with the content of the file  

## License
The MIT License (MIT)

Copyright (c) 2023 Franklin Stennett

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
