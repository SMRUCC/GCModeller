# API
_namespace: [RDotNET.Extensions.Bioinformatics.deSolve](./index.md)_

Example:
 
 ```R
 ## =======================================================================
 ## Example2: Substrate-Producer-Consumer Lotka-Volterra model
 ## =======================================================================

 ## Note:
 ## Function sigimp passed as an argument (input) to model
 ## (see also lsoda And rk examples)

 SPCmod <- function(t, x, parms, input) {
 With (as.list(c(parms, x)), {
 import <- input(t)
 dS <- import - b*S*P + g*C # substrate
 dP <- c*S*P - d*C*P # producer
 dC <- e*P*C - f*C # consumer
 res <- c(dS, dP, dC)
 list(res)
 })
 }

 ## The parameters 
 parms <- c(b = 0.001, c = 0.1, d = 0.1, e = 0.1, f = 0.1, g = 0.0)

 ## vector of timesteps
 times <- seq(0, 200, length = 101)

 ## external signal with rectangle impulse
 signal <- data.frame(times = times, import = rep(0, length(times)))

 signal$import[signal$times >= 10 & signal$times <= 11] <- 0.2
 sigimp <- approxfun(signal$times, signal$import, rule = 2)

 ## Start values for steady state
 xstart <- c(S = 1, P = 1, C = 1)

 ## Solve model
 out <- ode(y = xstart, times = times,
 func = SPCmod, parms = parms, input = sigimp)

 ## Default plot method
 plot(out)

 ## User specified plotting
 mf <- par(mfrow = c(1, 2))
 matplot(out[,1], out[,2:4], type = "l", xlab = "time", ylab = "state")
 legend("topright", col = 1:3, lty = 1:3, legend = c("S", "P", "C"))
 plot(out[,"P"], out[,"C"], type = "l", lwd = 2, xlab = "producer",
 ylab = "consumer")
 par(mfrow = mf)
 ```



### Methods

#### ode
```csharp
RDotNET.Extensions.Bioinformatics.deSolve.API.ode(System.String,System.String,System.String,System.String,System.String,System.String[])
```
General Solver for Ordinary Differential Equations
 Solves a system of ordinary differential equations; a wrapper around the implemented ODE solvers

|Parameter Name|Remarks|
|--------------|-------|
|y|the initial (state) values for the ODE system, a vector. If y has a name attribute, the names will be used to label the output matrix|
|times|time sequence for which output is wanted; the first value of times must be the initial time.|
|func|either an R-function that computes the values of the derivatives in the ODE system (the model definition) at time t, or a character string giving the name of a compiled function in a dynamically loaded shared library.
 If func Is an R-Function, it must be defined As: func <- function(t, y, parms,...). t Is the current time point in the integration, y Is the current estimate of the variables in the ODE system. If the initial values y has a names attribute, the names will be available inside func. parms Is a vector Or list of parameters; ... (optional) are any other arguments passed to the function.
 The return value of func should be a list, whose first element Is a vector containing the derivatives of y with respect to time, And whose next elements are global values that are required at each point in times. The derivatives must be specified in the same order as the state variables y.
 If func Is a String, Then dllname must give the name Of the Shared library (without extension) which must be loaded before ode Is called. See package vignette "compiledCode" For more details.|
|parms|parameters passed to func.|
|method|the integrator to use, either a function that performs integration, or a list of class rkMethod, or a string ("lsoda", "lsode", "lsodes","lsodar","vode", "daspk", "euler", "rk4", "ode23", "ode45", "radau", "bdf", "bdf_d", "adams", "impAdams" or "impAdams_d" ,"iteration"). Options "bdf", "bdf_d", "adams", "impAdams" or "impAdams_d" are the backward differentiation formula, the BDF with diagonal representation of the Jacobian, the (explicit) Adams and the implicit Adams method, and the implicit Adams method with diagonal representation of the Jacobian respectively (see details). The default integrator used is lsoda.
 Method "iteration" Is special in that here the function func should return the New value of the state variables rather than the rate of change. This can be used for individual based models, for difference equations, Or in those cases where the integration Is performed within func). See last example.|
|additionals|additional arguments passed to the integrator or to the methods.|


_returns: 
 A matrix of class deSolve with up to as many rows as elements in times and as many columns as elements in y plus the number of "global" values 
 returned in the second element of the return from func, plus an additional column (the first) for the time value. There will be one row for each 
 element in times unless the integrator returns with an unrecoverable error. If y has a names attribute, it will be used to label the columns of 
 the output value._
> 
>  This is simply a wrapper around the various ode solvers.
>  See package vignette For information about specifying the model In compiled code.
>  See the selected integrator For the additional options.
>  The Default integrator used Is lsoda.
>  The option method = "bdf" provdes a handle to the backward differentiation formula (it Is equal to using method = "lsode"). It Is best suited to solve stiff (systems of) equations.
>  The option method = "bdf_d" selects the backward differentiation formula that uses Jacobi-Newton iteration (neglecting the off-diagonal elements of the Jacobian (it Is equal to using method = "lsode", mf = 23). It Is best suited to solve stiff (systems of) equations.
>  method = "adams" triggers the Adams method that uses functional iteration (no Jacobian used); (equal to method = "lsode", mf = 10. It Is often the best choice for solving non-stiff (systems of) equations. Note when functional iteration Is used, the method Is often said to be explicit, although it Is in fact implicit.
>  method = "impAdams" selects the implicit Adams method that uses Newton- Raphson iteration (equal to method = "lsode", mf = 12.
>  method = "impAdams_d" selects the implicit Adams method that uses Jacobi- Newton iteration, i.e. neglecting all off-diagonal elements (equal to method = "lsode", mf = 13.
>  For very stiff systems, method = "daspk" may outperform method = "bdf".
>  
>  ###### Example
>  
>  ```R
>  a <- -8/3 ; b <- -10; c <- 28
>  yini <- c(X = 1, Y = 1, Z = 1)
>  Lorenz <- function (t, y, parms) {
>     with(as.list(y), {
>        dX <- a * X + Y * Z
>        dY <- b * (Y - Z)
>        dZ <- -X * Y + c * Y - Z
>        list(c(dX, dY, dZ))
>     })
>  }
>  times <- seq(from = 0, to = 100, by = 0.01)
>  out <- ode(y = yini, times = times, func = Lorenz, parms = NULL)
>  plot(out, lwd = 2)
>  plot(out[,"X"], out[,"Y"], type = "l", xlab = "X",
>       ylab = "Y", main = "butterfly")
>  ```
>  


