require(GCModeller);

imports "visualPlot" from "visualkit";
imports ["geneExpression", "sampleInfo"] from "phenotype_kit";

@profile {

	x = load.expr("C:\GSM3067190_06hpf.csv");
	x = NULL;
	
}

print(" ~~done!");
profile = profiler.fetch() |> as.data.frame();
print(profile, max.print = 13);

@profile {

	x = load.expr0("C:\GSM3067190_06hpf.HTS");
	x = NULL;
	
}

print(" ~~done!");
profile = profiler.fetch() |> as.data.frame();
print(profile, max.print = 13);

