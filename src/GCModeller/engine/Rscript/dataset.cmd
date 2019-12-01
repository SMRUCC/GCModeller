@echo off

SET result=S:\synthetic_biology\test_model\wildtype\model\test20191127
SET kegg_cpd="D:\biodeep\biodeep_v2\data\KEGG\KEGG_cpd.commandNames.json"

R# ./dataset.R --data "%result%" --set "mass\metabolome.json" --kegg_cpd %kegg_cpd%