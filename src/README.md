# ReadAllMessages — мод для Schedule I (MelonLoader)

Добавляет кнопку **Read All** в приложение **Messages** на игровом телефоне.
Кнопка помечает все диалоги как прочитанные — и больше ничего не делает.
Дополнительно: клавиша **F8** делает то же самое (запасной вариант).

## Как это работает
- Мод ждёт загрузки игровой сцены (`Main` / `Tutorial`) и появления `MessagesApp`.
- Создаёт кнопку в правом нижнем углу домашней страницы Messages.
- По клику проходит по `MessagesApp.Conversations` и вызывает `MSGConversation.SetRead(true)`
  для всех непрочитанных диалогов, затем обновляет счётчик уведомлений
  (`RefreshNotifications`). Это убирает точки «непрочитано» и цифру на иконке приложения.

## Требования
1. **Schedule I** (Steam).
2. **MelonLoader 0.6+** (лучше 0.7.x), установленный в игру.
3. **.NET SDK 6.0 или новее** для сборки: https://dotnet.microsoft.com/download
4. Запусти игру с MelonLoader хотя бы один раз до главного меню —
   MelonLoader сгенерирует прокси-сборки в `Schedule I\MelonLoader\Il2CppAssemblies\`.
   Без этого проект не соберётся.

## Сборка
1. Открой `ReadAllMessages/ReadAllMessages.csproj` и проверь строку `<GamePath>` —
   путь к папке игры (по умолчанию `C:\Program Files (x86)\Steam\steamapps\common\Schedule I`).
2. В консоли из папки `ReadAllMessages` (где лежит `.csproj`):

   ```
   dotnet build -c Il2Cpp
   ```

   Это для **обычной ветки** Steam (по умолчанию). Если играешь на ветке
   `alternate` (Mono) — собирай так:

   ```
   dotnet build -c Mono
   ```

3. Если в `GamePath` есть папка `Mods`, готовый `ReadAllMessages.dll`
   скопируется туда автоматически. Иначе возьми его из `bin\Il2Cpp\` (или `bin\Mono\`)
   и положи в `Schedule I\Mods\` вручную.

## Проверка
1. Запусти игру, загрузи сейв.
2. В консоли MelonLoader должно появиться: `Кнопка Read All добавлена в Messages.`
3. Открой телефон → Messages → справа внизу синяя кнопка **Read All**.
4. Нажми — все непрочитанные диалоги станут прочитанными, в консоли будет
   `Прочитано диалогов: N`.

## Возможные проблемы
- **Проект не собирается, нет Il2CppAssemblies** — запусти игру с MelonLoader
  один раз и дождись главного меню.
- **Сборки называются иначе** (другая версия MelonLoader): открой
  `Schedule I\MelonLoader\` и проверь, что есть `net6\MelonLoader.dll` и
  `Il2CppAssemblies\Assembly-CSharp.dll`. Если пути отличаются — поправь их в `.csproj`.
- **Кнопка не появилась** — посмотри консоль MelonLoader на ошибки от
  `ReadAllMessages` и пришли лог (`Schedule I\MelonLoader\Latest.log`).
  F8 при этом всё равно должен работать.
- **Кнопка перекрывает другой элемент UI** — позицию легко поменять в `Core.cs`
  (метод `CreateButton`, строки с `anchorMin/anchorMax/anchoredPosition`).

## Замечания
- Статус «прочитано» в игре локальный и не синхронизируется по сети —
  в мультиплеере мод безопасен.
- S1API не используется: для одной кнопки прямое обращение к игровым классам
  проще и без лишней зависимости. Если захочешь — легко портировать.
