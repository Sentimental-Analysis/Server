Sales Portal Core Library

|    CI    |      Status   |
|----------|:-------------:|
| AppVeyor(Windows): | [![Build status](https://ci.appveyor.com/api/projects/status/4qlnc6et6o03h128?svg=true)](https://ci.appveyor.com/project/dominikus1993/core-4k299) |
| CircleCI(Linux): | [![CircleCI](https://circleci.com/gh/Sentimental-Analysis/Core.svg?style=svg)](https://circleci.com/gh/Sentimental-Analysis/Core) |
| MyGet: | [![sentimental-analysis MyGet Build Status](https://www.myget.org/BuildSource/Badge/sentimental-analysis?identifier=4ebac062-0d30-4112-bb61-4826b1056882)](https://www.myget.org/) |
## How to Build
1. Install dotnet core SDK https://www.microsoft.com/net/core#Windows
2. dotnet restore 
3. dotnet build src/Core
4. dotnet test test/Core.Tests