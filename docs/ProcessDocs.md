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
**6)Add project reference**
``` bash 
dotnet list player-study.tests/player-study.tests.csproj reference
dotnet list player-study.tests/player-study.tests.csproj reference
```
``` bash
  511  dotnet.exe new classlib -n PlayerStudy.Core -f net8.0
  512  rm -rf PlayerStudy.Core/Class1.cs 
  513  dotnet.exe sln PlayerStudy.sln add PlayerStudy.Core/PlayerStudy.Core.csproj 
  514  dotnet.exe sln PlayerStudy.sln list
  515  dotnet.exe sln PlayerStudy.sln remove player-study.tests\player-study.tests.csproj
  516  dotnet.exe sln PlayerStudy.sln list
  517  dotnet.exe sln PlayerStudy.sln remove player-study.tests\player-study.tests.csproj
  518  dotnet.exe sln PlayerStudy.sln remove player-study.tests\player-study.tests.csproj
  519  dotnet.exe sln PlayerStudy.sln list
  520  dotnet.exe sln PlayerStudy.sln list
  521  dotnet.exe new nunit -n PlayerStudy.Test
  522  dotnet.exe PlayerStudy.sln add PlayerStudy.Test/PlayerStudy.Test.csproj 
  523  dotnet.exe sln PlayerStudy.sln add PlayerStudy.Test/PlayerStudy.Test.csproj 
  524  dotnet.exe sln PlayerStudy.sln list
  525  dotnet add PlayerStudy.Test/PlayerStudy.Test.csproj reference PlayerStudy.Core/PlayerStudy.Core.csproj 
  526  dotnet.exe list PlayerStudy.Test/PlayerStudy.Test.csproj 
  527  dotnet.exe list PlayerStudy.Test/PlayerStudy.Test.csproj reference
  528  history
```