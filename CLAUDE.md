# CLAUDE.md - Spyke Extras Package

## What This Does
Optional utilities package providing common game features like visual effects, icon management, image loading, I/O helpers, and animation utilities.

## Package Structure

```
upm-spyke-extras/
├── Runtime/
│   ├── Effects/       ← Visual effects (FloatingText, Shiny, Sparkle)
│   ├── Icons/         ← Icon management and sprite loading
│   ├── ImageRepo/     ← Remote image loading with caching
│   ├── IO/            ← File I/O helpers, serialization
│   ├── Animation/     ← Animation helpers, tweening extensions
│   ├── Panels/        ← Reusable UI panel patterns
│   └── Spyke.Extras.asmdef
├── Editor/
│   └── Spyke.Extras.Editor.asmdef
├── Tests/
│   ├── Runtime/
│   └── Editor/
├── package.json
└── CLAUDE.md
```

## Key Modules

| Module | Purpose | Status |
|--------|---------|--------|
| `Runtime/Effects/` | Visual effects (floating text, pulse, shiny) | ✅ Done |
| `Runtime/Icons/` | Icon sprite management | ✅ Done |
| `Runtime/ImageRepo/` | Remote image loading with cache | 🚧 TODO |
| `Runtime/IO/` | File I/O helpers | 🚧 TODO |
| `Runtime/Animation/` | Animation utilities | 🚧 TODO |
| `Runtime/Panels/` | Reusable panel components | 🚧 TODO |

## How to Use

### Installation
```json
// Packages/manifest.json
{
  "dependencies": {
    "com.spykegames.extras": "https://github.com/spykegames/upm-spyke-extras.git#v0.1.0"
  }
}
```

### Basic Usage
```csharp
using Spyke.Extras.Effects;
using Spyke.Extras.Icons;
using Spyke.Extras.ImageRepo;

// Effects - Floating text
[Inject] private readonly IFloatingTextService _floatingText;
_floatingText.Show("+100", position, Color.yellow);

// Icons - Load sprite
[Inject] private readonly IIconService _icons;
var sprite = await _icons.GetIconAsync("coin");

// ImageRepo - Remote images
[Inject] private readonly IImageRepository _imageRepo;
var texture = await _imageRepo.LoadAsync(url);
```

## Dependencies
- com.spykegames.core (required)
- com.spykegames.ui (required)

## Depends On This
- Game-specific projects (optional dependency)

## Source Files to Port

From `client-bootstrap`:
| Source | Destination |
|--------|-------------|
| `SpykeLib/.../Effects/` | `Runtime/Effects/` |
| `Common/UI/Icons/` | `Runtime/Icons/` |
| `SpykeLib/.../ImageRepo/` | `Runtime/ImageRepo/` |
| `SpykeLib/.../IO/` | `Runtime/IO/` |
| `Common/Animation/` | `Runtime/Animation/` |

## Status
🚧 **IN DEVELOPMENT** - Package structure created, modules pending

### Completed
- ✅ Package structure created
- ✅ Assembly definitions configured
- ✅ CLAUDE.md documentation
- ✅ Effects module (FloatingText, ShinyEffect, PulseEffect)
- ✅ Icons module (IIconService, IconConfig, addressables support)

### Planned Modules
- 🚧 ImageRepo (remote image loading, caching)
- 🚧 IO (file helpers, JSON serialization)
- 🚧 Animation (tween extensions, sequence helpers)
