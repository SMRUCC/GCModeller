imports "sampleInfo" from "phenotype_kit";

sampleInfo(
	ID          = ["A1", "A2", "A3", "B1", "B2", "B3", "C1", "C2", "C3"],
	sample_name = ["A1", "A2", "A3", "B1", "B2", "B3", "C1", "C2", "C3"],
	sample_info = ["A", "A", "A", "M", "M", "M", "C", "C", "C"]
) :> write.csv(file = "P:\2020_videos\biological_analysis\assets\sampleInfo.csv");