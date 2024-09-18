Namespace LevenbergMarquardt
    ''' <summary>
    ''' Created by duy on 20/1/15.
    ''' </summary>
    Public Interface LmModelError

        ''' <summary>
        ''' Evaluates the error function with input optimization parameter values
        ''' </summary>
        ''' <param name="optParams"> A vector of real values of parameters used in optimizing
        '''                  the error function </param>
        ''' <returns> Double value of the error function </returns>
        Function eval(optParams As Double()) As Double

        ''' <summary>
        ''' Computes the Jacobian vector of the error function with input
        ''' optimization parameter values
        ''' </summary>
        ''' <param name="optParams"> A vector of real values of parameters used in optimizing
        '''                  the error function </param>
        ''' <returns> Jacobian vector of the error function </returns>
        Function jacobian(optParams As Double()) As Double()

        ''' <summary>
        ''' Computes the Hessian matrix of the error function with input
        ''' optimization parameter values
        ''' </summary>
        ''' <param name="optParams"> A vector of real values of parameters used in optimizing
        '''                  the error function </param>
        ''' <param name="approxHessianFlg"> A boolean flag to indicate whether the Hessian
        '''                         matrix can be approximated instead of having to be
        '''                         computed exactly </param>
        ''' <returns> Hessian matrix of the error function </returns>
        Function hessian(optParams As Double(), approxHessianFlg As Boolean) As Double()()

        ''' <summary>
        ''' Computes the Hessian matrix of the error function with input
        ''' optimization parameter values
        ''' </summary>
        ''' <param name="optParams"> A vector of real values of parameters used in optimizing
        '''                  the error function </param>
        ''' <returns> The exact Hessian matrix of the error function </returns>
        Friend Function hessian(optParams As Double()) As Double()()
        Return hessian(optParams, False)
        End Function
    End Interface

End Namespace
