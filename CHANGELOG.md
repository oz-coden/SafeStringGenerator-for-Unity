# Changelog

## 2.0.0 - 2026-09-14

- Added CHANGELOG.md
- Redesigned generation around deterministic, Unity-independent identifier, literal, and Scene-tree logic.
- Added collision-safe generated APIs under `SafeStringGenerator.Generated`.
- Added an ownership-aware, write-if-different generated file transaction that preserves `.meta` files.
- Added event-driven regeneration with snapshot comparison and no per-frame polling.
- Added Project Settings for the generated files path.
- Reworked all PropertyDrawers to preserve missing and mixed values until explicit user input.
- Added Runtime, Editor, and Editor Test assembly definitions, including generated `.asmref` integration.
- Added Editor tests for identifiers, literals, Scene generation, writer safety, update decisions, and drawer mutations.
- Moved selector attributes into the `SafeStringGenerator` namespace.

## 1.0.2

- Improved generated variable names.

## 1.0.1

- Made the generated file location easier to configure.

## 1.0.0

- Initial release.

