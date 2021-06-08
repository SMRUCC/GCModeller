require(charts);

result_table <- read.csv(result_table);

str(result_table);

let time as double <- result_table[, "#time"] :> as.numeric;
let lines = [];
let colorSet = list(x1 = "blue", x2 = "red", x3 = "green", x4 = "gray");

for(name in names(symbols)) {
	let y <- result_table[, name] :> as.numeric;
	let line <- serial(time, y, name, colorSet[[name]]);
	
	lines <- lines << line;
	
	print(line);
}

plot(lines, 
	line         = TRUE, 
	padding      = "padding: 200px 150px 250px 250px;", 
	x.lab        = "#time", 
	y.lab        = "intensity",
	legendBgFill = "white",
	title        = "Atkinson system",
	y.format     = "G2",
	interplot    = "B_Spline"
)
:> save.graphics(file = "./atkinson.png")
;