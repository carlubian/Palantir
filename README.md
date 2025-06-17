# Palantir
Generative AI system to interpret and propose hypothesis based on conversations.

## App configuration
To make this work, the following appsetting.json files are needed:

### Palantir.IMES
The file needs to be located in <code>./Palantir.IMES/appsettings.json</code>

```json
{
  "Horizon": {
    "Component": "Palantir.IMES",
    "Logs": {
      "Provider": "Seq",
      "Endpoint": "http://localhost:5341",
      "LogLevel": "Debug"
    },
    "Messaging": {
      "Provider": "Garnet",
      "Endpoint": "localhost:6400"
    }
  },
  "Palantir": {
    "Cloud": {
      "Endpoint": "https://***.openai.azure.com/", // Replace with Azure OpenAI endpoint
      "ApiKey": "***", // Replace with Azure OpenAI API key
      "Deployment": "gpt-4.1" // Replace with Azure OpenAI deployment name
    }
  }
}
```
