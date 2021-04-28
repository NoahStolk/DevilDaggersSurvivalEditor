# Spawnset Binary

## Files

There are 3 files in Devil Daggers which use this format. These are "survival", "dagger", and "menu", although only "survival" makes use of all the features.

- `devildaggers/dd/dagger` contains the dagger lobby.
- `devildaggers/dd/menu` contains the menu.
- `devildaggers/dd/survival` contains the game.
	
## Format

The internal structure of spawnset binaries consists of 5 parts:

| Name | Size in bytes |
|------|---------------|
| Header buffer | 36 |
| Arena buffer | 10404 |
| Spawns header buffer | 36 or 40 depending on version |
| Spawns buffer | 28 x the amount of spawns |
| Settings buffer | 0, 5, or 9 depending on version |

### Overview of known values

- Header buffer
    - Versions
    - Shrinking controls
    - Brightness
- Arena buffer
    - For every tile:
        - Tile height
- Spawns header buffer
    - Spawn count
- Spawns buffer
    - For every spawn:
        - Spawn enemy type
        - Spawn delay
- Settings buffer
    - Initial hand
    - Additional gems
    - Timer start

### Header buffer

Fixed-length buffer of 36 bytes. Contains shrinking control and brightness values as well as presumably a version number and some unknown miscellaneous values that usually cause the game to crash or behave oddly when modified.

The header buffer for the default spawnset looks like this:

| Binary (hex) | Data type | Meaning | Value |
|--------------|-----------|---------|-------|
| `04000000` | 32-bit integer | Spawn version | 4 |
| `09000000` | 32-bit integer | World version | 9 |
| `0000A041` | 32-bit floating point | Shrink end radius | 20 |
| `01004842` | 32-bit floating point | Shrink start radius | 50 |
| `CDCCCC3C` | 32-bit floating point | Shrink rate | 0.025 |
| `00007042` | 32-bit floating point | Brightness | 60 |
| `00000000` | 32-bit integer | Game mode | 0 |
| `33000000` | ? | ? | ? |
| `01000000` | ? | ? | ? |

### Arena buffer

Fixed-length one-dimensional array of 2601 (51 x 51 = 2601 tiles) 32-bit floating point numbers (2601 x 32 / 8 = 10404 bytes) representing the height of each tile in the arena.

### Spawns header buffer

Fixed-length buffer of 40 bytes. Contains the amount of spawns, but mainly unknown values.

The spawns header buffer for the default spawnset looks like this:

| Binary (hex) | Data type | Meaning | Value |
|--------------|-----------|---------|-------|
| `00000000` | ? | ? | ? |
| `00000000` | ? | ? | ? |
| `00000000` | ? | ? | ? |
| `01000000` | ? | ? | ? |
| `F4010000` | 32-bit integer | Devil dagger unlock time | 500 |
| `FA000000` | 32-bit integer | Golden dagger unlock time | 250 |
| `78000000` | 32-bit integer | Silver dagger unlock time | 120 |
| `3C000000` | 32-bit integer | Bronze dagger unlock time | 60 |
| `00000000` | ? | ? | ? |
| `76000000` | 32-bit integer | Spawn count | 118 |

### Spawns buffer

This is the only part of the file with a variable length. It represents the list of spawns. Each spawn buffer consists of 28 bytes that include the enemy type as a 32-bit integer and the delay value as a 32-bit floating point number. The other bytes in each of the spawn buffers seem to be the same for all of them and appear to have no meaning.

These are the first 3 spawns in the original game:

| Binary (hex) | Data type | Meaning | Value |
|--------------|-----------|---------|-------|
| `00000000` | 32-bit integer | Enemy type | 0 |
| `00004040` | 32-bit floating point | Spawn delay | 3 |
| `00000000` | ? | ? | ? |
| `03000000` | ? | ? | ? |
| `00000000` | ? | ? | ? |
| `0000F041` | ? | ? | ? |
| `0A000000` | ? | ? | ? |

| Binary (hex) | Data type | Meaning | Value |
|--------------|-----------|---------|-------|
| `FFFFFFFF` | 32-bit integer | Enemy type | -1 |
| `0000C040` | 32-bit floating point | Spawn delay | 6 |
| `00000000` | ? | ? | ? |
| `03000000` | ? | ? | ? |
| `00000000` | ? | ? | ? |
| `0000F041` | ? | ? | ? |
| `0A000000` | ? | ? | ? |

| Binary (hex) | Data type | Meaning | Value |
|--------------|-----------|---------|-------|
| `00000000` | 32-bit integer | Enemy type | 0 |
| `0000A040` | 32-bit floating point | Spawn delay | 5 |
| `00000000` | ? | ? | ? |
| `03000000` | ? | ? | ? |
| `00000000` | ? | ? | ? |
| `0000F041` | ? | ? | ? |
| `0A000000` | ? | ? | ? |

### Enemy types

Here's the list of enemy types that the survival file defines:

#### V3 / V3.1 (current)

| Binary (hex) | Data type | Meaning | Value |
|--------------|-----------|---------|-------|
| `00000000` | 32-bit integer | Squid I | 0 |
| `01000000` | 32-bit integer | Squid II | 1 |
| `02000000` | 32-bit integer | Centipede | 2 |
| `03000000` | 32-bit integer | Spider I | 3 |
| `04000000` | 32-bit integer | Leviathan | 4 |
| `05000000` | 32-bit integer | Gigapede | 5 |
| `06000000` | 32-bit integer | Squid III | 6 |
| `07000000` | 32-bit integer | Thorn | 7 |
| `08000000` | 32-bit integer | Spider II | 8 |
| `09000000` | 32-bit integer | Ghostpede | 9 |
| `FFFFFFFF` | 32-bit integer | Empty | -1 |

#### V2

| Binary (hex) | Data type | Meaning | Value |
|--------------|-----------|---------|-------|
| `00000000` | 32-bit integer | Squid I | 0 |
| `01000000` | 32-bit integer | Squid II | 1 |
| `02000000` | 32-bit integer | Centipede | 2 |
| `03000000` | 32-bit integer | Spider I | 3 |
| `04000000` | 32-bit integer | Leviathan | 4 |
| `05000000` | 32-bit integer | Gigapede | 5 |
| `06000000` | 32-bit integer | Squid III | 6 |
| `07000000` | 32-bit integer | Andras | 7 |
| `08000000` | 32-bit integer | Spider II | 8 |
| `FFFFFFFF` | 32-bit integer | Empty | -1 |

#### V1

| Binary (hex) | Data type | Meaning | Value |
|--------------|-----------|---------|-------|
| `00000000` | 32-bit integer | Squid I | 0 |
| `01000000` | 32-bit integer | Squid II | 1 |
| `02000000` | 32-bit integer | Centipede | 2 |
| `03000000` | 32-bit integer | Spider I | 3 |
| `04000000` | 32-bit integer | Leviathan | 4 |
| `05000000` | 32-bit integer | Gigapede | 5 |
| `FFFFFFFF` | 32-bit integer | Empty | -1 |

### Settings buffer

Fixed-length buffer of 9 bytes. It was added to the game's V3.1 update which released on February 2021, specifically for spawnset and modding purposes, and is not used in the default spawnset. It only works when the header's spawn version is 5 or 6. The last value, TimerStart, only works on spawn version 6.

| Binary (hex) | Data type | Meaning | Value |
|--------------|-----------|---------|-------|
| `04` | Byte | Initial hand upgrade | 4 |
| `05000000` | 32-bit integer | Additional gems | 5 |
| `0000A041` | 32-bit floating point | Timer start | 20 |
