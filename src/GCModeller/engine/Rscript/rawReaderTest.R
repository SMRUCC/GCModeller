imports "vcellkit.rawXML" from "vcellkit.dll";

let path as string = "K:\20200226\metabolism\vcell\result\raw\control_6.vcXML";

using raw as open.vcellXml(file = path, mode = "read") {
	# debug test
	frame.index(raw) :> print;
	
	# extract data
	raw 
	:> time.frames(metabolome = "mass_profile")
	:> write.csv(file = "K:\20200226\metabolism\vcell\result\raw\control_6.metabolome.csv")
	;
}