imports "S.system" from "simulators";
imports "plot.charts" from "R.plot";

setwd(!script$dir);

let symbols = list(x1 = -100, x2 = -100000, x3 = 0, x4 = 10);
let result_table as string = "./atkinson.csv";

using data.driver as snapshot(result_table, symbols = names(symbols)) {
	kernel(data.driver, S.script("Atkinson system"))
	:> environment(
		beta1    = 30,
		beta3    = 30,
		beta4    = 1,
		lamda1   = 2,
		lamda3   = 2,
		alpha1   = 20,
		alpha2   = 20,
		alpha3   = 1,
		a        = 1,
		n1       = 4,
		n2       = 5,
		n3       = 1,
		is.const = TRUE
	)
	:> environment(symbols)
	:> s.system([
		x1 -> beta1*(lamda1*(1+alpha1*(x4^n1)/(1+x4^n1))-x1),
		x2 -> x1-x2,
		x3 -> beta3*(lamda3*(1+alpha2*((x4/a)^n2)/(1+(x4/a)^n2))*(1/(1+x2^n3))-x3),
		x4 -> beta4*(x3-x4)
	])
	:> run(ticks = 4.5, resolution = 0.1)
	;
}

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