imports "gseakit.DEG_sample" from "gseakit.dll";

let [result, output] = [?"--data", ?"--save"];

if(is.empty(output)) {
	output <- `${dirname(result)}/sampleInfo.csv`;
}

result 
:> sampleinfo.text.groups 
:> write.sampleinfo(file = output);

