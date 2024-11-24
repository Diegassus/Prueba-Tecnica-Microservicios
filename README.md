# Prueba Tecnica

Este proyecto es una prueba tecnica sobre microservicios. Se opto por utilizar *.Net Core v8.0.300* con *Entity Framework Core v9.0.0*, con una base de datos en memoria de EF *(Entity Framework Core InMemory v9.0.0)*.

El sistema esta planteado para la gestion de personas y peliculas. Estos son microservicios para permitir la escalabilidad del proyecto.

## Descripcion del sistema

El sistema se compone de 2 Microservicios y un Gateway.

El primero microservicio "Peliculas" cuenta con los siguientes endpoints

- GetMovies (Recibe una lista de ids y devuelve las peliculas correspondientes a los elementos).
- ValidateMovie (Recibe un id y retorna la pelicula correspondiente).
- CargarPeliculas (Este endpoint hardcodea la base de datos de Peliculas con 2 registros. Esto es para facilitar el uso de la solucion).

El segundo microservicio "Personas" cuenta con los siguientes endpoints

- GetPersonas (Devuelve un listado de personas ordenado por apellido y nombre).
- GetPersonaByName (Recibe un parametro el cual se evalua si es un id o el nombre de una persona, devolviendo el correspondiente registro).
- CreatePersona (Recibe una persona y genera el registro en la base de datos).
- UpdatePersona (Recibe los datos de la persona a actualizar y los datos a cambiar).
- DeletePersona (Recibe el id de la persona y la elimina de la base de datos).
- AddMovie (Añade una pelicula al registro de la persona correspondiente del id que recibe por parametro).
- DeleteMovie (Elimina la pelicula recibida de la lista de la persona correspondiente al id recibido).

El Gateway, contiene las URLs a las cuales comunicarse para ser redirijido a los microservicios con Ocelot

## Como usar los endpoints

A continuacion, se detallan los endpoints a utilizar y como enviar los datos correspondientes. Se aconseja respetar la primer solicitud para tener peliculas cargadas.

Revisar el archivo Ocelot.json para comprender las rutas y los redireccionamientos.

*Cargar peliculas*
![imagen](https://github.com/user-attachments/assets/acf4028d-fb4e-42e1-aa94-cb24755af4dd)

*Crear una persona*
![imagen](https://github.com/user-attachments/assets/17507c9d-5901-4625-843e-2442ee83470b)

*Update de la persona*
![imagen](https://github.com/user-attachments/assets/841c6323-7486-4095-8124-9fba3797995b)

*Listado de personas*
![imagen](https://github.com/user-attachments/assets/b24a29b5-7636-4ca3-b285-5eebbb5e8175)

*Obtener persona por id*
![imagen](https://github.com/user-attachments/assets/d45c469a-2f28-44a3-aef2-b7278f87836d)

*Obtener persona por nombre*
![imagen](https://github.com/user-attachments/assets/5b4f45aa-2eb2-4c87-85ed-d42224f8c354)

*Eliminar persona*
![imagen](https://github.com/user-attachments/assets/b0030f40-41e7-4122-b432-5b1f4dd727dc)

*Añadir pelicula a la lista de la persona*
![imagen](https://github.com/user-attachments/assets/b1ef0308-9766-4584-81dc-9d124e49f125)

*Eliminar pelicula de la lista de la persona*
![imagen](https://github.com/user-attachments/assets/b6b69ba7-7752-4005-9a6f-a2a27e289fae)






