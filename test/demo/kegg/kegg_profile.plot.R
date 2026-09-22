imports "visualkit.plots" from "visualkit.dll";

["D:\GCModeller\src\workbench\R#\demo\kegg\profiles.json"] 
:> read.list(mode="numeric")
:> kegg.category_profiles.plot(
	size = "2200,2100", 
	displays = 8,
	title = "KEGG Compound Pathway Profiling",
	axisTitle = "Number Of KEGG Compounds",
	tick = 50,
	colors = "#E41A1C,#377EB8,#4DAF4A,#984EA3,#FF7F00,#CECE00"
)
:> save.graphics(file = "./profiles.png")
;
