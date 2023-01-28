setwd(@dir);

data = read.csv("../all_counts.csv", row.names = 1, check.names = FALSE);

for(sample in colnames(data)) {
	v = data[, sample];
	v = v / sum(v) * [10 ^8];
	v[v == 0.0] = min(v[v > 0]) / 2;
	data[,sample] = v;
}

write.csv(data, file = "./ath_norm.csv", row.names = TRUE);
writeLines(rownames(data), con = "./geneIds.txt");