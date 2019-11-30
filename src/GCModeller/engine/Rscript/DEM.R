require(VisualBasic.R)

Imports(Microsoft.VisualBasic.Data.Linq);

getSampleGroup <- function(sampleInfo.csv) {
	sampleInfo <- GroupBy(read.csv(sampleInfo.csv), key = "sample_group");

	for(label in names(sampleInfo)) {
		sampleInfo[[label]] <- sampleInfo[[label]][, "sample_name"] %=>% as.vector %=>% make.names;
	}
	
	sampleInfo;
}

DEM <- function(data.csv, sampleInfo.csv) {
	sampleGroups <- getSampleGroup(sampleInfo.csv);
	raw <- read.csv(data.csv);
	rownames(raw) <- as.vector(raw[, 1]);
	raw[, 1] <- NULL;
	normal <- sampleGroups$normal;
	normal <- raw[, normal];
	sampleGroups$normal <- NULL;
	result <- data.frame(KEGG_ID = rownames(raw));
	rownames(result) <- rownames(raw);
	
	for(groupName in names(sampleGroups) ) {
		print(groupName);
	
		EG_group <- sampleGroups[[groupName]];
		EG_group <- raw[, EG_group];
		EG_DEM <- lapply(rownames(EG_group), function(cid) {
			exp <- as.vector(EG_group[cid, ]) %=>% as.numeric;
			ctl <- as.vector(normal[cid, ]) %=>% as.numeric;
			avgFC <- mean(exp) / mean(ctl);
			log2FC <- log(avgFC, 2);
				
			if (all((exp == exp[1]) | (exp <= 1E-10)) && all((ctl == ctl[1]) | (ctl <= 1E-10))) {	
				p.value <- 0;
			} else {			
				tryCatch({
					p.value <- t.test(exp, ctl, var.equal = TRUE)$p.value;
				}, error=  function(e) {
					print(exp);
					print(ctl);
					stop(e)
				})
			}
						
			list(foldchange = avgFC, log2FC = log2FC, p.value = p.value);
		});
		
		title <- sprintf("[%s] vs normal", groupName);
		FC <- sapply(EG_DEM, function(r) r$foldchange) %=>% as.vector;
		log2FC <- sapply(EG_DEM, function(r) r$log2FC) %=>% as.vector;
		p.value <- sapply(EG_DEM, function(r) r$p.value) %=>% as.vector;
		group.DEM <- data.frame(FC =FC, log2FC = log2FC, p.value = p.value);
		rownames(group.DEM) <- rownames(EG_group);
		colnames(group.DEM) <- c(title, sprintf("%s log2FC", title), sprintf("%s p.value", title) );
		
		result <- cbind(result, group.DEM);
	}
	
	result;
}