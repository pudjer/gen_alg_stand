# Web-стенд генетического алгоритма

Razor Pages-приложение: выбираете задачу, задаёте её параметры, собираете комбинацию
модулей (функция потерь, селекция, скрещивание, мутация, начальная популяция),
запускаете расчёт и получаете график лучшего/среднего значения по поколениям.

## Запуск

```bash
dotnet run --project Web/Web.csproj
```

Откройте `http://localhost:5000`.

Для запуска нужен ASP.NET Core Runtime 10. В Arch/CachyOS:

```bash
sudo pacman -S aspnet-runtime
```

Если ставить рантайм не хочется, можно собрать self-contained копию:

```bash
dotnet publish Web/Web.csproj -c Release -r linux-x64 --self-contained true -o /tmp/ga-web
/tmp/ga-web/Web --urls http://127.0.0.1:5080
```

## Как это работает

1. `ProblemCatalog` хранит все задачи из `Implementations.Problems`.
2. `ProblemFactory<T>.Create` отдаёт декартово произведение модулей; страница
   показывает уникальные варианты по каждой оси.
3. `RunConfiguration<T>` (`Catalog/RunConfiguration.cs`) собирает `GeneticProblem<T>` и
   `Runtime<T>`.
4. Подписчик на `Core.IObserver` (`SubscribableObserver<T>`) на каждом поколении
   считает `min`/`average` по популяции. Значения `Individ.LossValue` кэшируются,
   поэтому селекция и элитизм переиспользуют их без повторного расчёта.
5. В память попадают только агрегаты (не более ~1500 точек). Шаг прореживания
   вычисляется заранее: `stride = ceil((generations + 1) / 1499)`.
6. После запроса точки сериализуются в JSON и рисуются на `<canvas>` обычным
   `wwwroot/chart.js` — без CDN и без live-обновлений.

## Ограничения стенда

| Ограничение | Значение | Где |
|---|---|---|
| Оценок за прогон (популяция × поколения) | 2 000 000 | `IndexModel.MaxEvaluations` |
| Размер популяции | 100 000 | `RunConfiguration.Run` |
| Поколений | 500 000 | `IndexModel.OnPostRun` |
| Время расчёта | 20 с | `IndexModel.Budget` |
| Точек на графике | ~1500 | `IndexModel.MaxPoints` |

Превышение лимита оценок отклоняется до расчёта с понятным сообщением. Таймаут
останавливает прогон из observer'а и показывает уже собранные поколения.

## Структура

```
Web/
  Program.cs                  точка входа, content root, DI
  Catalog/
    ProblemCatalog.cs         список задач и форматирование лучшей особи
    ProblemDescriptor.cs      обёртка IProblemFactory<T> без generic в UI
    RunModels.cs              ModuleOption / RunRequest / RunResult
    RunConfiguration.cs       сборка проблемы, observer, лимиты, прогон
  Pages/
    Index.cshtml(.cs)         форма и график
  wwwroot/
    site.css, site.js         оформление, переключение параметров модулей
    chart.js                  отрисовка графика на canvas
```

Параметры модулей рендерятся для всех вариантов сразу; имена полей имеют вид
`axis_index_key` (например, `crossover_1_alpha`). Пустое поле означает значение
по умолчанию из фабрики модуля.