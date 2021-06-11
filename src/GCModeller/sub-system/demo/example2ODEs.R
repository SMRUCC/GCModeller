[
	x1 -> 2 * x2 - 1.2 * (x1 ^ 0.5) * (x3 ^ (-1)),
	x2 -> 2 * (x1 ^ 0.1) * (x3 ^ (-1)) * (x4 ^ 0.5) - (2 * x2),
	x3 -> 0.5,
	x4 -> 1
]
|> deSolve(
    y0 = list(
		x1 = 2, 
		x2 = 0.1, 
		x3 = 0.5, 
		x4 = 1
	),
    a          = 0,
    b          = 10,
	resolution = 20000
)
|> as.data.frame
|> write.csv(file = `${dirname(@script)}/example2.csv`)
;