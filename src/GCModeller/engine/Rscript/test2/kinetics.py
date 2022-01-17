import GCModeller

from vcellkit import modeller

def eval(s_content):

    return kinetics("(Vmax * S) / (Km + S)", Vmax = 10, S = "s", Km = 2).kinetics_lambda().eval_lambda(s = s_content)


for target in 0:10 step 0.25:
    result = eval(target)

    print(`target [${target}] kinetics is ${result}`)



