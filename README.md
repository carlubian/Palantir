# Palantir
Generative AI system to interpret and propose hypothesis based on conversations.

## App configuration
Palantir needs a Garnet server in order to pass messages and events between its components.
This server can be deployed locally using a docker command like the following:
<pre>docker run -d -p 6400:6379 --name garnet-palantir ghcr.io/microsoft/garnet</pre>

In addition, the following appsetting.json files are needed:

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
    },
    "Prompts": {
      "System": [
        "" // Insert contents of the IMES System Prompt
      ]
    }
  }
}
```

#### System Prompt

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
  "    \"hipotesis\":",
  "    [",
  "        {",
  "            \"contenido\": \"<TEXTO QUE DESCRIBE LA HIPÓTESIS>\",",
  "            \"certeza\": 0.5 <NÚMERO ENTRE 0 Y 1 QUE INDICA LA PROBABILIDAD DE QUE ESTA HIPÓTESIS SEA CIERTA>",
  "        },",
  "        ...",
  "    ]",
  "}"
]
```

Note that the current prompt uses a generic user profile. Subsequent versions could
use a database to store and fetch specific profiles for certain users to replace the "Contexto adicional".

#### User Prompt

IMES receives as input a list of conversation turns between "Usuario" and "Interlocutor" codified
as an instance of Palantir.Core.Model.ConversationMessage.

The current test conversation can be found in the following file:
<pre>Palantir/IMES/Services/ImesService.cs</pre>

#### Demo output

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

It's apparent that the system is capable of interpreting natural language and making assumptions about
both direct and indirect language, as well as proposing unlikely (but nevertheless possible) hypothesis.

In addition, the LLM is capable of understanding the output format and creating a valid JSON syntax. This
is epecially useful to programatically handle that output in further steps or in a UI.
