# R.Bioinformatics
R language bioinformatics analysis package wrapper for VisualBasic.

This project is aim at provides a high performance distribution and parallel computing environment for bioinformatics data analysis of VisualBasic hybrid programming with R.


##### Runtime

Require of VisualBasic server CLI runtime

> PM> Install-Package VB_AppFramework

Or reference to source code project:

> https://github.com/xieguigang/VisualBasic_AppFramework

Folks from project:
R.NET   [https://rdotnet.codeplex.com/](https://rdotnet.codeplex.com/)

Currently there is a bioconductor GUI installer was included in this project for those beginners in the area of bioinformatics and some of the common used R package wrapper class which written in VisualBasic was developed to extends the biological data analysis ability of GCModeller and Microsoft .NET languages.

![](https://raw.githubusercontent.com/SMRUCC/R.Bioinformatics/master/Bioconductor/bioconductor_logo_rgb.jpg)
![](https://github.com/SMRUCC/R.Bioinformatics/blob/master/Bioconductor/screenshot.png?raw=true)
![](https://raw.githubusercontent.com/SMRUCC/R.Bioinformatics/master/20160312032449.png)

## VisualBasic & R language hybrids

For example:

```vbnet
Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.API.base
Imports RDotNET.Extensions.VisualBasic.API.utils
Imports RDotNET.Extensions.Bioinformatics.deSolve.API
Imports RDotNET.Extensions.Bioinformatics.deSolve

' Setup variables in R language from VisualBasic
Call $"p      <- {p}".丶
Call $"beta   <- {beta}".丶
Call $"c      <- {c}".丶
Call $"delta  <- {delta}".丶
Call $"U0     <- {U0}".丶
Call $"rho    <- {rho}".丶
Call $"lambda <- {lambda}".丶

' The initial value for infected cells(I0) is set to zero.
' The best model based on the Akaike Information Criterion(AIC) is presented in Figure3, providing an estimate of 9 ffu/ml for V0.
' The initial number of susceptible cells(U0) can be taken from the experiment in Half mannetal. (2008) as 5 × 10^5.
Dim yini = "c(U=U0, I=0, V=9)"
Dim model = [function](
    {
        "t",
        "y",
        "params"
    },
_
       "with(as.list(y), {

        dU <- lambda - rho * U - beta * U * V
        dI <- beta * U * V - delta * I
        dV <- p * I - c * V

        list(c(dU, dI, dV))
})")

require("deSolve")

' Calling ode{deSolve}
Dim times = seq([from]:=0, [to]:=5.6, by:=0.01)
Dim out = ode(y:=yini,
              times:=times,
              func:=model,
              parms:=NULL,
              method:=integrator.rk4)

' Calling write.csv{utils}
Call write.csv(out, "x:\ebola_test.csv")
```

