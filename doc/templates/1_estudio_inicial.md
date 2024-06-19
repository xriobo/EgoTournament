# ANTEPROYECTO

## Descripción

Mi proyecto tratará de una aplicación multiplataforma para Windows y Android. Gestionará y monitorizará torneos del juego League of Legends.

### Justificación y Funcionalidades

Vi la necesidad cuando intenté competir con mis amigos en el juego League of Legends y no existía la posibilidad de crear un torneo reuniéndonos a todos, ni encontramos ninguna manera simple de ver nuestra puntuación y clasificación.

EL diseño de esta aplicación intentará mostrar los torneos con un aspecto amigable para el usuario. Contendrá un buscador que, dado un nombre de invocador (nombre de usuario en el juego), mostrará el historial de las últimas partidas jugadas en League Of Legends.

Desde la aplicación se podrá gestionar la autenticación de usuario, permitiendo así su creación, modificación y borrado.

En los torneos exitirán las siguientes funcionalidades: creación, borrado, modificación, ver la clasificación en tiempo real y ver los resultados de los torneos ya finalizados gracias a un histórico.

Incluirá una visualización de las reglas del torneo seleccionado.

En la búsqueda por invocador mencionada anteriormente se verán sus últimas 20 partidas con detalles como el resultado de la partida, estadísticas y campeón seleccionado.

Reuniendo estas características los usuarios podrán organizar sus propios torneos con los participantes que ellos deseen de manera autónoma. Los participantes podrán tener una cuenta con o sin clasificación oficial del League of Legend.

### Estudio de necesidades
Explicar divisiones.
Existen webs y aplicaciones que muestran los datos en tiempo real de un jugador único o de la partida en la que está participando como [OPGG](https://www.op.gg/). No encontré ninguna posibilidad de seleccionar, por el RiotId, usuarios de divisiones diferentes o con posibilidad de jugar Ranks pero sin division para mostrar una competición entre ellos.

### Posibles receptores

El público objetivo de mi aplicación son los jugadores que quieren realizar torneos con sus amigos al margen de la competición oficial del juego y poder hacer un seguimiento de dicho torneo. Además de los participantes en el torneo, el resto de usuarios podrán acceder a su historial de partidas (las 20 últimas) o de cualquier otro usuario del League of Legends, aumentando así el número de destinatarios de la aplicación. 

### Publicidad

Se promocionará a través de las redes sociales con publicaciones en la futura página oficial de la aplicación y con anuncios publicitarios en las mismas. Además, dada la tipología de la aplicación el boca a boca será una parte fundamental en su divulgación. 

### Modelo de negocio
Será una aplicación freemium con tres tipos de usuarios diferentes.

Según su pago tendrán acceso a más o menos funcionalidades. Siendo el básico gratuíto y el premium el más caro.

Además de esto se incluirán anuncios en la aplicación.

## Requisitos

El desarrollo de esta aplicación será en lenguaje c#. Usaré el IDE Visual Studio con el sdk Net 8 y su framework Net Maui con vistas xaml para desarrollo de aplicaciones multiplataforma.

La base de datos será de tipo Firebase Database Realtime y la autenticación Firebase Authentication.

La aplicación podrá ser descargada desde play store o microsoft store.

Se necesitarán tres fuentes de datos, la base de datos de autenticación, la base de datos propia de los datos de la aplicación y una api pública Riot, la empresa de League of Legends en la que se usará una autenticación OAuth2 por Bearer token.

## Planificación

| Fase | Inicio | Final | Descripción |
| :--------: | :-------: | :-------: | :--------: |
| Anteproyecto | 08/04 | 12/04 | Se expondrá la idea del proyecto con las tecnologías, funcionalidades y necesidades a cubrir.
| Análisis, Diseño e Implementación | 12/04 | 12/06 | Se fijarán funcionalidades, diagrama de base de datos, mockup de frontend y se empezará a codificar. |