imports "debugger" from "vcellkit";

const network = [

	rxn1 -> a + b = 2*c + d,
	rxn2 -> a + d =  3*b,
	rxn3 -> c = a + d,
	rxn4 -> c = 2*d,
	rxn5 -> c = e + 2 * f,
	rxn6 -> f = a + d,
	rxn7 -> e = 2* b

];

using mass as auto(new dataset.driver(), `${dirname(@script)}/matrix/mass.csv`) {
	using flux as auto(new dataset.driver(), `${dirname(@script)}/matrix/flux.csv`) {
		network 
		:> test_network(a = 100, b = 1, c = 800, d = 1000) 
		:> flux.dynamics()
		:> flux.load_driver(
			mass = mass,
			flux = flux
		)
		:> as.object 
		:> do.call("Run")
		;	
	}
}


