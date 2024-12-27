
# Bayonetta Cos Tool

A tool for previewing and modifying Bayonetta 3 cos files that contain the color palettes used for clothes and hair recolors.

As they are, the color values come in an exponential scale of 0.0 to 1.0, instead of the usual linear range of 0 to 255. This means the color values change more extremely the closer they are to 1.0. For this reason I added a built in function that converts those values to the linear scale of 0 to 255 for easier use.
## Features

- Support for cos files
- Real time preview of each color
- Scale toggle to switch between 0.0 to 1.0 and 0 to 255
- Complete saving functionality


## Downloads

There are two options of builds to choose from at [releases](https://github.com/Lyzder/BayoCosTool/releases):

| Build     | Description                               |
| :-------- | :---------------------------------------- |
| Self contained | Single bigger and independent executable file|
| Shared runtime | Multiple files to run with .Net runtime|

The self contained release consists of a single .exe file made to run in any Windows PC without any external requirements. On the other hand, the shared runtime is composed of multiple, much smaller files, but it requires the .Net runtime to be installed in the PC to be able to run.

For information and download links for the .Net runtime go to their [official website](https://dotnet.microsoft.com/en-us/download).
## Special Thanks

 - Kerilk for his templates that made the work so much easier, and for the tools we are still using to this day.
 - Skyth for lending me his endian binary classes.


## Feedback

If you have any feedback or questions, please join the modding scene discord server [Infernal Warks](https://discord.com/invite/nX5EzVU).

