# Які файли створюються

На кожен запуск створюється окрема папка сесії:

```text
Application.persistentDataPath/
└── DiagnosticsSessions/
    ├── latest-session-path.txt
    └── session-20260318-142211-a1b2c3/
        ├── session.json
        ├── events.jsonl
        └── summary.txt
```

## 1. `latest-session-path.txt`

Швидкий покажчик на останню сесію.  
Корисно, коли треба швидко знайти потрібну папку.

## 2. `session.json`

Метадані сесії:

- sessionId
- Unity version
- platform
- buildType
- startedUtc
- endedUtc
- повний шлях до папки і файлів

Це зручно для розуміння, в якому середовищі був зібраний лог.

## 3. `events.jsonl`

Головний файл для аналізу.

Формат JSONL означає:
- один JSON-об’єкт на один рядок
- легко читати руками
- легко парсити скриптами
- легко надсилати мені для аналізу

Кожен рядок містить:
- час
- кадр
- scene
- channel
- severity
- eventName
- message
- dataJson
- stackTrace
- повний snapshot героя на момент події

### Приклад рядка

```json
{"sessionId":"abc","scene":"MainScene","frame":381,"channel":"Ladder","severity":"Info","eventName":"ClimbStarted","message":"Player entered climb mode.","dataJson":"{\"touchingLadder\":true}","hero":{"state":"Climb","position":{"x":12.0,"y":8.0},"velocity":{"x":0.0,"y":2.4},"input":{"x":0.0,"y":1.0},"grounded":false,"touchingLadder":true,"climbing":true,"recentlyGrounded":true,"recentlyTouchedLadder":true,"topExitLocked":false,"climbStartLocked":true,"gravityScale":0.0}}
```

## 4. `summary.txt`

Людський короткий звіт:

- загальна інформація по сесії
- останній snapshot героя
- кількість подій по типах
- коротка підказка, що саме надсилати

## Що саме надсилати мені

Найкращий варіант:
- всю папку `session-...` у zip
- опис кроків відтворення бага
- 1–2 скріни інспектора героя / драбини, якщо є

Цього вже достатньо для нормального розбору більшості проблем руху і climb-логіки.
