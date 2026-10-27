# Algorithms — architecture specification

## Quick start

```csharp
// TSP over 8 cities with a generational GA and elitism.
var template = PermutationChromosome.Identity(8);
var loss = new TspLossFunction<PermutationChromosome>(distanceMatrix);

var problem = new GeneticProblem<PermutationChromosome>
{
    LossFunction = loss,
    Selection = new TournamentSelection<PermutationChromosome>(loss, 4),
    Crossover = new OrderCrossover<PermutationChromosome>(1.0),
    Mutation = new SwapMutation<PermutationChromosome>(0.3),
};

var observer = new BestChromosomeObserver<PermutationChromosome>(loss);

var input = new Input<PermutationChromosome>
{
    Problem = problem,
    InitialPopulation = new RandomPermutationPopulation<PermutationChromosome>(50, template),
    Observer = observer,
    AlgorithmFactory = new IslandAlgorithmFactory<PermutationChromosome>(
        new IslandOptions { IslandCount = 4, MigrationInterval = 10, MigrantsPerIsland = 1 },
        new GenerationalOptions { MaxGenerations = 80, EliteCount = 1 }),
};

new GenAlgCycle<PermutationChromosome> { Random = myRandom }.Run(input);
Console.WriteLine(observer.BestLoss);
```

`GenAlgCycle` (in `Core`) calls `IAlgorithm<T>.Iterate` until it returns `null`; the initial
population and every subsequent generation are reported to the observer.

Target project: `Algorithms/Algorithms.csproj` (`net10.0`, `ImplicitUsings`, `Nullable` enabled).
It references only `Core`. **No NuGet packages.** BCL only.

Core contracts (`namespace Core.Contracts`, read-only, do not change):

```csharp
IChromosome<TSelf>   { TSelf Clone(); }                                  // TSelf : IChromosome<TSelf>
IAlgorithm<T>        { IReadOnlyList<T>? Iterate(IReadOnlyList<T> population, IRandom random); }
ISelection<T>        { IReadOnlyList<T> Select(IReadOnlyList<T> population, IRandom random); }
ICrossover<T>        { IReadOnlyList<T> Crossover(IReadOnlyList<T> population, IRandom random); }
IMutation<T>         { IReadOnlyList<T> Mutate(IReadOnlyList<T> population, IRandom random); }
IInitialPopulation<T>{ IReadOnlyList<T> Create(IRandom random); }
ILossFunction<T>     { double Evaluate(T chromosome); }
IObserver<T>         { void OnGeneration(IReadOnlyList<T> population); }
IAlgorithmFactory<T> { IAlgorithm<T> Create(GeneticProblem<T> problem); }
IRandom              { double NextDouble(); int Next(int minInclusive, int maxExclusive); }
GeneticProblem<T>    { ILossFunction<T> LossFunction; ISelection<T> Selection; ICrossover<T> Crossover; IMutation<T> Mutation; }
```

`IAlgorithm<T>.Iterate` returns `null` to signal "finished"; `GenAlgCycle` then stops.

## Global conventions

- **Minimization**: smaller loss is better, everywhere.
- File-scoped namespaces, `sealed` concrete classes, abstract base classes unsealed.
- Guard public arguments with `ArgumentNullException.ThrowIfNull` / `ArgumentOutOfRangeException.ThrowIf*`.
- XML `///` summary on every public type and public member (one or two lines is enough).
- No mutable shared state except where explicitly described (stagnation criterion, algorithm generation counters, observers).
- Chromosomes are treated as immutable values: never mutate a chromosome or the array returned by `Genes`.
- Do not edit `Core`, the `.csproj`, nor files owned by other tasks.
- Delete `Algorithms/Class1.cs`.

### Folder → namespace map

| Folder | Namespace |
| --- | --- |
| `Chromosomes/` | `Algorithms.Chromosomes` |
| `Chromosomes/Crossovers/` | `Algorithms.Crossovers` |
| `Chromosomes/Mutations/` | `Algorithms.Mutations` |
| `Chromosomes/Selections/` | `Algorithms.Selections` |
| `Chromosomes/InitialPopulations/` | `Algorithms.InitialPopulations` |
| `Chromosomes/LossFunctions/` | `Algorithms.LossFunctions` |
| `Chromosomes/Observers/` | `Algorithms.Observers` |
| `Orchestration/` | `Algorithms.Orchestration` |
| `Factories/` | `Algorithms.Factories` |

---

## 1. Chromosome hierarchy (`Chromosomes/`)

### 1.1 Interfaces

`IGenotypeChromosome.cs`:

```csharp
namespace Algorithms.Chromosomes;

/// <summary>A chromosome represented as a fixed-length sequence of genes.</summary>
public interface IGenotypeChromosome<TSelf, TGene> : IChromosome<TSelf>
    where TSelf : IGenotypeChromosome<TSelf, TGene>
{
    /// <summary>Number of genes.</summary>
    int Length { get; }

    /// <summary>Gene at the given position.</summary>
    TGene this[int index] { get; }

    /// <summary>Read-only view over the genes; never mutated.</summary>
    IReadOnlyList<TGene> Genes { get; }

    /// <summary>Creates a chromosome with the same parameters and the supplied genes (copied).</summary>
    TSelf WithGenes(IReadOnlyList<TGene> genes);
}
```

`INumericChromosome.cs`:

```csharp
namespace Algorithms.Chromosomes;

/// <summary>A genotype whose genes are numbers constrained to [MinValue, MaxValue].</summary>
public interface INumericChromosome<TSelf, TNumber> : IGenotypeChromosome<TSelf, TNumber>
    where TSelf : INumericChromosome<TSelf, TNumber>
    where TNumber : System.Numerics.INumber<TNumber>
{
    TNumber MinValue { get; }
    TNumber MaxValue { get; }
}

public interface IIntChromosome<TSelf> : INumericChromosome<TSelf, int>
    where TSelf : IIntChromosome<TSelf>;

public interface IFloatChromosome<TSelf> : INumericChromosome<TSelf, double>
    where TSelf : IFloatChromosome<TSelf>;
```

`IBinaryChromosome.cs`:

```csharp
namespace Algorithms.Chromosomes;

public interface IBinaryChromosome<TSelf> : IGenotypeChromosome<TSelf, bool>
    where TSelf : IBinaryChromosome<TSelf>;
```

`IPermutationChromosome.cs`:

```csharp
namespace Algorithms.Chromosomes;

/// <summary>Genotype whose integer genes are distinct; order carries meaning (scheduling, routing).</summary>
public interface IOrderedChromosome<TSelf> : IGenotypeChromosome<TSelf, int>
    where TSelf : IOrderedChromosome<TSelf>;

/// <summary>Ordered chromosome whose genes are a permutation of 0..Length-1 (TSP, N-Queens).</summary>
public interface IPermutationChromosome<TSelf> : IOrderedChromosome<TSelf>
    where TSelf : IPermutationChromosome<TSelf>;
```

### 1.2 Concrete chromosomes

All constructors copy the incoming sequence into an internal array and validate invariants.
`WithGenes` validates again and returns a new instance. `Clone` returns an independent copy.

| File | Type / constructor |
| --- | --- |
| `IntChromosome.cs` | `sealed class IntChromosome : IIntChromosome<IntChromosome>`, `ctor(IReadOnlyList<int> genes, int minValue, int maxValue)` |
| `FloatChromosome.cs` | `sealed class FloatChromosome : IFloatChromosome<FloatChromosome>`, `ctor(IReadOnlyList<double> genes, double minValue, double maxValue)` |
| `BinaryChromosome.cs` | `sealed class BinaryChromosome : IBinaryChromosome<BinaryChromosome>`, `ctor(IReadOnlyList<bool> genes)` |
| `PermutationChromosome.cs` | `sealed class PermutationChromosome : IPermutationChromosome<PermutationChromosome>`, `ctor(IReadOnlyList<int> genes)`; also `static PermutationChromosome Identity(int length)` |

Invariants (throw `ArgumentException` / `ArgumentOutOfRangeException`):
- non-empty gene list;
- `minValue <= maxValue`;
- every numeric gene in `[minValue, maxValue]` (float: also reject NaN/Infinity);
- permutation genes contain each value `0..Length-1` exactly once.
- `Identity(n)` returns `0,1,...,n-1`.

---

## 2. Crossovers (`Chromosomes/Crossovers/`)

`CrossoverBase.cs`:

```csharp
namespace Algorithms.Crossovers;

/// <summary>Pairs adjacent chromosomes and produces one offspring pair per pair.</summary>
public abstract class CrossoverBase<TSelf> : ICrossover<TSelf>
    where TSelf : IChromosome<TSelf>
{
    protected CrossoverBase(double crossoverProbability = 1.0); // validate 0..1

    public IReadOnlyList<TSelf> Crossover(IReadOnlyList<TSelf> population, IRandom random);
    // null-guard args; empty population -> empty list.
    // for i = 0,2,4...: if no partner -> add population[i].Clone(); break.
    // with probability p -> CrossPair(a, b, random), else (a.Clone(), b.Clone()).

    protected abstract (TSelf First, TSelf Second) CrossPair(TSelf first, TSelf second, IRandom random);
}
```

| File | Type | Constraint / behaviour |
| --- | --- | --- |
| `SinglePointCrossover.cs` | `sealed SinglePointCrossover<TSelf, TGene> : CrossoverBase<TSelf>` | `TSelf : IGenotypeChromosome<TSelf, TGene>`; cut in `[1, Length-1]`; if `Length < 2` clone both |
| `TwoPointCrossover.cs` | `sealed TwoPointCrossover<TSelf, TGene>` | same constraint; two ordered distinct cuts; swap middle segment |
| `UniformCrossover.cs` | `sealed UniformCrossover<TSelf, TGene>` | same constraint; ctor `(double swapProbability = 0.5, double crossoverProbability = 1.0)`; per gene swap with probability |
| `ArithmeticCrossover.cs` | `sealed ArithmeticCrossover<TSelf>` | `TSelf : IFloatChromosome<TSelf>`; ctor `(double weight = 0.5, double crossoverProbability = 1.0)`; child1 = w*a+(1-w)*b, child2 = w*b+(1-w)*a, clamp to bounds; result via `WithGenes` |
| `BlendCrossover.cs` | `sealed BlendCrossover<TSelf>` | `TSelf : IFloatChromosome<TSelf>`; ctor `(double alpha = 0.5, double crossoverProbability = 1.0)`; per gene beta uniform in `[-alpha, 1+alpha]`, child = a + beta*(b-a), clamp |
| `IntegerArithmeticCrossover.cs` | `sealed IntegerArithmeticCrossover<TSelf>` | `TSelf : IIntChromosome<TSelf>`; ctor `(double weight = 0.5, double crossoverProbability = 1.0)`; round to nearest and clamp |
| `OrderCrossover.cs` | `sealed OrderCrossover<TSelf>` | `TSelf : IOrderedChromosome<TSelf>`; classic OX |
| `PartiallyMappedCrossover.cs` | `sealed PartiallyMappedCrossover<TSelf>` | `TSelf : IOrderedChromosome<TSelf>`; classic PMX |
| `CycleCrossover.cs` | `sealed CycleCrossover<TSelf>` | `TSelf : IOrderedChromosome<TSelf>`; classic CX; both parents must share the same gene set (documented); must not loop forever |

All permutation crossovers build `int[]` children and return `parent.WithGenes(child)`.

---

## 3. Mutations (`Chromosomes/Mutations/`)

`MutationBase.cs`:

```csharp
namespace Algorithms.Mutations;

public abstract class MutationBase<TSelf> : IMutation<TSelf>
    where TSelf : IChromosome<TSelf>
{
    protected MutationBase(double chromosomeProbability = 0.1); // validate 0..1

    public IReadOnlyList<TSelf> Mutate(IReadOnlyList<TSelf> population, IRandom random);
    // per chromosome: probability -> MutateChromosome, else Clone(); preserve order and count.

    protected abstract TSelf MutateChromosome(TSelf chromosome, IRandom random);
}
```

| File | Type | Constraint / behaviour |
| --- | --- | --- |
| `BitFlipMutation.cs` | `sealed BitFlipMutation<TSelf>` | `TSelf : IBinaryChromosome<TSelf>`; ctor `(double geneProbability = 0.01, double chromosomeProbability = 0.1)`; flips each gene independently |
| `UniformIntMutation.cs` | `sealed UniformIntMutation<TSelf>` | `TSelf : IIntChromosome<TSelf>`; `(double geneProbability = 0.01, double chromosomeProbability = 0.1)`; resample in `[MinValue, MaxValue]` inclusive |
| `UniformFloatMutation.cs` | `sealed UniformFloatMutation<TSelf>` | `TSelf : IFloatChromosome<TSelf>`; same ctor; resample in `[MinValue, MaxValue]` |
| `GaussianMutation.cs` | `sealed GaussianMutation<TSelf>` | `TSelf : IFloatChromosome<TSelf>`; `(double geneProbability = 0.01, double sigma = 0.1, double chromosomeProbability = 0.1)`; std = `sigma * (MaxValue - MinValue)`, clamp; Box–Muller |
| `PermutationMutationBase.cs` | `abstract PermutationMutationBase<TSelf> : MutationBase<TSelf>` | `TSelf : IOrderedChromosome<TSelf>`; ctor `(double chromosomeProbability = 0.1)`; copies genes to `int[]`, calls `MutateOrder`, returns `WithGenes` |
| `SwapMutation.cs` | `sealed SwapMutation<TSelf> : PermutationMutationBase<TSelf>` | swap two distinct random positions; no-op when `Length < 2` |
| `InversionMutation.cs` | `sealed InversionMutation<TSelf>` | reverse a random non-empty segment |
| `ScrambleMutation.cs` | `sealed ScrambleMutation<TSelf>` | shuffle a random segment |
| `InsertMutation.cs` | `sealed InsertMutation<TSelf>` | remove one gene and reinsert at another position |

`MutateOrder` signature: `protected abstract void MutateOrder(int[] genes, IRandom random);`.

---

## 4. Selections (`Chromosomes/Selections/`)

Every implementation needs fitness, so it takes `ILossFunction<TChromosome>` in the constructor.
`Select` returns **exactly `population.Count`** individuals (sampling with replacement is allowed), preserving the contract that the population size is stable.

| File | Type / ctor | Behaviour |
| --- | --- | --- |
| `TournamentSelection.cs` | `sealed TournamentSelection<TChromosome>(ILossFunction<TChromosome> lossFunction, int tournamentSize = 3)` | each pick = best of `tournamentSize` uniformly random individuals |
| `RouletteWheelSelection.cs` | `sealed RouletteWheelSelection<TChromosome>(ILossFunction<TChromosome> lossFunction)` | weight `1 / (1 + max(0, loss))`; fall back to uniform when total is 0/NaN/Infinity |
| `RankSelection.cs` | `sealed RankSelection<TChromosome>(ILossFunction<TChromosome> lossFunction)` | sort ascending by loss; weight of rank `r` is `Count - r` (best = `Count`) |
| `TruncationSelection.cs` | `sealed TruncationSelection<TChromosome>(ILossFunction<TChromosome> lossFunction, double survivalRate = 0.5)` | keep top `max(1, ceil(Count*rate))`, fill result by random picks from survivors |
| `RandomSelection.cs` | `sealed RandomSelection<TChromosome>()` | uniform sampling with replacement |

Empty population → return empty list (do not throw) except where impossible; guard invalid `tournamentSize < 1` and `survivalRate` outside `(0, 1]`.

---

## 5. Initial populations (`Chromosomes/InitialPopulations/`)

Every implementation takes a template chromosome; the template supplies length, bounds and construction via `WithGenes`, so no extra factories are needed.

| File | Type / ctor |
| --- | --- |
| `RandomIntPopulation.cs` | `sealed RandomIntPopulation<TChromosome>(int size, TChromosome template)` — `TChromosome : IIntChromosome<TChromosome>`; uniform int in `[MinValue, MaxValue]` |
| `RandomFloatPopulation.cs` | `sealed RandomFloatPopulation<TChromosome>(int size, TChromosome template)` — `IFloatChromosome`; uniform double in `[MinValue, MaxValue]` |
| `RandomBinaryPopulation.cs` | `sealed RandomBinaryPopulation<TChromosome>(int size, TChromosome template, double trueProbability = 0.5)` — `IBinaryChromosome`; validate probability |
| `RandomPermutationPopulation.cs` | `sealed RandomPermutationPopulation<TChromosome>(int size, TChromosome template)` — `IPermutationChromosome`; Fisher–Yates shuffle of `0..Length-1` |

Validate `size >= 1`, non-null template.

---

## 6. Loss functions (`Chromosomes/LossFunctions/`)

All minimize.

| File | Type | Behaviour |
| --- | --- | --- |
| `OneMaxLossFunction.cs` | `sealed OneMaxLossFunction<TChromosome> : ILossFunction<TChromosome>`, `TChromosome : IBinaryChromosome<TChromosome>` | `-(number of true genes)` |
| `TargetVectorLossFunction.cs` | `sealed TargetVectorLossFunction<TChromosome>`, `TChromosome : IFloatChromosome<TChromosome>`, `ctor(IReadOnlyList<double> target)` | mean squared error; throws if length mismatch |
| `SphereLossFunction.cs` | `sealed SphereLossFunction<TChromosome>`, `IFloatChromosome` | `sum(x_i^2)` |
| `RosenbrockLossFunction.cs` | `sealed RosenbrockLossFunction<TChromosome>`, `IFloatChromosome` | `sum(100*(x_{i+1}-x_i^2)^2 + (1-x_i)^2)`; `0` when `Length < 2` |
| `RastriginLossFunction.cs` | `sealed RastriginLossFunction<TChromosome>`, `IFloatChromosome` | `10n + sum(x^2 - 10*cos(2*pi*x))` |
| `SumTargetLossFunction.cs` | `sealed SumTargetLossFunction<TChromosome>`, `TChromosome : IIntChromosome<TChromosome>`, `ctor(long target)` | `abs(sum(genes) - target)` |
| `NQueensLossFunction.cs` | `sealed NQueensLossFunction<TChromosome>`, `IPermutationChromosome` | number of attacking queen pairs; genes index = column, value = row |
| `TspLossFunction.cs` | `sealed TspLossFunction<TChromosome>`, `IPermutationChromosome`, `ctor(double[,] distanceMatrix)` | cost of cycle `gene[0] -> ... -> gene[n-1] -> gene[0]`; throw if matrix size ≠ length; throw on negative distances |

---

## 7. Observers (`Chromosomes/Observers/`)

| File | Type |
| --- | --- |
| `NullObserver.cs` | `sealed NullObserver<TChromosome>` with private ctor and `public static NullObserver<TChromosome> Instance { get; }` |
| `CompositeObserver.cs` | `sealed CompositeObserver<TChromosome>(params IObserver<TChromosome>[] observers)`; forwards to all |
| `ConsoleObserver.cs` | `sealed ConsoleObserver<TChromosome>(ILossFunction<TChromosome> lossFunction, string? label = null)`; prints generation, best, mean per call |
| `BestChromosomeObserver.cs` | `sealed BestChromosomeObserver<TChromosome>(ILossFunction<TChromosome> lossFunction)`; exposes `TChromosome? Best`, `double BestLoss` (init `+Infinity`), `int Generation` |
| `HistoryObserver.cs` | `sealed HistoryObserver<TChromosome>(ILossFunction<TChromosome> lossFunction)`; exposes `IReadOnlyList<double> BestLosses`, `IReadOnlyList<double> MeanLosses` |

All compute `lossFunction.Evaluate` per individual. Empty population → no-op / unchanged state.

---

## 8. Orchestration (`Orchestration/`)

Algorithms do not invent their own operator hierarchies. They own **only the schedule**: which of
the four operators of a `GeneticProblem<TChromosome>` is called, in what order, and when to stop.
`LossFunction` is a first-class scheduling input — it ranks individuals for elitism and migration.

### 8.1 Options

```csharp
public sealed record GenerationalOptions
{
    public static GenerationalOptions Default { get; } = new();
    public int MaxGenerations { get; init; } = 1000;
    public int EliteCount { get; init; } = 1;              // 0 disables elitism
    public bool MaintainPopulationSize { get; init; } = true;
}

public sealed record IslandOptions
{
    public static IslandOptions Default { get; } = new();
    public int IslandCount { get; init; } = 4;
    public int MigrationInterval { get; init; } = 10;      // 0 disables migration
    public int MigrantsPerIsland { get; init; } = 1;
    public Func<int, IRandom>? RandomFactory { get; init; } = null;  // null => sequential
    public int MaxDegreeOfParallelism { get; init; } = -1;           // -1 => ProcessorCount
}
```

### 8.1.1 Parallel islands

Islands are the natural unit of parallelism: they share no mutable state, and the only shared input
is the random source. `IRandom` carries no thread-safety guarantee, so the algorithm never shares one
instance between workers.

- `RandomFactory == null` (default) → **sequential**; all islands share the `IRandom` passed to
  `Iterate`. Existing behaviour is unchanged.
- `RandomFactory != null` and `IslandCount > 1` → every island runs on the thread pool with its own
  `IRandom`. Seeds are drawn from the shared source **one at a time, before the workers start**, so a
  seeded run stays reproducible while the workers run concurrently.
- `MaxDegreeOfParallelism`: `-1` = `Environment.ProcessorCount`, `0`/`1` = force sequential,
  positive = cap the worker count.

Requirements and caveats:

- Problem operators (`Selection`, `Crossover`, `Mutation`, `LossFunction`) must be stateless or safe
  for concurrent calls. All operators shipped here satisfy this.
- Parallel and sequential runs are **not** numerically identical — islands consume different random
  streams. Each mode is individually reproducible for a fixed seed.
- Migration always runs on the calling thread after every island finishes the step, so it cannot
  cascade within one round.
- Prefer this when the loss function is expensive. With a very cheap loss and few small islands,
  `Parallel.For` overhead can outweigh the gain; measure before enabling.

### 8.2 `GenerationalAlgorithm<TChromosome>`

```csharp
public sealed class GenerationalAlgorithm<TChromosome> : IAlgorithm<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    public GenerationalAlgorithm(GeneticProblem<TChromosome> problem, GenerationalOptions? options = null);

    public IReadOnlyList<TChromosome>? Iterate(IReadOnlyList<TChromosome> population, IRandom random);
}
```

`Iterate`:

1. `generation >= MaxGenerations` or empty population → `null`.
2. `selected  = problem.Selection.Select(population, random)`
3. `children  = problem.Crossover.Crossover(selected, random)`
4. `mutated   = problem.Mutation.Mutate(children, random)`
5. Elitism (skip when `EliteCount == 0`): rank `population` and `mutated` with
   `problem.LossFunction`; overwrite the `EliteCount` worst offspring slots with the best parents
   (elites are reused by reference — chromosomes are immutable).
6. If `MaintainPopulationSize` and the count changed, truncate or pad by cloning uniformly random
   individuals.
7. `generation++`; return the next generation.

### 8.3 `IslandModelAlgorithm<TChromosome>`

```csharp
public sealed class IslandModelAlgorithm<TChromosome> : IAlgorithm<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    public IslandModelAlgorithm(
        GeneticProblem<TChromosome> problem,
        Func<GeneticProblem<TChromosome>, IAlgorithm<TChromosome>> islandFactory,
        IslandOptions? options = null);

    public IReadOnlyList<TChromosome>? Iterate(IReadOnlyList<TChromosome> population, IRandom random);
}
```

- First call splits the incoming population into `min(IslandCount, population.Count)` contiguous,
  non-empty islands and creates one inner algorithm per island **from the same problem instance**.
- Later calls ignore the incoming population and continue internal state.
- Each step runs one `Iterate` on every unfinished island; `null` marks an island finished.
- Every `MigrationInterval` steps (when `> 0`): island `i` sends its `MigrantsPerIsland` best
  individuals (ranked by `problem.LossFunction`) to island `(i + 1) % n`, replacing that island's
  worst individuals. All emigrants are chosen before any island is written, so migration cannot
  cascade within a single round.
- Returns the flattened population of all islands after each step.
- Returns `null` on the call **after** all islands finished (the final population is still observed
  once). Empty initial population → `null`.

---

## 9. Factories (`Factories/`)

```csharp
public sealed class DelegatingAlgorithmFactory<TChromosome> : IAlgorithmFactory<TChromosome>
{
    public DelegatingAlgorithmFactory(Func<GeneticProblem<TChromosome>, IAlgorithm<TChromosome>> factory);
}

public sealed class GenerationalAlgorithmFactory<TChromosome> : IAlgorithmFactory<TChromosome>
{
    public GenerationalAlgorithmFactory(GenerationalOptions? options = null);
}

public sealed class IslandAlgorithmFactory<TChromosome> : IAlgorithmFactory<TChromosome>
{
    public IslandAlgorithmFactory(
        IslandOptions? options = null,
        GenerationalOptions? islandGenerationalOptions = null);
}
```

Factories hold configuration only; every operator is taken from the `GeneticProblem` passed to
`Create`. `IslandAlgorithmFactory` hands the same problem to each island.

---

## 10. Validation

- `dotnet build Algorithms/Algorithms.csproj` must succeed with zero warnings introduced by new code.
- Verified scenarios: OneMax (32/32 bits), TSP (converges), N-Queens (0 attacks), Sphere (→ 0),
  integer sum-target (exact).
- No file outside the assigned task may be touched.
