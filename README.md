# Gestion des équipements et maintenance

Mini-projet POO en C# : suivi des interventions, planning des techniciens et coûts.

## Structure
- `Enums/` : statuts, priorités, spécialités
- `Exceptions/` : exceptions métier
- `Repositories/` : `IEntite`, `IRepository<T>`, `InMemoryRepository<T>`
- `Models/` : classes du domaine (Equipement, Technicien, Intervention, ...)
- `Services/` : `ServiceMaintenance`
- `Program.cs` : scénario de démonstration

## Exécution
```
dotnet run
```
Prérequis : SDK .NET 8 (ou modifier `TargetFramework` dans le `.csproj`, minimum .NET 6).
