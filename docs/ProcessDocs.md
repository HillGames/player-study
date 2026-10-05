# Process Doc

## Create Godot Project and Build it

## Create test project
**1) create Test project**

``` bash
dotnet.exe new nunit -n MyGame.Tests
```
-----
**2) Add test project ot main solution**
``` bash
dotnet.exe sln add MyGame.Tests/MyGame.Tests.csproj
dotnet.exe sln list
```
---
**3) update csproj files to make sure all of them has same run time configured**
---
**4) check run time is available in the pc**
```bash
dotnet.exe --list-runtimes
```
-------
**5) Test the created solution by createin gsimple test fucniton**
``` bash
dotnet.exe test player-study.tests/player-study.tests.csproj 
``` 
-----
**6)**
