require(GCModeller);

imports "uniprot" from "seqtoolkit";

print(parseHeader(c("- && sp|O22795|RK28_ARATH Large ribosomal subunit protein bL28c OS=Arabidopsis thaliana OX=3702 GN=RPL28 PE=2 SV=2 && PF00830:Ribosomal L28 family",
"- && sp|Q9FXT6|PEX14_ARATH Peroxisomal membrane protein PEX14 OS=Arabidopsis thaliana OX=3702 GN=PEX14 PE=1 SV=2 && PF04695:Pex14 N-terminal domain",
"- && sp|Q8GYT9|SIS3_ARATH E3 ubiquitin-protein ligase SIS3 OS=Arabidopsis thaliana OX=3702 GN=SIS3 PE=2 SV=2 && PF13639:Ring finger domain",
"- && - && -",
"- && sp|O49453|Y4844_ARATH Uncharacterized protein At4g28440 OS=Arabidopsis thaliana OX=3702 GN=At4g28440 PE=1 SV=1 && PF21473:Single-stranded DNA binding protein Ssb-like, OB fold",
"- && sp|Q9FFW5|PERK8_ARATH Proline-rich receptor-like protein kinase PERK8 OS=Arabidopsis thaliana OX=3702 GN=PERK8 PE=1 SV=1 && PF07714:Protein tyrosine and serine/threonine kinase",
"- && sp|Q84WN0|Y4920_ARATH Uncharacterized protein At4g37920 OS=Arabidopsis thaliana OX=3702 GN=At4g37920 PE=2 SV=2 && -",
"- && sp|A0A517FNC4|72A61_PARPY Cytochrome P450 CYP72A616 OS=Paris polyphylla OX=49666 GN=CYP72A616 PE=1 SV=1 && PF00067:Cytochrome P450",
"- && sp|Q9SX33|ALA9_ARATH Putative phospholipid-transporting ATPase 9 OS=Arabidopsis thaliana OX=3702 GN=ALA9 PE=3 SV=1 && PF13246:Cation transport ATPase (P-type)|PF16209:Phospholipid-translocating ATPase N-terminal|PF16212:Phospholipid-translocating P-type ATPase C-terminal")));