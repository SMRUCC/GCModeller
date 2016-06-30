
''' <summary>
''' The very base type of a network simulator. (一个IterationMathEngine类型的对象之中可能会存在多个反应机器类型的子模块)
''' </summary>
''' <typeparam name="DataType"></typeparam>
''' <typeparam name="TExpr"></typeparam>
''' <remarks></remarks>
Public MustInherit Class ReactorMachine(Of DataType, TExpr As IDynamicsExpression(Of DataType))
    Implements IReactorMachine

#Region "Public Property & Fields"

    ''' <summary>
    ''' The network entity that using for the system behaviour simulation.(所需要被进行模拟计算的网络对象实体)
    ''' </summary>
    ''' <remarks></remarks>
    <DumpNode> Protected _DynamicsExprs As TExpr()
    <DumpNode> Public Property IterationLoops As Integer Implements IReactorMachine.IterationCycle
    <DumpNode> MustOverride ReadOnly Property RuntimeTicks As Long
#End Region

#Region "Public Methods"

    Public MustOverride Function Initialize() As Integer Implements IReactorMachine.Initialize
    Protected MustOverride Function __innerTicks(KernelCycle As Integer) As Integer

    Public Overridable Function TICK() As Integer Implements IReactorMachine.TICK
        Return __innerTicks(RuntimeTicks)
    End Function

    Friend Function get_Expressions() As TExpr()
        Return _DynamicsExprs
    End Function
#End Region

End Class

''' <summary>
''' 生化反应器的接口
''' </summary>
Public Interface IReactorMachine

#Region "Public Property"

    ''' <summary>
    ''' The total kernel loop for this simulation experiment.(总的内核循环次数)
    ''' </summary>
    ''' <remarks></remarks>
    Property IterationCycle As Integer
#End Region

#Region "Public Methods"

    Function Initialize() As Integer
    Function TICK() As Integer
#End Region

End Interface
