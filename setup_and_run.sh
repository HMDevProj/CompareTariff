#!/bin/bash

# Define the required .NET SDK version
DOTNET_VERSION="8.0"

# Function to install .NET SDK
install_dotnet() {
  echo "Installing .NET SDK $DOTNET_VERSION..."

  # Download Microsoft package signing key and feed
  wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
  sudo dpkg -i packages-microsoft-prod.deb
  rm packages-microsoft-prod.deb

  # Install the .NET SDK
  sudo apt-get update
  sudo apt-get install -y dotnet-sdk-$DOTNET_VERSION

  echo ".NET SDK $DOTNET_VERSION installed."
}

# Function to install Node.js and npm
install_node() {
  echo "Installing Node.js and npm..."
  sudo apt-get update
  sudo apt-get install -y nodejs npm

  echo "Node.js and npm installed."
}

# Function to install SQL Server
install_sql_server() {
  echo "Installing SQL Server..."

  # Import the public repository GPG keys
  wget -qO- https://packages.microsoft.com/keys/microsoft.asc | sudo apt-key add -

  # Register the SQL Server Ubuntu repository
  sudo add-apt-repository "$(wget -qO- https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/mssql-server-$(lsb_release -rs).list)"
  sudo apt-get update
  sudo apt-get install -y mssql-server

  echo "SQL Server installed."
}

# Function to install .NET application dependencies
install_app_dependencies() {
  echo "Installing application dependencies..."

  # Restore .NET project dependencies
  dotnet restore

  echo "Application dependencies installed."
}

# Function to run the application
run_application() {
  echo "Building and running the application..."

  # Build the application
  dotnet build

  # Run the application
  dotnet run --project YourProjectName/YourProjectName.csproj
}

# Install .NET SDK if not already installed
if ! dotnet --list-sdks | grep -q "$DOTNET_VERSION"; then
  install_dotnet
else
  echo ".NET SDK $DOTNET_VERSION is already installed."
fi

# Install Node.js and npm if not already installed
if ! command -v node &> /dev/null; then
  install_node
else
  echo "Node.js is already installed."
fi

# Install SQL Server if not already installed
if ! command -v sqlcmd &> /dev/null; then
  install_sql_server
else
  echo "SQL Server is already installed."
fi

# Install application dependencies
install_app_dependencies

# Run the application
run_application
