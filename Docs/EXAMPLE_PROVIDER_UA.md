# Example provider for future HeroV2

```csharp
using System.Collections.Generic;
using UnityEngine;

public sealed class HeroV2DiagSnapshotProvider : MonoBehaviour, IDiagSnapshotProvider
{
    [SerializeField] private HeroMotor2D motor;

    public void AppendSnapshot(List<DiagField> fields)
    {
        if (motor == null) return;

        fields.Add(new DiagField("grounded", motor.IsGrounded.ToString()));
        fields.Add(new DiagField("falling", motor.IsFalling.ToString()));
        fields.Add(new DiagField("position", transform.position.ToString()));
    }
}
```

Ідея така:
- логер нічого не знає про `HeroMotor2D`
- provider знає про `HeroMotor2D`
- заміниш мотор — просто заміниш provider
