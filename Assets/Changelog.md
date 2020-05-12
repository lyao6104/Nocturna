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
