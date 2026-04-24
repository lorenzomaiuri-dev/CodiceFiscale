# CodiceFiscale
[![Stargazers][stars-shield]][stars-url]
[![MIT License][license-shield]][license-url]
[![NuGet Version][nuget-badge]][nuget-url]
[![Build Status][build-badge]][build-url]
[![LinkedIn][linkedin-shield]][linkedin-url]

A .NET Core library to calculate and validate the Italian Tax Code (Codice Fiscale)

## Table of Contents

- [Description](#description)
- [Installation](#installation)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)
- [Acknowledgments](#acknowledgments)

## Description

The **CodiceFiscale** library provides an easy-to-use interface for generating and validating the Italian Tax Code based on personal data such as name, surname, date of birth, and city of birth.

It is designed for .NET developers who need to work with the Italian Tax Code in applications like tax management, user registration, and government service integrations.

## Installation

You can install the CodiceFiscale library via NuGet Package Manager:

```bash
dotnet add package CodiceFiscale --version X.X.X
```

Or by adding the following to your .csproj:

```
<PackageReference Include="CodiceFiscale" Version="X.X.X" />
```

Once installed, restore the packages:

```bash
dotnet restore
```

## Usage

Here’s a few examples of how to use the library:

### Generate a Codice Fiscale
```csharp
using CodiceFiscaleLib.Helpers;

string result = EncodingHelper.Encode("Rossi", "Mario", 'M', new DateTime (1980, 5, 20), "Milano");

Console.WriteLine($"Generated Codice Fiscale: {result}");
```

### Decode a Codice Fiscale
```csharp
using CodiceFiscaleLib.Helpers;

// decode
var decoded = DecodingHelper.Decode(code);

// get the dictionary
var decodedDict = (Dictionary<string, string>)decoded["raw"];

// access the value
string firstName = decodedDict["firstname"];

Console.WriteLine($"Decoded Codice Fiscale First Name: {firstName}");
```

### Validate a Codice Fiscale
```csharp
using CodiceFiscaleLib.Helpers;

bool isValid = DecodingHelper.IsValid("RSSMRA80E20F205I");

Console.WriteLine($"Is valid Codice Fiscale: {isValid}");
```

## Contributing

Contributions are welcome! If you'd like to contribute to this project, please follow these steps:

1. Fork the repository
2. Create a new branch (git checkout -b feature/your-feature)
3. Commit your changes (git commit -am 'Add new feature')
4. Push the branch (git push origin feature/your-feature)
5. Open a Pull Request

Please ensure all pull requests pass the existing tests and include new tests for any added functionality

## License
This project is licensed under the MIT License. See the LICENSE file for more details

<!-- ACKNOWLEDGMENTS -->
## Acknowledgments

Special thanks to all contributors and resources that made this project possible

<!-- LINKS & IMAGES -->
[stars-shield]: https://img.shields.io/github/stars/lorenzomaiuri-dev/Reactive-Architecture?style=social
[stars-url]: https://github.com/lorenzomaiuri-dev/Reactive-Architecture/stargazers
[license-shield]: https://img.shields.io/badge/license-MIT-blue
[license-url]: https://mit-license.org/license.txt
[nuget-badge]: https://img.shields.io/nuget/v/CodiceFiscale.svg
[nuget-url]: https://www.nuget.org/packages/CodiceFiscale/
[build-badge]: https://img.shields.io/github/actions/workflow/status/lorenzomaiuri-dev/CodiceFiscale/dotnet.yml?branch=main
[build-url]: https://github.com/lorenzomaiuri-dev/CodiceFiscale/actions/workflows/dotnet.yml
[linkedin-shield]: https://img.shields.io/badge/LinkedIn-Profile-blue?logo=linkedin&logoColor=white
[linkedin-url]: https://it.linkedin.com/in/lorenzo-maiuri-9a7472244
