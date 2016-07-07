---
title: bnFit
---

# bnFit
_namespace: [RDotNet.Extensions.Bioinformatics.declares.bnlearn](N-RDotNet.Extensions.Bioinformatics.declares.bnlearn.html)_

Fit the parameters of a Bayesian network conditional on its structure.
 
 bn.fit fits the parameters of a Bayesian network given its structure and a data set; bn.net returns the structure underlying a fitted Bayesian network.
 An in-place replacement method Is available to change the parameters of each node in a bn.fit object; see the examples for discrete, continuous And hybrid networks below. For a discrete node (class bn.fit.dnode Or bn.fit.onode), the New parameters must be in a table object. For a Gaussian node (class bn.fit.gnode), the New parameters can be defined either by an lm, glm Or pensim object (the latter Is from the penalized package) Or in a list with elements named coef, sd And optionally fitted And resid. For a conditional Gaussian node (class bn.fit.cgnode), the New parameters can be defined by a list with elements named coef, sd And optionally fitted, resid And configs. In both cases coef should contain the New regression coefficients, sd the standard deviation of the residuals, fitted the fitted values And resid the residuals. configs should contain the configurations if the discrete parents of the conditional Gaussian node, stored as a factor.

> 
>  bn.fit returns an object of class bn.fit, bn.net an object of class bn. See bn class and bn.fit class for details.
>  



### Properties

#### data
a data frame containing the variables In the model.
#### debug
a boolean value. If TRUE a lot of debugging output is printed; otherwise the function is completely silent.
#### method
a character string, either mle for Maximum Likelihood parameter estimation or bayes for Bayesian parameter estimation (currently implemented only for discrete data).
#### x
an object of class bn (for bn.fit and custom.fit) or an object of class bn.fit (for bn.net).
