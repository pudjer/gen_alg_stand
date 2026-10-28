# Architecture

A research bench for genetic algorithms. The user picks a problem, picks the operators,
tunes their parameters, runs the algorithm and looks at the convergence history.

## Layers

Dependencies point **downward only**. A project may reference the projects below it and
nothing else.

```
Web        (Blazor WASM, presentation only)
  |
Catalog    (what exists: problems + operator descriptors + generic dispatch)
  |
Studio     (experiment model and interfaces: config, result, runner, store)
  |
Parameters (parameter metadata: specs, validation, typed access)
  |
Algorithms (operators, chromosomes, orchestration)
  |
Core       (contracts only, frozen)
```

| Project | May reference | Owns |
|---|---|---|
| `Core` | — | `IChromosome`, `ICrossover`, `IMutation`, `ISelection`, `ILossFunction`, `IInitialPopulation`, `IObserver`, `IRandom`, `IAlgorithm`, `GeneticProblem<T>` |
| `Algorithms` | `Core` | Concrete chromosomes, operators, loss functions, initial populations, observers, `GenerationalAlgorithm`, `IslandModelAlgorithm`, `SeededRandom` |
| `Parameters` | — | `ParameterSpec`, `ParameterKind`, `ParameterSet` — metadata and validation, no knowledge of algorithms |
| `Studio` | `Parameters` | `ExperimentConfig`, `GenerationStats`, `ExperimentResult`, `StoredExperiment`, `IExperimentRunner`, `IExperimentStore` — the model, no algorithms |
| `Catalog` | `Studio`, `Parameters`, `Algorithms`, `Core` | Descriptors that map a string key + parameter values to a concrete operator instance, and the generic dispatch that builds a runner |
| `Web` | `Catalog`, `Studio`, `Parameters` | Razor pages, JS interop, charts |

`Web` must not reference `Algorithms` directly. Everything it needs comes through `Catalog`
and `Studio`. This keeps the UI free of generic type parameters.

`Studio` deliberately does **not** reference `Catalog`: the model must not know what operators
exist. The dependency is inverted — `Catalog` implements `IExperimentRunner` for a concrete
chromosome type, and `Web` wires the two together.

## The one dispatch point

Operators are generic over the chromosome type, the UI is not. The bridge is
`IProblemEntry` in `Catalog`: it exposes a non-generic view (keys, display names,
parameter specs) and a `CreateRunner(config)` method. `ProblemEntry<TChromosome>` implements
it by closing the generic over a concrete chromosome type.

**This is the only place in the application where a type parameter is resolved at runtime.**
Everything above `Catalog` works with strings and `ParameterSet`.

## Naming

- Descriptor keys are `"<category>.<name>"` in lower case: `"crossover.singlepoint"`,
  `"mutation.bitflip"`, `"loss.onemax"`, `"problem.onemax"`, `"algorithm.island"`.
- Parameter keys are lower camel case: `"crossoverProbability"`, `"tournamentSize"`.
- Every parameter has a `Default` that is valid by construction — `ParameterSpec` validates it.

## Rules

1. **No `object` casts outside `Catalog`.** If a cast is needed higher up, the descriptor is wrong.
2. **No `Algorithms` types in `Web`.** If a page needs a chromosome, it gets `ChromosomeView`.
3. **`Core` is frozen.** Changes there need a real reason; `Algorithms` is free to change.
4. **Determinism is a feature.** Same `ExperimentConfig` (including `Seed`) must produce the
   same `ExperimentResult`. Any change that breaks this is a bug.
5. **No hyper-parameterization.** A knob exists only if a user would actually turn it.
   Three similar classes are better than one class with a strategy parameter.

## Data flow of a run

```
ExperimentConfig (strings)
  -> IProblemEntry.CreateRunner(config)
  -> ProblemEntry<T>.CreateRunner: resolve descriptors, build operators, build GeneticProblem<T>
  -> IExperimentRunner.Run(progress, ct)
  -> StatisticsObserver<T> collects per-generation stats
  -> ExperimentResult (serializable)
  -> IExperimentStore.SaveAsync
```

## File ownership

Each folder is written by exactly one author at a time. Do not edit files outside your
assigned folder; if a change is needed elsewhere, report it instead.
