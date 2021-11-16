imports "S.system" from "simulators";

# set y0 of the ODEs system
const symbols = list(
	x1 = -100, 
	x2 = -100000, 
	x3 = 0, 
	x4 = 10
);

using data.driver as snapshot("./atkinson.csv", symbols = names(symbols)) {
	data.driver
	|> kernel(S.script("Atkinson system"))
	|> environment(
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
	|> environment(symbols)
	|> s.system([
		x1 -> beta1 * (lamda1 * (1 + alpha1 * (x4 ^ n1) / (1 + x4 ^ n1)) - x1),
		x2 -> x1 - x2,
		x3 -> beta3 * (lamda3 * (1 + alpha2 * ((x4 / a) ^ n2) / (1 + (x4 / a) ^ n2)) * (1 / (1 + x2 ^ n3)) - x3),
		x4 -> beta4 * (x3 - x4)
	])
	|> run(ticks = 4.5, resolution = 0.1)
	;
}
