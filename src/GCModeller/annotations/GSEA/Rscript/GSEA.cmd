@echo off

SET background="../data/uniprot-taxonomy_314565_GO.XML"
SET geneSet="../data/xcb_TCS_uniprot.txt"
SET go="P:\go.obo"

R# ./GSEA.R --background %background% --geneSet %geneSet% --go %go% --save ./GO.csv