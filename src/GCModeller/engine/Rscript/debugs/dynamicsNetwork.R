imports "debugger" from "vcellkit";

const network = [

	rxn1 -> a + b = c + d,
	rxn2 -> a + d =  3*b

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


