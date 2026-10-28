# Web-стенд генетического алгоритма

Razor Pages-приложение: выбираете задачу, задаёте её параметры, собираете комбинацию
алгоритма и модулей (функция потерь, селекция, скрещивание, мутация, начальная
популяция), запускаете расчёт и получаете график лучшего/среднего значения по
поколениям.

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
2. `ProblemFactory<T>.Create` отдаёт декартово произведение осей: алгоритм и пять
   модулей; страница показывает уникальные варианты по каждой оси.
3. `RunConfiguration<T>` (`Catalog/RunConfiguration.cs`) собирает `GeneticProblem<T>`,
   выбранный `IAlgorithm<T>` и `Runtime<T>`.
4. Подписчик на `Core.IObserver` (`SubscribableObserver<T>`) на каждом поколении
   считает `min`/`average` по популяции. Значения `Individ.LossValue` кэшируются,
   поэтому селекция и элитизм переиспользуют их без повторного расчёта.
5. В память попадают только агрегаты (не более ~1500 точек). Шаг прореживания
   вычисляется заранее: `stride = ceil((generations + 1) / 1499)`.
6. После запроса точки сериализуются в JSON и рисуются на `<canvas>` обычным
   `wwwroot/chart.js` — без CDN и без live-обновлений.
7. Дополнительно каждый дескриптор задачи может иметь опциональный
   `IProblemVisualizer<T>` (`Catalog/Visualization.cs`). Он получает лучшую особь и
   уже созданную `problem.LossFunction`; если визуализатора нет — блок не рисуется.

## Алгоритмы

Алгоритм — такая же ось варианта, как модули, и хранится в `Variant<T>.Algorithm`.
Доступны два варианта:

| Алгоритм | Что делает | Параметры |
|---|---|---|
| `Generational` | классическое поколенческое развитие | общие «Поколений» и «Элитных особей» |
| `Island model` | несколько островов, каждый шагает своим `GenerationalAlgorithm`, затем миграция | `islandCount`, `migrationInterval`, `migrantsPerIsland` |

Число поколений и элитизм берутся из общих полей запуска и передаются в фабрику
алгоритма. Для островной модели лимит оценок считается по суммарной популяции
всех островов: observer на нулевом поколении видит уже объединённую популяцию.

## Визуализации

| Тип | Задачи | Что рисуется |
|---|---|---|
| `tsp-map` | TSP | настоящая карта городов и тур лучшей особи |
| `nqueens` | N Queens | доска, конфликтующие ферзи красные |
| `bits` | OneMax, Longest Run, Knapsack | полоса битов и число единиц |
| `heatmap` | float/int, длина ≥ 2 | срез `Loss.Evaluate` по первым двум генам |
| `curve` | float/int, длина = 1 | кривая по одному гену |

У TSP три отдельные функции потерь в одной фабрике: `TSP (random)`, `TSP (circle)`,
`TSP (line)`. Это именно разные наборы городов, а не разные способы отрисовки:
круглые лежат на окружности, линейные — на горизонтальной прямой. Координаты
строит публичный `TspCityGenerator`:

```csharp
IReadOnlyList<TspCity> cities =
    TspCityGenerator.Create(count: 20, seed: 1337, layout: TspLayout.Circle);
double[,] distances = TspCityGenerator.CreateDistances(cities);
```

Генератор детерминирован: одинаковые `count`, `seed` и `layout` дают одинаковые
города. `TspProblemFactory` использует seed `1337 + length`, поэтому раскладки
воспроизводимы. `TspLossFunction.Cities` хранит координаты, по которым
`TspMapVisualizer` строит карту; если задача создана из готовой матрицы расстояний,
координат нет и блок не рисуется.

Остальные визуализации используют только публичные члены: `Genes`, `Length`,
`MinValue`/`MaxValue` и `ILossFunction.Evaluate`.

Сетка heatmap ограничена ~2 млн вызовов `Evaluate` (`side² × длина`), считается
после прогона и не входит в 20-секундный бюджет. Карта TSP рисуется до 300 городов.

## Ограничения стенда

| Ограничение | Значение | Где |
|---|---|---|
| Оценок за прогон (суммарная популяция × поколения) | 2 000 000 | `IndexModel.MaxEvaluations` |
| Размер суммарной популяции | 100 000 | `RunConfiguration.Run` |
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
    Visualization.cs          контракт опционального визуализатора
    Visualizers/              биты, карта TSP/N-Queens, числовой срез
  Pages/
    Index.cshtml(.cs)         форма, график истории и визуализация
  wwwroot/
    site.css, site.js         оформление, переключение параметров модулей
    chart.js                  график истории
    visualize.js              рендер специфичных визуализаций
```

Параметры модулей рендерятся для всех вариантов сразу; имена полей имеют вид
`axis_index_key` (например, `crossover_1_alpha`). Пустое поле означает значение
по умолчанию из фабрики модуля.