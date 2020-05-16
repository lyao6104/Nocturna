# Nocturna Project Changelog

## April 23, 2020

- Initial commit of Unity project
  - Lots of other stuff before this but this is where we're starting from.

## May 12, 2020

- Changes to Weather System
  - Added more weather types
    - ClearHot
    - ClearCold
    - Snowy
    - Dry
    - Wet
    - Droughtlike
  - Changed how Weather is determined
    - Rather than being purely random, weather is chosen based on season and what the previous day's weather was like.
    - Almost definitely needs some balancing.
  - Made it so that Weather text can be different from how it's named in the code
    - E.g. ClearHot and ClearCold both get rendered as just "Clear".
- Changes to Farmers
  - Drought-like weather will wither away a farmer's crops.

## May 16, 2020

- Some optimizations
  - Changed how messages are logged
    - Discovered that rich text is a thing, so rather than having a separate GameObject for each line,
    each day has it's own GameObject, so there *might* be less RAM usage. Maybe.
      - With this discovery, `LogSpace` and `LogMessage`, as well as the way colours are handled, were updated accordingly.
  - Disabled Graphic Raycaster for aformentioned message GameObjects
    - This should reduce the amount of time it takes to simulate a day.
  - Split off different UI sections into separate canvases
    - Apparently this is a good thing to do according to Unity documentation.
- Fixed a bug where UI buttons were being prematurely re-enabled when unpausing while simulating multiple days
- Updated `To-Do List.md`
