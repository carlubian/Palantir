# Palantir
Generative AI system to interpret and propose hypothesis based on conversations.

## App configuration
Palantir is designed to be deployed as part of a .NET Aspire architecture.

It has the following components:
- **ApiService**: Implements the SignalR hub and hosts the services required to query and process the model response.
- **AppHost**: Used to run the Aspire Dashboard and orchestrate the rest of the system.
- **Core**: Defines the model classes and contains the implementation of the services used by the ApiService.
- **Web**: Contains a Blazor website that exposes a UI so that users can interact with the model API.

### Palantir.ApiService
The file needs to be located in <code>./Palantir.ApiService/appsettings.json</code>

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Palantir": {
    "Cloud": {
      "Endpoint": "", // Replace with the Azure endpoint
      "ApiKey": "", // Replace with the Azure API key
      "Deployment": "gpt-4.1" // Replace with the Azure deployment name
    },
    "Prompts": {
      "System": [
        "", // Replace with the system prompt
      ]
    }
  }
}
```

## System Components

Palantir is divided into the following layers:

- Layer 1 (TBD): Queries a sentiment analysis model that scores each incoming message in the conversation.
- Layer 2 (IMES): Queries an LLM model that generates hypothesis based on a fragment of conversation.
- Layer 3 (TBD): Queries a series of LLM models in parallel that "play" a role and decide how they feel about the previous hypothesis.

## Layer 2: IMES

The IMES, or "Intelligent Meaning Extraction System" layer is intended to receive a fragment of
conversation between two users, and output a list of hypothesis as to what the "other user" is
meaning to say.

### System Prompt

The current version of the IMES System Prompt is the following:

```json
"System": [
  "# Introducción",
  "A continuación vas a recibir un fragmento de conversación entre 'Usuario' e 'Interlocutor'.",
  "'Usuario' representa al humano con el que estás hablando actualmente. 'Interlocutor' es otra persona con la que 'Usuario' está hablando.",
  "",
  "# Instrucciones",
  "Tu tarea es extraer una serie de hipótesis sobre lo que 'Interlocutor' está queriendo decir en la conversación, basándote en lo siguiente:",
  "- El significado literal de las palabras, como lo diría una persona directa y transparente",
  "- Otros significados que podrían estar implícitos en las palabras, como lo diría una persona indirecta o que asume demasiadas cosas",
  "- La posibilidad de que 'Interlocutor' esté mintiendo, ocultando información o manipulando a 'Usuario'",
  "- La posibilidad de que 'Interlocutor' esté utilizando un lenguaje figurado, como metáforas o ironía",
  "",
  "# Contexto adicional",
  "- 'Interlocutor' es una persona adulta que vive en Europa Occidental, con estudios superiores y habilidades sociales medias-altas.",
  "",
  "# Formato de respuesta",
  "La salida debe ser un JSON válido que siga el siguiente esquema:",
  "{",
  "    \"Entries\":",
  "    [",
  "        {",
  "            \"Content\": \"<TEXTO QUE DESCRIBE LA HIPÓTESIS>\",",
  "            \"Probability\": 0.5 <NÚMERO ENTRE 0 Y 1 QUE INDICA LA PROBABILIDAD DE QUE ESTA HIPÓTESIS SEA CIERTA>",
  "        },",
  "        ...",
  "    ]",
  "}"
]
```

Note that the current prompt uses a generic user profile. Subsequent versions could
use a database to store and fetch specific profiles for certain users to replace the "Contexto adicional".

### User Prompt

IMES receives as input a list of conversation turns between "Usuario" and "Interlocutor" codified
as an instance of Palantir.Core.Model.Conversation.

### Demo output

An invocation to IMES with a LLM-generated fake conversation yields a result like the following:

```json
{
    "hipotesis": [
        {
            "contenido": "Interlocutor está planeando una sorpresa para Usuario el sábado y no quiere que Usuario haga planes para poder llevarla a cabo.",
            "certeza": 0.85
        },
        {
            "contenido": "Interlocutor no quiere asistir a la merienda de Usuario y está buscando una excusa indirecta para no comprometerse.",
            "certeza": 0.3
        },
        {
            "contenido": "Interlocutor está utilizando el suspenso y el misterio para aumentar la expectación de Usuario y hacerlo sentir especial en torno a su cumpleaños.",
            "certeza": 0.7
        },
        {
            "contenido": "Interlocutor podría estar mintiendo sobre no saber lo que ocurrirá, ya que da varias pistas de que hay algo planeado.",
            "certeza": 0.6
        },
        {
            "contenido": "El uso de frases como 'los mejores momentos son los que no planeamos' y emoticonos podría indicar lenguaje figurado o ironía, aunque es más probable que refuerce la idea de una sorpresa.",
            "certeza": 0.5
        },
        {
            "contenido": "Interlocutor podría estar deseando que Usuario no haga planes, no necesariamente por una sorpresa, sino para tener flexibilidad por si aparece alguna otra actividad interesante a último momento.",
            "certeza": 0.2
        },
        {
            "contenido": "Interlocutor podría estar ocultando información relevante sobre planes concretos en el sábado para mantener el efecto sorpresa.",
            "certeza": 0.7
        }
    ]
}
```
