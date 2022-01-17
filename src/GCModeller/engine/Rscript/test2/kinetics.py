import GCModeller

from vcellkit import modeller

result = kinetics("(Vmax * S) / (Km + S)", Vmax = 10, S = "s", Km = 2).kinetics_lambda().eval_lambda(s = 5)

print(`result value of target kinetics is ${result}`)
