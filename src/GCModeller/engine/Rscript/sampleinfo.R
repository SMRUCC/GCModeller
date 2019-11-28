imports "gseakit.DEG_sample" from "gseakit.dll";

let [result, output] = [?"--data", ?"--save"];

result 
:> sampleinfo.text.groups 
:> write.sampleinfo(file = output);

