using Core.Contracts.Inner;

namespace Core.Contracts.Outer;

/// <summary>
/// Критерий останова задачи — единственный, который пишет пользователь.
/// <para>
/// Отмена и лимит поколений проверяются движком самостоятельно,
/// дублировать их здесь не нужно.
/// </para>
/// </summary>
public interface ITermination
{
    /// <summary>
    /// Вызывается на границе поколения, до создания следующего.
    /// <c>true</c> — завершить прогон.
    /// </summary>
    bool ShouldStop(IExecutionContext context);
}
