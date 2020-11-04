

OpenTelemetry sample app with 3 aspnet core services that call each other via HttpClient.
All 3 services are instrumented via the `OpenTelemetry.Instrumentation.AspNetCore` and `OpenTelemetry.Instrumentation.Http` (for `HttpClient`) instrumentation. 
It uses Jaeger exporter at port `6835`


```
To test A->B->C run

curl https://localhost:44397/api/ResourceA

To Test A -> B -> C
         \-> C  

curl https://localhost:44397/api/ResourceA/1

```
