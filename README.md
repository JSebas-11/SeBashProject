# 🐚 SeBashProject — Shell UNIX-like en C# + AI

SeBash es una implementación personalizada de un shell estilo Bash desarrollada en C#, inspirada en el proyecto de CodeCrafters Bash, pero expandida y adaptada a una arquitectura propia y extensible.

El objetivo inicial fue reproducir el comportamiento del shell real (parsing, ejecución, pipes, redirecciones, comandos builtin, history…), pero con una base sólida y modular para permitir agregar funcionalidades avanzadas más adelante.

## 🧠 Próximamente: Personalización del shell y mas cositas.

## 🤖 Nuevo: Comando builtin *tian* con IA (Gemini API)

`tian` es un asistente dentro del shell que te ayuda a entender, generar y analizar comandos o archivos directamente desde la terminal.
**tian es tu copiloto de línea de comandos:** explica comandos, resume contenido de archivos, genera comandos según tu intención, usa tu historial para sugerirte acciones y más. Todo sin salir del shell.

---

# 🚀 Funcionalidades principales
## 📜 Gestión de comandos

- Ejecución de comandos externos mediante execvp estilo UNIX.
- Soporte para programas con argumentos múltiples.
- Manejo correcto de stdin, stdout y stderr.

## 🔧 Comandos builtin implementados

- cd  
- pwd  
- echo  
- type  
- tian (<br>
        tian -e "cmd" → Explica un comando y detalla qué hace, riesgos, flags y comportamiento.<br>
        tian -ef <file> → Explica el contenido de un archivo (scripts, configs, logs, etc.).<br>
        tian -g "prompt" → Genera un comando basado en tu intención (ej: *"zip all images except png"*).<br>
        tian -s <file> → Resume un archivo (errores, causas, highlights o contenido general).<br>
        tian -h "prompt" → Genera sugerencias combinando tu historial (hasta las últimas 80 entradas) y tu prompt.<br>
  )
- history (<br>
        history → Muestra el historial completo. <br>
        history # → Obtiene las últimas *x* entradas.<br>
        history -c → Borra **todos** los elementos del historial.<br>
        history -c <cmd> → Elimina **todas las apariciones** de un comando específico en el historial.<br>
        history -r <file> → Carga historial desde un archivo.<br>
        history -w <file> → Escribe el historial completo en un archivo.<br>
        history -a <file> → Agrega las nuevas entradas del historial a un archivo existente.<br>
  )
- exit → con guardado automático del historial
  
## 🔄 Pipelines y redirecciones

### PIPES (|)
Soporte para ejecucion de comandos encadenados mediante pipes: 
- ls | grep txt | wc
Cada comando se ejecuta en una etapa del pipeline, recibiendo el stdout anterior y produciendo su propio stdout.

### REDIRECCIONES
#### - Salida estándar (stdout)
Sobrescribir archivo:
- echo hi > file.txt
- echo hi 1> file.txt

#### - Error estándar (stderr)
Sobrescribir archivo:
- echo hi 2> file.txt

#### - Append de salida/error
Agregar sin borrar contenido previo:
- echo hi >> file.txt
- echo hi 2>> file.txt

---

# 🏗️ Arquitectura y diseño interno

El proyecto sigue una estructura modular y extensible pensada para permitir agregar comandos y funcionalidades con facilidad. Ademas de algunos patrones de diseños adecuados para el contexto 
como DependencyInjection, Factory y Builder.

---

# ✨ Objetivos futuros

- 🤖 Integración de IA: Mjorar *tian* con más modos y comprensión contextual del shell
- 🎨 Personalización estetica: Builtin propio que permita una experiencia interactiva y moderna integrada directamente en el shell

---

# 🛠️ Instalación y configuración

## 1. Clonar o descargar el repositorio
Clona el proyecto con: git clone https://github.com/JSebas-11/SeBashProject.git; O descárgalo directamente desde GitHub.

## 2. Configurar variables y servicios
Desde el directorio raíz, abre el archivo `appsettings.json`.

#### 🔑 API de Gemini  
En la sección **GenerativeService**, ingresa tu **API key** para habilitar el builtin `tian`.  
Sin esta clave, las funciones de IA no estarán disponibles.

#### 🕘 Configuración del historial (HistoryConfig)  
También puedes ajustar el comportamiento del historial:

- **MaxHistory** → cantidad máxima de entradas almacenadas.  
- **SaveDuplicates** → permite o evita guardar comandos repetidos.  
- **Excludes** → lista de comandos que no se guardarán.  
- **FilePath** → ruta donde se persiste el historial.

Estos valores están ubicados en:

- `appsettings.json` (HistoryConfig, logs y servicio de IA)
- `resources/history.txt` (archivo donde se guarda el historial)

## 3. Ejecutar
Desde la carpeta principal del proyecto, ejecutar: dotnet run Program.cs
