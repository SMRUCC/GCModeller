@echo off

SET base=P:\huang
SET sp_name=Genus_species

REM Requirements
REM
REM 1. GCModeller X64
REM 2. CMD.Internals toolkits

REM Whitespace and non-ascii character and symbols should avoid in the path string value.

REM config the genome data source and the virtual cell model output path
SET genome="%base%\genome"
SET chromosome_gbk=%genome%\huang.gbk
SET chromosome_id=huang
SET model_output="%base%\bacterial.GCmarkup"

REM config KEGG repository data directory location at here
SET br08901_maps="P:\91001_GB\KGML\br08901"
SET KEGG_cpds="P:\91001_GB\KGML\KEGG_cpd"
SET br08201_network="P:\91001_GB\KGML\br08201"
SET maps="P:\91001_GB\KGML\Maps"

REM config myva COG database files location
SET myva="E:\GCModeller-repo\COGs\Myva\myva"
SET whog="E:\GCModeller-repo\COGs\Myva\whog.XML"

REM config for UniProt reference sequence database
SET uniprot="P:\91001_GB\KO\uniprot-bacteria.KO.faa"

REM config for RegPrecise database repository
SET regpreciseMotifs="P:\91001_GB\transcript_regulations\Regprecise.motifs.fasta"
SET regpreciseRegulators="P:\91001_GB\transcript_regulations\regulators\KEGG_genomes.fasta"
SET regprecise="P:\91001_GB\transcript_regulations\RegpreciseDownloads"

REM config for the component output directory
SET KO_base=%base%\KO
SET COG_base=%base%\COG
SET TF_base=%base%\transcript_regulations

mkdir %KO_base%
mkdir %COG_base%
mkdir %TF_base%

REM first of all
REM Extract sequence data and genome context files
foreach *.gbk in %genome% do localblast /Export.gb /gb $file /out %genome%

REM KO annotation base on SBH
mkdir "%KO_base%\blastp"
mkdir "%COG_base%\blastp"

REM blastp procedures
makeblastdb -in "%uniprot%" -dbtype prot

foreach dir in %genome% do makeblastdb -in "$file/%sp_name%.faa" -dbtype prot
foreach dir in %genome% do blastp -query "$file/%sp_name%.faa" -db %uniprot% -evalue 1e-5 -num_threads 8 -out "%KO_base%\blastp\$basename.txt"
REM and then export raw blastp output result using GCModeller CLI tools.
foreach *.txt in "%KO_base%\blastp" do localblast /SBH.Export.Large /in $file /out "%KO_base%\KO_align_sbh\$basename.csv" /s.pattern "tokens | first" /q.pattern "tokens | 4" /identities 0.15 /coverage 0.5 /top.best
foreach *.csv in "%KO_base%\KO_align_sbh" do eggHTS /proteins.KEGG.plot /in "$file" /field "hit_name" /geneId.field "query_name" /size 2600,2200 /tick -1 /out "%KO_base%\KO_profiles\$basename"
REM mark the gene hits on KEGG pathway maps
foreach dir in "%KO_base%\KO_profiles" do eggHTS /KEGG.Color.Pathway /in "$file/KOCatalogs.csv" /ref %maps% /out "$file/maps"

REM COG annotation base on SBH
foreach dir in %genome% do blastp -query "$file/%sp_name%.faa" -db %myva% -evalue 1e-5 -num_threads 8 -out "%COG_base%\blastp\$basename.txt"
foreach *.txt in "%COG_base%\blastp" do localblast /COG.myva /blastp $file /whog %whog% /top.best /grep "tokens | 4" /out "%COG_base%\profiles\$basename.csv"
foreach *.csv in "%COG_base%\profiles" do eggHTS /COG.profiling.plot /in "$file" /size 2000,1300 /out "%COG_base%\$basename.png"

REM motif predicts
foreach dir in %genome% do makeblastdb -in "$file/%sp_name%.fna" -dbtype nucl

mkdir "%TF_base%\motifs\blastn"
mkdir "%TF_base%\regulators\blastp"

REM blastn mappings
REM no evalue filter
foreach dir in %genome% do blastn -query %regpreciseMotifs% -db "$file/%sp_name%.fna" -word_size 5 -out "%TF_base%\motifs\blastn\$basename.txt" /@set "/parallel=8;/clr=false"

REM export blastn mapping result and do motif tree cluster for the predictions
foreach *.txt in "%TF_base%\motifs\blastn" do localblast /Export.blastnMaps /in $file /out "%TF_base%\motifs\mappings\$basename.csv"
foreach *.csv in "%TF_base%\motifs\mappings" do VirtualFootprint /scan.blastn.map.motifsite /in $file /hits.base 5 /out "%TF_base%\motifs\sites\$basename.csv"
foreach *.csv in "%TF_base%\motifs\sites" do VirtualFootprint /Site.match.genes /in $file /genome "%genome%\$basename\%sp_name%.gbff" /max.dist 250 /replicon "locus" /out "%TF_base%\motifs\contexts\$basename.csv" /skip.RNA

REM TF regulators predictions
makeblastdb -in %regpreciseRegulators% -dbtype prot
foreach dir in %genome% do blastp -query "$file/%sp_name%.faa" -db %regpreciseRegulators% -evalue 1e-5 -num_threads 7 -out "%TF_base%\regulators\blastp\$basename.txt"

REM sbh method
REM due to the reason of orthology can be inherits from multiple source
REM So in this step, no top best hits limitation
REM This will resulted multiple genome source regulation network that will be created in next step
foreach *.txt in "%TF_base%\regulators\blastp" do localblast /SBH.Export.Large /in $file /out "%TF_base%\regulators\sbh\$basename.csv" /s.pattern "tokens ' ' first" /q.pattern "tokens | 4" /identities 0.6 /coverage 0.8

REM regulator annotations
foreach *.csv in "%TF_base%\regulators\sbh" do regprecise /regulators.bbh /bbh $file /regprecise %regprecise% /sbh /allow.multiple /description %regpreciseRegulators% /out "%TF_base%\regulators\mappings\$basename.csv"

REM if want to union the regulator predicts data between different replicons
REM the using
REM Excel /rbind /in "%TF_base%\regulators\mappings\%chromosome_id%.csv" /out %TF_union%

REM build TF regulation network after we have create the motif site and TF predictions
foreach *.csv in "%TF_base%\regulators\mappings" do VirtualFootprint /regulation.footprints /regulator "$file" /footprint "%TF_base%\motifs\contexts\$basename.csv" /out "%TF_base%\result_networks\$basename.csv"

REM TF from chromosome regulates genes from plasmids
foreach *.csv in "%TF_base%\motifs\contexts" do VirtualFootprint /regulation.footprints /regulator "%TF_base%\regulators\mappings\%chromosome_id%.csv" /footprint "$file" /out "%TF_base%\result_networks\$basename.csv"

REM chromosome map plot
mapplot /Config.Template /out "%genome%\plot\config.inf"
mapplot --Draw.ChromosomeMap.genbank /gb "%chromosome_gbk%" /motifs "%TF_base%\motifs\contexts\%chromosome_id%.csv" /hide.mics /conf "%genome%\plot\config.inf" /out "%genome%\plot" /COG "%COG_base%\profiles\%chromosome_id%.csv"

REM compile virtual cell data model

SET KO_union="P:\91001_GB\KO\NC_005810.KO.csv"
SET TF_union="P:\91001_GB\transcript_regulations\NC_005810.tf_regulations.csv"
SET GB_union="P:\91001_GB\NC_005810.genbank"

REM first of all, union the chromosome and plasmids genome data
ncbi_tools /gbff.union /in %genome% /out %GB_union%
REM and then, union all KEGG annotation data
Excel /rbind /in "%KO_base%\KO_profiles\*\KOCatalogs.csv" /out %KO_union%
REM at last, union all of the TF-regulation data
Excel /rbind /in "%TF_base%\result_networks" /out %TF_union%

REM finally, we are able to build this virtual cell model
GCC /compile.KEGG /in %GB_union% /KO %KO_union% /maps %br08901_maps% /compounds %KEGG_cpds% /reactions %br08201_network% /regulations %TF_union% /out %model_output%