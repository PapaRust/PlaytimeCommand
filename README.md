# Playtime Command
An oxide plugin for Rust that allows a player to use the /playtime command to retrieve their hours played on the current server from BattleMetrics API.

**Author:** YourName  
**Version:** 1.1.0  
**Description:** Shows playtime of a player on a specific server using BattleMetrics API.

## Overview

The Playtime Command plugin provides a command for players to check their total playtime on a specific Rust server. It retrieves data from the BattleMetrics API, ensuring players have access to accurate playtime statistics.

## Features

- Retrieves player playtime from the BattleMetrics API.
- Displays playtime in hours.
- Configurable API key, server ID, and server name.

## Installation

1. **Download the Plugin:**
   - Download the `PlaytimeCommand.cs` file.

2. **Upload the Plugin:**
   - Place the `PlaytimeCommand.cs` file in your server's `oxide/plugins` directory.

3. **Load the Plugin:**
   - The plugin will automatically load. If not, use the command:
     ```
     oxide.load PlaytimeCommand
     ```

## Configuration

The plugin requires the BattleMetrics API key, server ID, and server name to function correctly. These values must be set in the configuration file.

### Configuration File

The configuration file is generated upon the first load of the plugin and is located at `oxide/config/PlaytimeCommand.json`. 

```json
{
  "BattleMetricsAPIKey": "YOUR_BATTLEMETRICS_API_KEY",
  "ServerID": "YOUR_SERVER_ID",
  "ServerName": "YOUR_SERVER_NAME"
}
