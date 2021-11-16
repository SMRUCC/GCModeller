require(charts);

const colorSet = list(
	x1 = "blue", 
	x2 = "red", 
	x3 = "green", 
	x4 = "gray"
)
;

bitmap(file = `${dirname(@script)}/atkinson.png`) {
	const result_table   <- read.csv(`${dirname(@script)}/atkinson.csv`, check_names = FALSE);
	const time as double <- result_table[, "#time"] 
	:> as.numeric
	;

	str(result_table);

	names(colorSet)
	|> sapply(function(name) {
		time |> serial(as.numeric(result_table[, name]), name, colorSet[[name]]);
	})
	|> as.vector
	|> plot(
		line         = TRUE, 
		padding      = "padding: 200px 150px 250px 250px;", 
		x.lab        = "#time", 
		y.lab        = "intensity",
		legendBgFill = "white",
		title        = "Atkinson system",
		y.format     = "G2",
		interplot    = "B_Spline"
	);
}
