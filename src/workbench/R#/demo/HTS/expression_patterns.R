imports "visualkit.plots" from "visualkit";
imports "geneExpression" from "phenotype_kit";

setwd(!script$dir);

"E:\smartnucl_integrative\biodeep_pipeline\Biodeep_Rpackage\etc\pathway_network\msms_Intensity.csv"
:> load.expr
:> relative
:> plot.expression_patterns
:> save.graphics(file = "./patterns.png")
;