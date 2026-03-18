# Diagnostics V2

Це нова база логування, яка **не зав'язана на Hero-скрипти**.

Вона тримається на трьох простих рівнях:

1. `DiagManager` — створює сесію, пише `session.json`, `events.jsonl`, `summary.txt`
2. `DiagUnityLogRelay` — забирає звичайні `Debug.Log/Warning/Error`
3. довільні `DiagMarker` / `DiagTransformSnapshot` / `DiagAutoSnapshot` — додаються на будь-які об'єкти без жорсткої залежності від конкретних gameplay-скриптів

## Що створюється

За замовчуванням у:

`E:\Logs\Miner\session-YYYYMMDD-HHMMSS-xxxxxx\`

створюються:

- `session.json`
- `events.jsonl`
- `summary.txt`

Також у корені створюється:

- `latest-session-path.txt`

## Принцип роботи

Система більше не знає нічого про Hero, Ladder, GridMotor або конкретні поля проекту.

Вона вміє лише:
- відкрити сесію
- приймати події
- зберігати події в JSONL
- збирати довільний контекст із компонентів, що реалізують `IDiagContextProvider`
- збирати довільні snapshots із компонентів, що реалізують `IDiagSnapshotProvider`

## Мінімальна інтеграція

### У сцені
Створи GameObject `Diagnostics` і повісь на нього:
- `DiagManager`
- `DiagUnityLogRelay`

### На будь-який важливий об'єкт
Наприклад на нового Hero:
- `DiagMarker`
- `DiagTransformSnapshot`
- `DiagAutoSnapshot`
- за бажанням `DiagGameObjectLifecycle`

Цього вже вистачить, щоб у логах були:
- створення/вмикання об'єкта
- позиція
- scale
- Rigidbody2D state
- Collider2D bounds
- звичайні Unity logs

## Як логувати вручну з будь-якого нового скрипта

```csharp
Diag.Event("Player", "SpawnResolved", "Hero spawned on ground.", this,
    ("spawnCell", spawnCell),
    ("position", transform.position));
```

```csharp
Diag.Warning("Collision", "BlockedBySolid", null, this,
    ("cell", cell),
    ("solidType", solidType));
```

```csharp
Diag.Error("Player", "InvalidState", "Unexpected locomotion state.", this,
    ("state", state));
```

## Чому ця версія краща

Стара система була сильно зав'язана на конкретні поля героя, драбини та locomotion-стани.

Нова система:
- не вимагає жодного Hero-специфічного компонента
- переживе повний rewrite player-системи
- дає тобі однаковий формат логів для будь-яких майбутніх систем
- легко розширюється через інтерфейси

## Що надсилати мені для аналізу

- zip усієї папки `session-*`
- короткі кроки відтворення
- за потреби скріни Inspector
