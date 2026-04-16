import GCModeller
import ggplot

from vcellkit import modeller

def eval(s_content):

    return kinetics("(Vmax * S) / (Km + S)", Vmax = 10, S = "s", Km = 2).kinetics_lambda().eval_lambda(s = s_content)

contents = []
rate     = []

for target in 0:60 step 0.5:
    result   = eval(target)
    contents = append(contents, target)
    rate     = append(rate, result)

    if target % 10 == 0:
        print(`target '${target}' kinetics is ${toString(result, format = "F4")}`)

data = data.frame(contents = contents, kinetics_rate = rate)

cat("\n\n")
print(data, max.print = 10)

bitmap(file = `${@dir}/kinetics_rate.png`):

    plt = ggplot(data, aes(x = "contents", y = "kinetics_rate"), padding = "padding: 100px 1024px 200px 250px;", width = 3600, height = 1440) 
    plt = plt + geom_line(width = 10, show.legend = TRUE, color = "Jet")
    plt
