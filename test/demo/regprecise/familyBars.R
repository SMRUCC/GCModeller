require(plot.charts);

let regulations <- read.csv("K:\20200226\TRN\genomics\search\regulations.csv");
let family = regulations[, "family"] 
:> as.character 
:> groupBy(name -> name) 
:> orderBy(group -> length(group), desc = TRUE)
:> lapply(group -> length(group), names = group -> group$key)
;

str(family);

family
:> as.numeric
:> plot
:> save.graphics(file = "K:\20200226\TRN\genomics\search\regulations.png")
;