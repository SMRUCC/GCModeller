Namespace LevenbergMarquardt

    ' Created by duy on 14/4/15.

    ''' <summary>
    ''' LmDatumError is an interface for evaluating error, Jacobian matrix and
    ''' Hessian matrix of a single piece of observed data
    ''' </summary>
    Public Interface LmDatumError
        ''' <summary>
        ''' Gets the total number of observed data
        ''' </summary>
        ''' <returns> An integer which is the number of observed data </returns>
        ReadOnly Property NumData As Integer

        ''' <summary>
        ''' Evaluates value of the error function for the k-th observed data that
        ''' corresponds to the parameter vector
        ''' </summary>
        ''' <paramname="dataIdx"> The index of the input data </param>
        ''' <paramname="optParams"> A vector of real values of parameters in the model
        ''' @return </param>
        Function eval(dataIdx As Integer, optParams As Double()) As Double

        ''' <summary>
        ''' Evaluates the Jacobian vector of the error function for the k-th observed
        ''' data that corresponds to the parameter vector
        ''' </summary>
        ''' <paramname="dataIdx"> The index of the input data </param>
        ''' <paramname="optParams"> A vector of real values of parameters in the model
        ''' @return </param>
        Function jacobian(dataIdx As Integer, optParams As Double()) As Double()

        ''' <summary>
        ''' Evaluates the Hessian matrix of the error function for the k-th observed
        ''' data that corresponds to the parameter vector
        ''' </summary>
        ''' <paramname="dataIdx"> The index of the input data </param>
        ''' <paramname="optParams"> A vector of real values of parameters in the model </param>
        ''' <paramname="approxHessianFlg"> A boolean flag to indicate whether the Hessian
        '''                         matrix can be approximated instead of having to be
        '''                         computed exactly
        ''' @return </param>
        Function hessian(dataIdx As Integer, optParams As Double(), approxHessianFlg As Boolean) As Double()()

        ''' <summary>
        ''' Evaluates the Hessian matrix of the error function for the k-th observed
        ''' data that corresponds to the parameter vector. The Hessian matrix is
        ''' computed exactly
        ''' </summary>
        ''' <paramname="dataIdx"> </param>
        ''' <paramname="optParams">
        ''' @return </param>
        Friend Function hessian(dataIdx As Integer, optParams As Double()) As Double()()
        Return hessian(dataIdx, optParams, False)
        End Function
    End Interface

End Namespace
