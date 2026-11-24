Namespace Simulation

    ''' <summary>
    ''' The minimal contract of an ODE model that the circuit simulator is able
    ''' to integrate: the right hand side evaluation and the input signal.
    ''' (电路仿真器可集成的 ODE 模型最小契约：右端求值与输入信号)
    ''' </summary>
    ''' <remarks>
    ''' The three node Michaelis Menten model <see cref="Model.CircuitModel"/>
    ''' implements this interface, and the two node control network of the
    ''' self tests implements it as well, so that the very same simulator, the
    ''' very same step protocol and the very same functional criteria are
    ''' applied to both networks.
    ''' </remarks>
    Public Interface IOdeModel

        ''' <summary>
        ''' The current value of the input signal.
        ''' (当前输入信号强度)
        ''' </summary>
        Property Input As Double

        ''' <summary>
        ''' Evaluate the time derivative of the state vector.
        ''' (计算状态向量对时间的导数)
        ''' </summary>
        ''' <param name="y">the current state</param>
        ''' <param name="ydot">the output derivative, it is overwritten</param>
        Sub Evaluate(y As Double(), ydot As Double())

    End Interface

End Namespace
