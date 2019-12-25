imports "visualkit.plots" from "visualkit.dll";

["D:\GCModeller\src\workbench\R#\demo\kegg\profiles.json"] 
:> read.list(mode="numeric")
:> kegg.category_profiles.plot()
:> save.graphics(file = "./profiles.png")
;
