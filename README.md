# Reflog

Helper utility to see recently checked out branches, switch to them, diff against current branch, or copy the names to clipboard.

## Installation

### From GitHub Releases
1. Go to the [Releases](https://github.com/RendijsSmukulis/reflog/releases) page
2. Download the appropriate version for your platform:
   - `reflog-windows-x64.zip` for Windows
   - `reflog-linux-x64.tar.gz` for Linux
   - `reflog-macos-x64.tar.gz` for macOS
3. Extract the files and run the executable

### From Source
```bash
git clone https://github.com/RendijsSmukulis/reflog --recurse-submodules
cd reflog
dotnet build src/Reflog/Reflog.csproj --configuration Release
```

## Usage

Run the executable in a git repository:

```bash
reflog [number]
```

Where `[number]` is optional and specifies how many recent branches to show (default: 10).

### Interactive Commands
- **Enter**: Switch to the selected branch
- **C**: Copy branch name to clipboard
- **D**: Open difftool to compare with selected branch
- **Q**: Quit the application

## Building Releases

This project uses GitHub Actions to automatically build and release versions when tags are pushed:

```bash
git tag v1.0.0
git push origin v1.0.0
```

This will trigger the release workflow and create a new GitHub release with artifacts for all supported platforms.

## Development

### Prerequisites
- .NET 8.0 SDK
- Git

### Building Locally
```bash
dotnet restore src/Reflog/Reflog.csproj
dotnet build src/Reflog/Reflog.csproj --configuration Release
```

### Running Tests
```bash
dotnet test src/Reflog/Reflog.csproj
```
