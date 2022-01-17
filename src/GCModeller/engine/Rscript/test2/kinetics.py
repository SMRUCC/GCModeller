import GCModeller
import ggplot

from vcellkit import modeller

def eval(s_content):

    return kinetics("(Vmax * S) / (Km + S)", Vmax = 10, S = "s", Km = 2).kinetics_lambda().eval_lambda(s = s_content)

contents = []
rate = []

for target in 0:30 step 0.25:
    result = eval(target)
    contents = append(contents, target)
    rate = append(rate, result)

    if target % 6 == 0:
        print(`target '${target}' kinetics is ${toString(result, format = "F4")}`)

data = data.frame(contents = contents, kinetics_rate = rate)

cat("\n\n")

print(data, max.print = 10)

bitmap(file = `${@dir}/kinetics_rate.png`, size = [1600,1200]):

    