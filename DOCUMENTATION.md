# Files

There are 3 files in Devil Daggers which use this format. These are "survival", "dagger", and "menu", although only "survival" makes use of all the features.

- devildaggers/dd/dagger contains the dagger lobby.
- devildaggers/dd/menu contains the menu.
- devildaggers/dd/survival contains the game.
	
# Format

The internal structure of spawnset binaries consists of 4 parts:

- Header buffer (36 bytes)
- Arena buffer (10404 bytes)
- Spawns header buffer (40 bytes)
- Spawns buffer (28 bytes x the amount of spawns)

## Overview of known values

- Header buffer
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

## Header buffer

Fixed-length buffer of 36 bytes. Contains shrinking control and brightness values as well as presumably a version number and some unknown miscellaneous values that usually cause the game to crash or behave oddly when modified.

The header buffer for the default spawnset looks like this:

| Binary | Data type | Meaning | Value |
|--------|-----------|---------|---------|
| `04000000` | ? | ? | ? |
| `09000000` | ? | ? | ? |
| `0000A041` | 32-bit floating point | Shrink end radius | 20 |
| `01004842` | 32-bit floating point | Shrink start radius | 50 |
| `CDCCCC3C` | 32-bit floating point | Shrink rate | 0.025 |
| `00007042` | 32-bit floating point | Brightness | 60 |
| `00000000` | ? | ? | ? |
| `33000000` | ? | ? | ? |
| `01000000` | ? | ? | ? |

## Arena buffer

Fixed-length one-dimensional array of 2601 (51 x 51 = 2601 tiles) 32-bit floating point numbers (2601 x 32 / 8 = 10404 bytes) representing the height of each tile in the arena.

## Spawns header buffer

Fixed-length buffer of 40 bytes. Contains the amount of spawns, but mainly unknown values.

The spawns header buffer for the default spawnset looks like this:

| Binary | Data type | Meaning | Value |
|--------|-----------|---------|---------|
| `00000000` | ? | ? | ? |
| `00000000` | ? | ? | ? |
| `00000000` | ? | ? | ? |
| `01000000` | ? | ? | ? |
| `F4010000` | ? | ? | ? |
| `FA000000` | ? | ? | ? |
| `78000000` | ? | ? | ? |
| `3C000000` | ? | ? | ? |
| `00000000` | ? | ? | ? |
| `76000000` | 32-bit unsigned integer | Spawn count | 118 |

## Spawns buffer

This is the only part of the file with a variable length. It represents the list of spawns. Each spawn buffer consists of 28 bytes that include the enemy type as a 32-bit integer and the delay value as a 32-bit floating point number. The other bytes in each of the spawn buffers seem to be the same for all of them and appear to have no meaning.

These are the first 3 spawns in the original game:

| Binary | Data type | Meaning | Value |
|--------|-----------|---------|---------|
| `00000000` | 32-bit signed integer | Enemy type | 0 |
| `00004040` | 32-bit floating point | Spawn delay | 3 |
| `00000000` | ? | ? | ? |
| `03000000` | ? | ? | ? |
| `00000000` | ? | ? | ? |
| `0000F041` | ? | ? | ? |
| `0A000000` | ? | ? | ? |

| Binary | Data type | Meaning | Value |
|--------|-----------|---------|---------|
| `FFFFFFFF` | 32-bit signed integer | Enemy type | -1 |
| `0000C040` | 32-bit floating point | Spawn delay | 6 |
| `00000000` | ? | ? | ? |
| `03000000` | ? | ? | ? |
| `00000000` | ? | ? | ? |
| `0000F041` | ? | ? | ? |
| `0A000000` | ? | ? | ? |

| Binary | Data type | Meaning | Value |
|--------|-----------|---------|---------|
| `00000000` | 32-bit signed integer | Enemy type | 0 |
| `0000A040` | 32-bit floating point | Spawn delay | 5 |
| `00000000` | ? | ? | ? |
| `03000000` | ? | ? | ? |
| `00000000` | ? | ? | ? |
| `0000F041` | ? | ? | ? |
| `0A000000` | ? | ? | ? |

Here's the list of enemy types that the survival file defines:

| Binary | Data type | Meaning | Value |
|--------|-----------|---------|---------|
| `00000000` | 32-bit signed integer | Squid I | 0 |
| `01000000` | 32-bit signed integer | Squid II | 1 |
| `02000000` | 32-bit signed integer | Centipede | 2 |
| `03000000` | 32-bit signed integer | Spider I | 3 |
| `04000000` | 32-bit signed integer | Leviathan | 4 |
| `05000000` | 32-bit signed integer | Gigapede | 5 |
| `06000000` | 32-bit signed integer | Squid III | 6 |
| `07000000` | 32-bit signed integer | Thorn | 7 |
| `08000000` | 32-bit signed integer | Spider II | 8 |
| `09000000` | 32-bit signed integer | Ghostpede | 9 |
| `FFFFFFFF` | 32-bit signed integer | Empty | -1 |
