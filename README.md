# 🐚 SeBashProject — Shell UNIX-like en C#

SeBash es una implementación personalizada de un shell estilo Bash desarrollada en C#, inspirada en el proyecto de CodeCrafters Bash, pero expandida y adaptada a una arquitectura propia y extensible.

El objetivo inicial fue reproducir el comportamiento del shell real (parsing, ejecución, pipes, redirecciones, comandos builtin, history…), pero con una base sólida y modular para permitir agregar funcionalidades avanzadas más adelante.

## 🧠 Próximamente: Integración de un comando builtin de IA, personalización del shell y mas cositas.

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
- history (
    history, 
    history # -> obtener ultimas x entradas, 
    history -r <file> -> lectura desde archivo
    history -w <file> -> escritura completa
    history -a <file> -> append incremental
)
- exit -> con guardado automático del historial

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

- 🤖 Integración de IA: Builtin propio que permita una experiencia interactiva y moderna integrada directamente en el shell
- 🎨 Personalización estetica: Builtin propio que permita una experiencia interactiva y moderna integrada directamente en el shell

---

# 🛠️ Instalación y configuración

## 1. Clonar o descargar el repositorio
Clona el proyecto con: git clone https://github.com/JSebas-11/SeBashProject.git; O descárgalo directamente desde GitHub.

## 2. Ejecutar
Desde la carpeta principal del proyecto, ejecutar: dotnet run Program.cs