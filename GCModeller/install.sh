#!/bin/sh

cd ./bin

DIR="$PWD"

for fn in *.exe; do
    if [ -f $fn ]; then
        $(mono "$DIR/$fn" "/linux-bash")
    fi
done

cd ..