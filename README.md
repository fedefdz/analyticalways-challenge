# analyticalways-acme

## Overview

Este proyecto es un challenge para Analyticalways, consiste en una `Class Library` de aplicación para ACME School.

## Project Structure

La solución está dividida en las siguientes capas y proyectos:

```
acme/ 
├── src/ 
│   ├── Application/
│   │   ├── UseCases/
│   │   │   ├── RegisterCourse/
│   │   │   ├── RegisterStudent/
│   │   │   ├── ContracCourse/
│   │   │   ├── PayRegistrationFeeCourse/
│   │   │   ├── EnrollStudentInCourse/
│   │   │   └── ListCourses/
│   │   └── Services/
│   │      └── PaymentGateway/
│   │          └── Strategies/
│   │               ├── BankTransfer/
│   │               ├── CreditCard/
│   │               └── DebitCard/
│   └── Domain/
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   ├── Exceptions/
│   │   └── Repositories/
└── test/ 
    └── UnitTest/
        ├── Application/
        └── Domain/        
```
- **AcmeSchool.Application**: Contiene la lógica de aplicación, incluyendo casos de uso, servicios y cualquier otra lógica que coordine las acciones entre la capa de presentación y la capa de dominio.
  - Contract course se divide en 3 use case: *ContractCourseUseCase*, *PayRegistrationFeeCourseUseCase* y *EnrollStudentInCourseUseCase*
  - Payment gateway se considera un servicio de aplicación porque el domnio solo necesita saber del pago de la tasa no de los gateways. Se definen varias strategies de formas de pago pero la interface principal `IPaymentGatewayService` es simple a alto nivel y la que debe usar el use case.

- **AcmeSchool.Domain**: Contiene las entidades del dominio, excepciones específicas del dominio y las interfaces de los repositorios. Esta capa define las reglas de negocio y los objetos utilizados a lo largo de la aplicación.

- **AcmeSchool.UnitTests**: Contiene las pruebas unitarias para los proyectos de la solución, asegurando que la lógica de negocio y los casos de uso funcionen como se espera.

## Testing

La solución utiliza xUnit para las pruebas unitarias, Moq para mockear dependencias y AutoFixture para generar datos de prueba. Las pruebas se organizan siguiendo la estructura del proyecto que prueban, y se centran en validar la lógica de negocio y los casos de uso definidos en la capa de aplicación.

### Running Tests

Para ejecutar las pruebas, puedes utilizar el comando `dotnet test` en la raíz del proyecto de pruebas.

## Consideraciones
No se uso MeditR para la capa de Application para no agregar dependencias.

Hay dos interface donde itervienen payments, en el Domain la `IPaymentRepository` que gestiona los los pagos de tasas abstrayendo información de los Gateways ya que eso es de la capa de Application. La interface `IPaymentGatewayService` brinda la posibilidad de gestionar datos necesarion para el pago por Gateways y la posibilidad de escalar con diferentes forma de pago mediante el record abstract `PaymentRequest`.


Las class `PaymentGatewayStrategySelector` y `PaymentGatewayStrategyDynamicSelector` permiten escalar con diferentes formas de pago pero no es obligación utilizarlas cuando se implemente `IPaymentGatewayService`