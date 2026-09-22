## ncbi mesh data

mtrees2024.txt - ncbi mesh tree https://lhncbc.nlm.nih.gov/ii/information/MBR/Stree/2024/

## kegg data

all kegg database file is stored via the git LFS storage.

+ kegg.zip

a kegg reference database file which is generated via 
the ``kegg_api`` package. the file strucutres of this
database file is:

    + <pathway>/
       + map.xml
       
       + compounds/
       + modules/

            + <module>
               + module.xml
               
               + reactions/
               + compounds/
