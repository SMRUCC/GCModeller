@echo off

REM Requirements
REM
REM 1. GCModeller X64
REM 2. CMD.Internals toolkits

REM config the genome data source and the virtual cell model output path
SET genome="P:\91001_GB\genome"
SET model_output="P:\91001_GB\NC_005810.GCmarkup"

REM config KEGG repository data directory location at here
SET br08901_maps="P:\91001_GB\KGML\br08901"
SET KEGG_cpds="P:\91001_GB\KGML\KEGG_cpd"
SET br08201_network="P:\91001_GB\KGML\br08201"

REM config myva COG database files location
SET myva="E:\GCModeller-repo\COGs\Myva\myva"
SET whog="E:\GCModeller-repo\COGs\Myva\whog.XML"

REM config for UniProt reference sequence database
SET uniprot="P:\91001_GB\KO\uniprot-bacteria.KO.faa"

REM config for the component output directory
SET KO_base=P:\91001_GB\KO
SET COG_base=P:\91001_GB\COG

REM first of all
REM Extract sequence data and genome context files
foreach *.gbk in %genome% do localblast /Export.gb /gb $file /flat

REM KO annotation base on SBH

REM blastp procedures
makeblastdb -in "%KO_base%\uniprot-bacteria.KO.faa" -dbtype prot
foreach dir in %genome% do makeblastdb -in "$file/Yersinia_pestis_biovar_Microtus_str._91001.faa" -dbtype prot
foreach dir in %genome% do blastp -query "$file/Yersinia_pestis_biovar_Microtus_str._91001.faa" -db %uniprot% -evalue 1e-5 -num_threads 8 -out "P:\91001_GB\KO\blastp\$basename.txt"
foreach *.txt in "%KO_base%\blastp" do localblast /SBH.Export.Large /in $file /out "P:\91001_GB\KO\KO_align_sbh\$basename.csv" /s.pattern "tokens | first" /q.pattern "tokens | 4" /identities 0.15 /coverage 0.5 /top.best
foreach *.csv in "%KO_base%\KO_align_sbh" do eggHTS /proteins.KEGG.plot /in "$file" /field "hit_name" /geneId.field "query_name" /size 2600,2200 /tick -1 /out "P:\91001_GB\KO\KO_profiles\$basename"

REM COG annotation base on SBH
foreach dir in %genome% do blastp -query "$file/Yersinia_pestis_biovar_Microtus_str._91001.faa" -db %myva% -evalue 1e-5 -num_threads 8 -out "%COG_base%\blastp\$basename.txt"
foreach *.txt in "%COG_base%\blastp" do localblast /COG.myva /blastp $file /whog %whog% /top.best /grep "tokens | 4" /out "%COG_base%\profiles\$basename.csv"
foreach *.csv in "%COG_base%\profiles" do eggHTS /COG.profiling.plot /in "$file" /size 2000,1300 /out "P:\91001_GB\COG\$basename.png"

REM motif predicts
foreach dir in %genome% do makeblastdb -in "$file/Yersinia_pestis_biovar_Microtus_str._91001.fna" -dbtype nucl

REM blastn mappings
REM no evalue filter
foreach dir in %genome% do blastn -query "P:\91001_GB\transcript_regulations\Regprecise.motifs.fasta" -db "$file/Yersinia_pestis_biovar_Microtus_str._91001.fna" -word_size 5 -out "P:\91001_GB\transcript_regulations\motifs\blastn\$basename.txt" /@set "/parallel=8;/clr=false"

REM export blastn mapping result and do motif tree cluster for the predictions
foreach *.txt in "P:\91001_GB\transcript_regulations\motifs\blastn" do localblast /Export.blastnMaps /in $file /out "P:\91001_GB\transcript_regulations\motifs\mappings\$basename.csv"
foreach *.csv in "P:\91001_GB\transcript_regulations\motifs\mappings" do VirtualFootprint /scan.blastn.map.motifsite /in $file /hits.base 5 /out "P:\91001_GB\transcript_regulations\motifs\sites\$basename.csv"
foreach *.csv in "P:\91001_GB\transcript_regulations\motifs\sites" do VirtualFootprint /Site.match.genes /in $file /genome "P:\91001_GB\genome\$basename.gbk" /max.dist 300 /out "P:\91001_GB\transcript_regulations\motifs\contexts\$basename.csv" /skip.RNA

REM TF regulators predictions
makeblastdb -in "P:\91001_GB\transcript_regulations\regulators\KEGG_genomes.fasta" -dbtype prot
foreach dir in %genome% do blastp -query "$file/Yersinia_pestis_biovar_Microtus_str._91001.faa" -db "P:\91001_GB\transcript_regulations\regulators\KEGG_genomes.fasta" -evalue 1e-5 -num_threads 7 -out "P:\91001_GB\transcript_regulations\regulators\blastp\$basename.txt"

REM sbh method
REM due to the reason of orthology can be inherits from multiple source
REM So in this step, no top best hits limitation
REM This will resulted multiple genome source regulation network that will be created in next step
foreach *.txt in "P:\91001_GB\transcript_regulations\regulators\blastp" do localblast /SBH.Export.Large /in $file /out "P:\91001_GB\transcript_regulations\regulators\sbh\$basename.csv" /s.pattern "tokens ' ' first" /q.pattern "tokens | 4" /identities 0.6 /coverage 0.8

REM regulator annotations
foreach *.csv in "P:\91001_GB\transcript_regulations\regulators\sbh" do regprecise /regulators.bbh /bbh $file /regprecise "P:\91001_GB\transcript_regulations\RegpreciseDownloads" /sbh /allow.multiple /description "P:\91001_GB\transcript_regulations\regulators\KEGG_genomes.fasta" /out "P:\91001_GB\transcript_regulations\regulators\mappings\$basename.csv"

REM build TF regulation network after we have create the motif site and TF predictions
foreach *.csv in "P:\91001_GB\transcript_regulations\regulators\mappings" do VirtualFootprint /regulation.footprints /regulator "$file" /footprint "P:\91001_GB\transcript_regulations\motifs\contexts\$basename.csv" /out "P:\91001_GB\transcript_regulations\result_networks\$basename.csv"

REM TF from chromosome regulates genes from plasmids
foreach *.csv in "P:\91001_GB\transcript_regulations\motifs\contexts" do VirtualFootprint /regulation.footprints /regulator "P:\91001_GB\transcript_regulations\regulators\mappings\NC_005810.csv" /footprint "$file" /out "P:\91001_GB\transcript_regulations\result_networks\$basename.csv"

REM chromosome map plot
mapplot /Config.Template /out "P:\91001_GB\genome\plot\config.inf"
mapplot --Draw.ChromosomeMap.genbank /gb "P:\91001_GB\genome\NC_005810.gbk" /motifs "P:\91001_GB\transcript_regulations\motifs\contexts\NC_005810.csv" /hide.mics /conf "P:\91001_GB\genome\plot\config.inf" /out "P:\91001_GB\genome\plot" /COG "P:\91001_GB\COG\profiles\NC_005810.csv"

REM compile virtual cell data model

SET KO_union="P:\91001_GB\KO\NC_005810.KO.csv"
SET TF_union="P:\91001_GB\transcript_regulations\NC_005810.tf_regulations.csv"
SET GB_union="P:\91001_GB\NC_005810.genbank"

REM first of all, union the chromosome and plasmids genome data
ncbi_tools /gbff.union /in %genome% /out %GB_union%
REM and then, union all KEGG annotation data
Excel /rbind /in "P:\91001_GB\KO\KO_profiles\*\KOCatalogs.csv" /out %KO_union%
REM at last, union all of the TF-regulation data
Excel /rbind /in "P:\91001_GB\transcript_regulations\result_networks" /out %TF_union%

REM finally, we are able to build this virtual cell model
GCC /compile.KEGG /in %GB_union% /KO %KO_union% /maps %br08901_maps% /compounds %KEGG_cpds% /reactions %br08201_network% /regulations %TF_union% /out %model_output%