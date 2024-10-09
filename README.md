
---

# Proyecto ASP.NET con Blazor WASM y Autenticación

Este proyecto es una aplicación web desarrollada con **ASP.NET Core 8**, que utiliza **IdentityFramework** para la autenticación de usuarios y **Blazor WebAssembly** para la interfaz de usuario una vez que el usuario ha iniciado sesión.

![image](https://github.com/user-attachments/assets/d6aac8e6-0886-4c29-b91f-16f1f1f99eb4)

![image](https://github.com/user-attachments/assets/c9768d17-7a27-4c17-91fe-b4604181a75d)

![image](https://github.com/user-attachments/assets/7cd83933-c4f5-4495-b4de-8e63351886b3)

![image](https://github.com/user-attachments/assets/36d64101-3ff5-41e1-bf84-72ca089f6aa1)


## Tecnologías Utilizadas

- **.NET 8**
- **ASP.NET Core**
- **IdentityFramework** para la autenticación
- **Razor Pages** para las vistas de inicio de sesión, registro y recuperación de contraseña
- **Blazor WebAssembly (Blazor WASM)** como frontend
- **API externa** para gestionar la lógica de negocio (Placeholder para el repositorio de la API)

## Características del Proyecto

1. **Autenticación y Autorización**:
   - Utiliza **IdentityFramework** para el registro, inicio de sesión, gestión de roles y recuperación de contraseñas.
   - Soporte para autenticación con email y contraseñas.
   
2. **Razor Pages**:
   - Se implementan las páginas de **Inicio de sesión**, **Registro** y **Recuperación de contraseña** usando Razor Pages.
   
3. **Aplicación Blazor WebAssembly**:
   - Una vez autenticado, el usuario es redirigido a una aplicación **Blazor WASM**, que interactúa con una API externa para obtener datos y ejecutar operaciones.
   
4. **API Externa**:
   - El proyecto se conecta a una **API** que gestiona la lógica del negocio.
   - La API se encuentra en un repositorio separado. Reemplazar el siguiente enlace con la ubicación del repositorio de la API:
     ```
     https://github.com/Charl-pena/CESistemaLogin.ServerApp
     ```

## Configuración del Proyecto

### Requisitos Previos

Asegúrate de tener las siguientes herramientas instaladas en tu entorno de desarrollo:

- [.NET SDK 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) con el **workload ASP.NET y desarrollo web**
- [Node.js](https://nodejs.org/en/) (opcional, si se utiliza alguna funcionalidad adicional del lado del cliente)

### Clonar el Repositorio

Primero, clona este repositorio en tu máquina local:

```bash
git clone https://github.com/Charl-pena/CESistemaLogin.ServerApp.git
cd tu-repositorio
```

### Configuración del Proyecto

#### Configurar la API

En el archivo `appsettings.json`, deberás configurar la URL base de la API:

```json
"ApiSettings": {
  "BaseUrl": "https://url-placeholder-de-la-api.com/"
}
```

#### Migraciones de la Base de Datos

Para habilitar la autenticación y la gestión de usuarios, asegúrate de aplicar las migraciones para IdentityFramework. Desde la terminal, ejecuta:

```bash
dotnet ef database update
```

> Nota: Si no tienes instalado `Entity Framework`, ejecuta primero:
> 
> ```bash
> dotnet tool install --global dotnet-ef
> ```

### Ejecutar el Proyecto

Para ejecutar el proyecto localmente, utiliza el siguiente comando:

```bash
dotnet run
```

Accede a la aplicación en `https://localhost:7191`.

---
