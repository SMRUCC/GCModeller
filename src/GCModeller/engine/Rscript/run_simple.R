require(GCModeller);

imports ["rawXML", "simulator", "modeller"] from "vcellkit";

setwd("P:\2022_nar\vcell");

modelfile  = "model.XML";
model      = as.object(read.vcell(path = modelfile));
time.ticks = 1000;

print("Run virtual cell model:");
print(model);

vcell = vcell.model(model);
mass  = vcell.mass.index(vcell);
flux  = vcell.flux.index(vcell);

dynamics = dynamics.default() :> as.object;

dynamics$transcriptionBaseline   = 200;
dynamics$transcriptionCapacity   = 500;
dynamics$productInhibitionFactor = 0.00000125;

print("Using dynamics parameter configuration:");
print(dynamics);

rawXml = "result.vcXML";

engine = vcell
|> engine.load(	
	iterations       = time.ticks, 
	time_resolutions = 1000, 	
	showProgress     = TRUE
) 
|> as.object()
;

using xml as open.vcellXml(file  = rawXml, mode  = "write", vcell = engine) {
	print(rawXml);

	# run virtual cell simulation and then 
	# save the result snapshot data files into 
	# target data directory
	engine$AttachBiologicalStorage(xml);
	engine$Run();
}