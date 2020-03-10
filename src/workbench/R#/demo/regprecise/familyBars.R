let regulations <- read.csv("S:\2020\union\1.TRN\regulations.csv");
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
:> save.graphics(file = "S:\2020\union\1.TRN\regulation_family.png")
;