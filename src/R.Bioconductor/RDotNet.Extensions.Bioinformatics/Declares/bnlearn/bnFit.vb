Imports RDotNet.Extensions.VisualBasic.Services.ScriptBuilder
Imports RDotNet.Extensions.VisualBasic.Services.ScriptBuilder.RTypes

Namespace declares.bnlearn

    ''' <summary>
    ''' Fit the parameters of a Bayesian network conditional on its structure.
    ''' 
    ''' bn.fit fits the parameters of a Bayesian network given its structure and a data set; bn.net returns the structure underlying a fitted Bayesian network.
    ''' An in-place replacement method Is available to change the parameters of each node in a bn.fit object; see the examples for discrete, continuous And hybrid networks below. For a discrete node (class bn.fit.dnode Or bn.fit.onode), the New parameters must be in a table object. For a Gaussian node (class bn.fit.gnode), the New parameters can be defined either by an lm, glm Or pensim object (the latter Is from the penalized package) Or in a list with elements named coef, sd And optionally fitted And resid. For a conditional Gaussian node (class bn.fit.cgnode), the New parameters can be defined by a list with elements named coef, sd And optionally fitted, resid And configs. In both cases coef should contain the New regression coefficients, sd the standard deviation of the residuals, fitted the fitted values And resid the residuals. configs should contain the configurations if the discrete parents of the conditional Gaussian node, stored as a factor.
    ''' </summary>
    ''' <remarks>
    ''' bn.fit returns an object of class bn.fit, bn.net an object of class bn. See bn class and bn.fit class for details.
    ''' </remarks>
    <RFunc("bn.fit")> Public Class bnFit : Inherits bnlearnBase

        ''' <summary>
        ''' an object of class bn (for bn.fit and custom.fit) or an object of class bn.fit (for bn.net).
        ''' </summary>
        ''' <returns></returns>
        Public Property x As RExpression
        ''' <summary>
        ''' a data frame containing the variables In the model.
        ''' </summary>
        ''' <returns></returns>
        Public Property data As RExpression
        ''' <summary>
        ''' a character string, either mle for Maximum Likelihood parameter estimation or bayes for Bayesian parameter estimation (currently implemented only for discrete data).
        ''' </summary>
        ''' <returns></returns>
        Public Property method As String = "mle"
        ''' <summary>
        ''' a boolean value. If TRUE a lot of debugging output is printed; otherwise the function is completely silent.
        ''' </summary>
        ''' <returns></returns>
        Public Property debug As Boolean = False
    End Class

    ''' <summary>
    ''' custom.fit takes a set of user-specified distributions and their parameters and uses them to build a bn.fit object. 
    ''' Its purpose is to specify a Bayesian network (complete with the parameters, not only the structure) using knowledge from experts in the field instead of learning it from a data set. 
    ''' The distributions must be passed to the function in a list, with elements named after the nodes of the network structure x. 
    ''' Each element of the list must be in one of the formats described above for in-place replacement.
    ''' </summary>
    <RFunc("custom.fit")> Public Class customFit

        ''' <summary>
        ''' an object of class bn (for bn.fit and custom.fit) or an object of class bn.fit (for bn.net).
        ''' </summary>
        ''' <returns></returns>
        Public Property x As RExpression
        ''' <summary>
        ''' a named list, with element for each node of x. See below.
        ''' </summary>
        ''' <returns></returns>
        Public Property dist As RExpression
        ''' <summary>
        ''' a vector Of character strings, the labels Of the discrete nodes which should be saved As ordinal random variables (bn.fit.onode) instead Of unordered factors (bn.fit.dnode).
        ''' </summary>
        ''' <returns></returns>
        Public Property ordinal As RExpression
    End Class

    <RFunc("bn.net")> Public Class bnNet

        ''' <summary>
        ''' an object of class bn (for bn.fit and custom.fit) or an object of class bn.fit (for bn.net).
        ''' </summary>
        ''' <returns></returns>
        Public Property x As RExpression
        ''' <summary>
        ''' a boolean value. If TRUE a lot of debugging output is printed; otherwise the function is completely silent.
        ''' </summary>
        ''' <returns></returns>
        Public Property debug As Boolean = False
    End Class
End Namespace