#!/bin/sh

# Running start from the GCModeller root
# Change work directory into bin
cd ./bin

DIR="$PWD"

for app in *.exe; do
    if [ -f $app ]; then
	
		# Generates the bash shortcuts for mono running .NET app on Linux/macOS
        $(mono "$DIR/$app" "/linux-bash")
    fi
done

# Start configuring the GCModeller environment variables
# Required of user inputs or tools auto configuration

./Settings set 

# Returns back to the GCModeller root
cd ..