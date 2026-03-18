# Швидкий старт

## 1. Скопіюй папку

Скопіюй папку `Assets/Scripts/Diagnostics` у свій Unity-проект.

## 2. Запусти проект

Після старту в Editor або Development Build система сама створить manager і HUD.

## 3. Мінімальне підключення

Щоб система вже щось збирала, додай на героя:

- `BasicRigidbody2DSnapshotProvider`
- `GameDiagAutoSnapshot`

Це дасть:
- position
- velocity
- gravityScale
- state label (ручний рядок)

## 4. Нормальне підключення під героя

Зроби свій adapter-компонент, який реалізує `IGameDiagSnapshotProvider`.

Приклад ідеї:

```csharp
using Miner.Diagnostics;
using UnityEngine;

public class HeroDiagSnapshotProvider : MonoBehaviour, IGameDiagSnapshotProvider
{
    [SerializeField] private Rigidbody2D rb;

    // Тут підтягни свої посилання на hero state / input / ladder detector

    public HeroDiagSnapshot BuildSnapshot()
    {
        return new HeroDiagSnapshot
        {
            state = "Walk",
            scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
            position = transform.position,
            velocity = rb != null ? rb.linearVelocity : Vector2.zero,
            input = new Vector2(0f, 0f),
            grounded = false,
            touchingLadder = false,
            climbing = false,
            recentlyGrounded = false,
            recentlyTouchedLadder = false,
            topExitLocked = false,
            climbStartLocked = false,
            gravityScale = rb != null ? rb.gravityScale : 0f
        };
    }
}
```

Потім:
- додай цей компонент на героя
- додай `GameDiagAutoSnapshot`
- в `providerSource` вкажи свій provider

## 5. Логуй тільки важливі переходи

Приклади:

```csharp
GameDiag.Event(DiagChannel.State, "StateChanged", $"Idle -> Walk");
GameDiag.Event(DiagChannel.Ladder, "ClimbStarted", "Player entered climb mode.");
GameDiag.Warning(DiagChannel.Ladder, "UnexpectedLadderExit", "Climb state was dropped unexpectedly.");
GameDiag.Error(DiagChannel.Motor, "VelocityNaN", "Detected invalid rigidbody velocity.");
```

## 6. Додай причину в data

Коли потрібно більше деталей, передавай serializable-object:

```csharp
[System.Serializable]
public class LadderEnterData
{
    public bool grounded;
    public bool touchingLadder;
    public bool recentlyGrounded;
    public bool topExitLocked;
}
```

```csharp
GameDiag.Event(
    DiagChannel.Ladder,
    "ClimbEnterAttempt",
    "Trying to enter climb mode.",
    new LadderEnterData
    {
        grounded = groundedNow,
        touchingLadder = touchingLadderNow,
        recentlyGrounded = recentlyGrounded,
        topExitLocked = topExitLocked
    });
```

## 7. Де шукати файли

Файли сесії лежать у:

`Application.persistentDataPath/DiagnosticsSessions/session-...`

Точний шлях система також записує в:

`Application.persistentDataPath/DiagnosticsSessions/latest-session-path.txt`
