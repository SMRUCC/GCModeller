@echo off

SET workspace=K:\20191112\wildtype
SET KO="%workspace%/model/faa_vs_KO.sbh.KO_bbh.wildtype,size=4521.KOclusters.Xml"
SET save="%workspace%/run/functional_analysis.enrichments.csv"

R# ./enrichment_result_union.R --data "%workspace%/run/functional_analysis" --save %save% --geneSet "%workspace%/model/1025_EG.txt" --KO %KO%