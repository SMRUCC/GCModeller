require(JSON);

options(strict = FALSE);

data = json_decode(readText(`${@dir}/HR2MSI mouse urinary bladder S096_top3.json`));

str(data);

pixels = (data$theta$expression) 
|> sapply(i -> i$geneID) 
|> strsplit(',', fixed = TRUE)
;

str(pixels);

tags = sapply(data$theta$expression, i -> which.max(i$experiments));

str(tags);

x = as.numeric(sapply(pixels, i -> i[1]));
y = as.numeric(sapply(pixels, i -> i[2]));

bitmap(file = `${@dir}/pixels.png`) {	
	plot(x, y, 
		class = `topic_${tags}`, 
		reverse = TRUE, 
		colorSet = "paper", 
		shape = "square",
		point.size = 23, 
		grid.fill = "white"
	);
}