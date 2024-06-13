# analyticalways-acme

## Overview

Este proyecto es un challenge para Analyticalways, consiste en una `Class Library` de aplicación para ACME School.

## Project Structure

La solución está dividida en las siguientes capas y proyectos:

- **AcmeSchool.Application**: Contiene la lógica de aplicación, incluyendo casos de uso, servicios y cualquier otra lógica que coordine las acciones entre la capa de presentación y la capa de dominio.

- **AcmeSchool.Domain**: Contiene las entidades del dominio, excepciones específicas del dominio y las interfaces de los repositorios. Esta capa define las reglas de negocio y los objetos utilizados a lo largo de la aplicación.

- **AcmeSchool.UnitTests**: Contiene las pruebas unitarias para los proyectos de la solución, asegurando que la lógica de negocio y los casos de uso funcionen como se espera.

## Testing

La solución utiliza xUnit para las pruebas unitarias, Moq para mockear dependencias y AutoFixture para generar datos de prueba. Las pruebas se organizan siguiendo la estructura del proyecto que prueban, y se centran en validar la lógica de negocio y los casos de uso definidos en la capa de aplicación.

### Running Tests

Para ejecutar las pruebas, puedes utilizar el comando `dotnet test` en la raíz del proyecto de pruebas.