
# TAMAI

Transient Absorption spectroscopy Multi-platform Analyzer Interface

## Overview

TAMAI is a .NET library for the analysis of transient absorption spectroscopy data. It is designed to be a flexible and modular tool for the analysis of data from a variety of sources.
This library is designed to be used in any .NET application, including Windows, Linux, and macOS.

## Requirements

TAMAI requires the following:

- .NET 7.0

## Usage

The classes in the `TAMAI.Spectra` namespace provide the core functionality of TAMAI.
To load data from a file and to create spectra, use the classes which inherit from the `TAMAI.Data.TasData` class.

```csharp
using TAMAI.Data;

var data = new MicroSecondTasData("/path/to/the/data/folder");
// `data.Spectra` contains the spectra
```

For more information, see the documentation comments in the source code.
Implementations of the TAMAI for Win (located in `TAMAI.Win`) also help to understand how to use the library.

### Analysis

The `TAMAI.Analysis` namespace contains classes for the analysis of spectra, such as decay curve fitting.

```csharp
var decay = data.Spectra[wlStart, wlEnd];  // `wlStart` and `wlEnd` are the start and end wavelengths of the decay curve.
var X = decay.Select(d => d.Key.Second);
var Y = decay.Select(d => d.Value.Absolute.OD);
var model = new SingleExponentialDecay();
var func = model.GetFunc(X, Y, null);
// `func.TimeConstant` contains the time constant of the decay curve.
```

## Features

- Load transient absorption spectroscopy data to create spectra.
- Fit decay curves at a given wavelength to various models.

## TAMAI for Win

TAMAI for Win is a Windows application that provides a graphical user interface for TAMAI. It is designed to be a simple and easy-to-use tool for the analysis of transient absorption spectroscopy data.

## License

This project is licensed under the terms of the [MIT license](https://choosealicense.com/licenses/mit/).
