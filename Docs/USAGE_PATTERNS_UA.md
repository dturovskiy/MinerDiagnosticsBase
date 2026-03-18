# Як використовувати в коді

## 1. Snapshot героя

Snapshot треба оновлювати часто, але не обов’язково писати в файл кожен кадр.

Тому логіка така:
- `GameDiag.SetHeroSnapshot(snapshot)` — можна хоч кожен `LateUpdate`
- `GameDiag.Event(...)` — тільки коли сталося щось важливе

## 2. Що логувати для руху і драбини

Рекомендовані події:

- `StateChanged`
- `GroundedChanged`
- `LadderContactChanged`
- `ClimbEnterAttempt`
- `ClimbStarted`
- `ClimbStopped`
- `TopExitLockStarted`
- `TopExitLockEnded`
- `UnexpectedLadderExit`
- `VelocityClamped`
- `JumpPressed`
- `JumpConsumed`
- `InputChanged`

## 3. Де саме викликати

### У state controller
Коли змінюється стан:

```csharp
GameDiag.Event(
    DiagChannel.State,
    "StateChanged",
    $"{oldState} -> {newState}");
```

### У ladder detector
Коли змінився факт контакту з драбиною:

```csharp
GameDiag.Event(
    DiagChannel.Ladder,
    "LadderContactChanged",
    $"touching={touchingLadderNow}");
```

### У hero motor
Коли реально стартував climb:

```csharp
GameDiag.Event(
    DiagChannel.Motor,
    "ClimbStarted",
    "Motor switched to climb movement.");
```

## 4. Як передавати контекст причини

Через serializable-об’єкт:

```csharp
[System.Serializable]
public class ClimbExitReasonData
{
    public bool grounded;
    public bool ladderLost;
    public bool topExitLocked;
    public string reason;
}
```

```csharp
GameDiag.Warning(
    DiagChannel.Ladder,
    "ClimbStopped",
    "Leaving climb state.",
    new ClimbExitReasonData
    {
        grounded = groundedNow,
        ladderLost = !touchingLadderNow,
        topExitLocked = topExitLocked,
        reason = "Ladder contact lost"
    });
```

## 5. Що не треба робити

Не треба:
- писати `Debug.Log` щокадру
- логувати кожну зміну позиції в файл
- дублювати одну й ту саму подію кілька разів в різних місцях
- писати величезні stack trace для звичайних info-подій

## 6. Мінімальний набір для твого кейсу

Для проблем з драбиною я б спочатку підключив тільки це:

- snapshot героя
- `StateChanged`
- `GroundedChanged`
- `LadderContactChanged`
- `ClimbEnterAttempt`
- `ClimbStarted`
- `ClimbStopped`

Цього вже достатньо, щоб побачити:
- чи герой справді торкнувся драбини
- чи вважався grounded
- чи був дозвіл на старт climb
- коли саме пропав контакт або змінився стан
