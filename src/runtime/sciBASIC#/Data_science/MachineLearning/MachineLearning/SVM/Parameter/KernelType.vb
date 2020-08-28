Namespace SVM

    ''' <summary>
    ''' Contains the various kernel types this library can use.
    ''' </summary>
    Public Enum KernelType
        ''' <summary>
        ''' Linear: u'*v
        ''' </summary>
        LINEAR
        ''' <summary>
        ''' Polynomial: (gamma*u'*v + coef0)^degree
        ''' </summary>
        POLY
        ''' <summary>
        ''' Radial basis function: exp(-gamma*|u-v|^2)
        ''' </summary>
        RBF
        ''' <summary>
        ''' Sigmoid: tanh(gamma*u'*v + coef0)
        ''' </summary>
        SIGMOID
        ''' <summary>
        ''' Precomputed kernel
        ''' </summary>
        PRECOMPUTED
    End Enum
End Namespace