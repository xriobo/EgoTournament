# Codificación y Pruebas


## Codificación
En el directorio EgoTournament [acceso directo](https://gitlab.iessanclemente.net/damd/a24xabierrl/-/tree/master/EgoTournament), tengo el código de la aplicación y los tests en EgoTournament.Tests [acceso tests](https://gitlab.iessanclemente.net/damd/a24xabierrl/-/tree/master/EgoTournament.Tests)

## Innovación
EL proyecto lo planteé para que sea multiplataforma: Android y Windows. En la parte del front utilicé .Net Maui, un sdk que usa las vistas de tipo xaml, y c# en la parte de back (modelos, viewModels, etc), ambos con el sdk net 8.0.

Tuve muchas dudas y por ello me ayudó mucho la documentación [Net Maui](https://learn.microsoft.com/es-es/dotnet/maui/?view=net-maui-8.0) de Microsoft acerca de las aplicaciones multiplataforma. Lo más cercano que había usado fue Android Studio y aunque tiene cierto parecido, la implementación cambia mucho.

Con respecto a la autenticación y base de datos, necesitaba que fuera en la nube y en tiempo real, por ello, elegí Firebase Authentication y Firebase Database Realtime. No había trabajado con ninguna de las dos, me resultó muy útil tanto la documentación de [autenticación](https://firebase.google.com/docs/auth?hl=es-419) como la de [database realtime](https://firebase.google.com/docs/database?hl=es-419).

La api de Riot fue necesaria para obtener datos de partidas y clasifiación oficial. En la implementación me guió esta documentación oficial de [Riot](https://developer.riotgames.com/docs/lol). En ella es requisito usar OAuth2 con bearer token como método de autenticación. Permite incluir el token en las peticiones de dos maneras: por cabecera o por parámetro. Para realizar una buena práctica implementé el token en la cabecera.

En la embebida de datos para las pantallas tuve que seguir algún tutorial de youtube como este [tutorial](https://www.youtube.com/watch?v=8biKCeA_gFI). Me sirvió de ayuda para enviar parámetros en las llamadas de una pantalla a otra respetando el modelo MVVM.

Los test fueron implementados y ejecutados con la herramienta XUnit.

La parte mala de usar tecnología tan nueva es que la curva de aprendizaje en algunas cosas fue muy alta pero vale la pena por la facilidad de escalibilidad y mantenimiento.

## Prototipos


### Prototipo 1
En la primera versión de la aplicación completé las siguientes funcionalidades:

- Crear cuenta
- Borrar cuenta
- Añadir información de perfil
- Gestionar los torneos: crear, modificar, visualizar, eliminar.
- Gestionar los participantes (nombres de invocador).
- Gestionar las reglas.
- Visualizar la clasificación de los torneos en tiempo real.
- Visualizar las reglas del torneo seleccionado.

### Prototipo 2
En la siguiente versión se realizaron las siguientes tareas:

- Incluir la búsqueda de la clasificación oficial e información de las últimas 20 partidas con un nombre de invocador como parámetro.
- Arreglos estéticos.
- Correción de posibles bugs de caché y otros servicios de back.

### Prototipo 3
En la última versión hasta el momento:

- Introducir la posibilidad de finalizar un torneo.
- Añadir la opción de ver los resultados finales de un torneo.
- Corrección de bugs de posibles bugs en front.
- Arreglos estéticos.
- Añadida pasarela de pago para evolucionar el rol.

## Pruebas
Además de las pruebas de desarrollo, gracias al emulador de android y la ejecución de la aplicación en windows, se crearon test de integración para comprobar consultas a Firebase Databse Realtime.

Mi pareja me ayudó a corregir y mejorar la estética dándome una visión neutral de la aplicación que para el que lo desarrolla es más complicado de distinguir.

Con estas pruebas pude reconocer mejoras y corregir los posibles bugs, especialmente de front que son más difíciles de encontrar y hacer arreglos estéticos para una mayor claridad de los datos mostrados.

## Funcionalidades

| Acción | Realizada | 
| :--------: | :-------: | 
| Crear cuenta | Si | 
| Borrar cuenta | Si | 
| Iniciar sesión | Si | 
| Cerrar sesión | Si | 
| Modificar perfil | Si | 
| Listar torneos | Si |
| Crear torneo | Si |
| Modificar torneo | Si |
| Eliminar torneo | Si |
| Finalizar torneo | Si |
| Gestionar participantes | Si |
| Gestionar reglas | Si |
| Ver reglas | Si |
| Ver clasificación | Si |
| Ver resultado torneo | Si |
| Buscar invocador | Si  |
| Pasarela Pago | Si  |