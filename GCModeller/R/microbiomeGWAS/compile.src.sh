#!/bin/sh

cd ./src
R CMD SHLIB dExp1.c
R CMD SHLIB dExp2.c
R CMD SHLIB parsePlink.c
R CMD SHLIB parsePlink2.c

rm *.o
mv *.so ../lib
