require(GCModeller);

imports "sampleInfo" from "phenotype_kit";

# sampleInfo(
	# ID          = ["A1", "A2", "A3", "B1", "B2", "B3", "C1", "C2", "C3"],
	# sample_name = ["A1", "A2", "A3", "B1", "B2", "B3", "C1", "C2", "C3"],
	# sample_info = ["A", "A", "A", "M", "M", "M", "C", "C", "C"]
# ) :> write.csv(file = "P:\2020_videos\biological_analysis\assets\sampleInfo.csv");

setwd(@dir);

sampleInfo(
	ID          = ["C61","C62","C63","C64","C91","C92","C93","C94","I561","I562","I563","I564","I591","I592","I593","I594","I861","I862","I863","I864","I891","I892","I893","I894"],
	sample_name = ["C61","C62","C63","C64","C91","C92","C93","C94","I561","I562","I563","I564","I591","I592","I593","I594","I861","I862","I863","I864","I891","I892","I893","I894"],
	sample_info = ["C6","C6","C6","C6","C9","C9","C9","C9","I56","I56","I56","I56","I59","I59","I59","I59","I86","I86","I86","I86","I89","I89","I89","I89"],
) 
|> write.csv(file = "./sampleInfo.csv");