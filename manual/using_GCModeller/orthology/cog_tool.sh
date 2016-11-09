#!/bin/bash

# If this is the first time running the cog_tool, then install the COG database first:

# <space> in file path is not allowed for blast+
COG="path/to/prot2003-2014.fasta"

cd "path/to/GCModeller/bin"

# If the NCBI localblast+ suite is not config yet, then run the folling command
./Settings set "DIR.BlastBin" "path/to/ncbi_blast+/bin/"
# Run this command for config the COG database 
./localblast 