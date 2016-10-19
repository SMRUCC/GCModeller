#!/bin/sh

for fn in *; do
    if [ -d $fn ]; then
        echo "$fn is a directory"
    fi
    if [ -f $fn ]; then
        echo "$fn is a file"
    fi
done