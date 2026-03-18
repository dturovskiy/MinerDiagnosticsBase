# Setup

## 1. Замінити стару систему

Видали або вимкни старі компоненти логування:
- `GameDiagManager`
- `GameDiagAutoSnapshot`
- старі hero-specific snapshot providers

## 2. Додати нову систему

У проект скопіюй папку:

`Assets/Scripts/DiagnosticsV2/Runtime`

## 3. У сцені створити об'єкт Diagnostics

Додай компоненти:
- `DiagManager`
- `DiagUnityLogRelay`

## 4. На Hero або інші важливі об'єкти

Додай:
- `DiagMarker`
- `DiagTransformSnapshot`
- `DiagAutoSnapshot`

## 5. Root path

У `DiagManager` поле `Root Folder Path` постав:

`E:\Logs\Miner`

## 6. Provider pattern

Коли з'явиться новий HeroV2, зроби окремий `HeroV2DiagSnapshotProvider : MonoBehaviour, IDiagSnapshotProvider`.

Тоді `DiagAutoSnapshot` автоматично почне включати дані нового героя без зміни менеджера логів.
