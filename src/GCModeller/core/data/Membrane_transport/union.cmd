@echo off

kegg_tools /KO.list /kgml "ko03070.xml" /out "ko03070.csv"
kegg_tools /KO.list /kgml "ko02010.xml" /out "ko02010.csv"
kegg_tools /KO.list /kgml "ko02060.xml" /out "ko02060.csv"

excel /rbind /in ./ /out ../Membrane_transport.csv /order_by EC_number