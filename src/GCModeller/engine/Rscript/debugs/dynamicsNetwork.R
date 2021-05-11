imports "debugger" from "vcellkit";

const network = [

	rxn1 -> a + b = c + d,
	rxn2 -> a + d =  3*b

];

network :> test_network(a = 100, b = 1, c = 800, d = 1000);
