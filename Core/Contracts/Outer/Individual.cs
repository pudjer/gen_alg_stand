namespace Core.Contracts.Outer;

/// <summary>
/// Особь — пара «хромосома + её оценка». Значение без поведения:
/// движок создаёт его после вызова <see cref="IFitnessFunction"/>,
/// операторы и пользователь только читают.
/// </summary>
/// <param name="Chromosome">Решение задачи.</param>
/// <param name="Fitness">Значение целевой функции для этого решения.</param>
public readonly record struct Individual(IChromosome Chromosome, double Fitness);
