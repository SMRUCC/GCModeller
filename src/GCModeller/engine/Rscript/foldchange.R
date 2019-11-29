imports "gseakit.DEG_sample" from "gseakit.dll";

let sampleInfo <- [file = ?"--sampleInfo"] 
  :> read.sampleinfo 
  :> as.object 
  :> groupBy(sample => sample$sample_group);

for(sample in sampleInfo) {
    print(sample);
}