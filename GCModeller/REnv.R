# demo script for build REnv unix man pages.

setwd(!script$dir);

let REnv = ls("REnv");

for(category in names(REnv)) {
	for(ref in REnv[[category]]) {
		man(get(ref)) :> writeLines(con = `./man/REnv/${category}/${ref}.1`);
	}
}