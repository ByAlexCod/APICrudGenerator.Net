# C# CRUD generator v1.0.0
For now, the generator only generates the sql script and the c# model.

## Install
Installing the generator is very simple. You just need to type this one command in your console.
```bash
dotnet tool install --global ALS.API.CRUDGenerator
```
The tool is now accessible from the console by typing `csgen`

## Create a configuration
To start using the generator tool for a new project you have to launch this command from the console.
```bash
csgen -i -c "[configuration_path]"
```
Here, `-i` means "init" and the `-c` is made to specify the configuration path. It will create a json file containing information about the project for which you are generating code. You can modify the configuration by hand if needed. 

## Generating a new model
Generating a new model can be done by typing this command:
```bash
csgen -c "[configuration_path]"
```
Questions will be asked and files will be generated using paths in the configuration file.

**Files that are generated should always be verified by hand.**

## Current features
- Generates a .sql migration file used to created the table.
- Generates a .cs file containing the get/set simple model.

## What's next?
- Generating the gateway and controller
- Direct configuration of constraints others than Primary Key in thewizard.