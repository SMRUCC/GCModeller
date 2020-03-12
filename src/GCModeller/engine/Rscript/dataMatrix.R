imports "vcellkit.rawXML" from "vcellkit.dll";

list.files("S:\2020\union\3.VCell\result\raw", pattern = "*.vcXML")
:> frame.matrix(tick = 499, metabolome = "mass_profile")
:> write.csv(file = "S:\2020\union\3.VCell\result\samples.csv")
;