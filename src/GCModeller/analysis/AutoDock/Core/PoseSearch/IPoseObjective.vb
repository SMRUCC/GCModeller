Namespace Core

    ''' <summary>BFGS 目标函数适配器</summary>
    Public Interface IPoseObjective

        ''' <summary>评估能量与梯度；grads() 就地填充；返回能量</summary>
        Function Evaluate(trans() As Double, rotvec() As Double, torsions() As Double,
                          grads() As Double, rigidCenter() As Double) As Double

        ReadOnly Property NumTorsions As Int32

    End Interface
End Namespace