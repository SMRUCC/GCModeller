imports "vcellkit.rawXML" from "vcellkit.dll";

let path as string = "K:\20200226\metabolism\vcell\result\raw\control_6.vcXML";

using xml as open.vcellXml(file = path, mode = "read") {
	let index = frame.index(xml);
	
	print(index);
}