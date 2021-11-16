imports "S.system" from "simulators";

const symbols = list(
	x1 = 2,
	x2 = 0.1,
	x3 = 0.5,
	x4 = 1
);

using data.driver as snapshot("./example2.csv", symbols = names(symbols)) {
	data.driver
	|> kernel(S.script("Example2"))
	|> environment(symbols)
	|> s.system([
		x1 -> 2 * x2 - 1.2 * (x1 ^ 0.5) * (x3 ^ (-1)),
		x2 -> 2 * (x1 ^ 0.1) * (x3 ^ (-1)) * (x4 ^ 0.5) - (2 * x2),
		x3 -> 0.5,
		x4 -> 1
	])
	|> run(ticks = 10, resolution = 0.25)
	;
}