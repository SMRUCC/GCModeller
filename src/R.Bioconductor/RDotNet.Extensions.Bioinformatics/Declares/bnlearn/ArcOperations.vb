Imports RDotNet.Extensions.VisualBasic.Services.ScriptBuilder
Imports RDotNet.Extensions.VisualBasic.Services.ScriptBuilder.RTypes

Namespace declares.bnlearn

    ''' <summary>
    ''' The set.arc function operates in the following way:
    ''' 
    ''' If there Is no arc between from And To, the arc from -> To Is added.
    ''' If there Is an undirected arc between from And To, its direction Is Set To from -> To.
    ''' If the arc To -> from Is present, it Is reversed.
    ''' If the arc from -> To Is present, no action Is taken.
    ''' </summary>
    <RFunc("set.arc")> Public Class setArc : Inherits bnlearnBase
        Public Property x As RExpression
        Public Property from As RExpression
        Public Property [to] As RExpression
        <Parameter("check.cycles")> Public Property checkCycles As Boolean = True
        Public Property debug As Boolean = False
    End Class

    ''' <summary>
    ''' The drop.arc function operates in the following way:
    '''
    ''' If there Is no arc between from And To, no action Is taken.
    ''' If there Is a directed Or an undirected arc between from And To, it Is dropped regardless Of its direction.
    ''' </summary>
    <RFunc("drop.arc")> Public Class dropArc : Inherits bnlearnBase
        Public Property x As RExpression
        Public Property from As RExpression
        Public Property [to] As RExpression
        Public Property debug As Boolean = False
    End Class

    ''' <summary>
    ''' The reverse.arc function operates in the following way:
    '''
    ''' If there Is no arc between from And To, it returns an Error.
    ''' If there Is an undirected arc between from And To, it returns an Error.
    ''' If the arc To -> from Is present, it Is reversed.
    ''' If the arc from -> To Is present, it Is reversed.
    ''' </summary>
    <RFunc("reverse.arc")> Public Class reverseArc : Inherits bnlearnBase
        Public Property x As RExpression
        Public Property from As RExpression
        Public Property [to] As RExpression
        <Parameter("check.cycles")> Public Property checkCycles As Boolean = True
        Public Property debug As Boolean = False
    End Class

    ''' <summary>
    ''' The set.edge function operates in the following way:
    '''
    ''' If there Is no arc between from And To, the undirected arc from - To Is added.
    ''' If there Is an undirected arc between from And To, no action Is taken.
    ''' If either the arc from -> To Or the arc To -> from are present, they are replaced With the undirected arc from - To.
    ''' </summary>
    <RFunc("set.edge")> Public Class setEdge : Inherits bnlearnBase
        Public Property x As RExpression
        Public Property from As RExpression
        Public Property [to] As RExpression
        <Parameter("check.cycles")> Public Property checkCycles As Boolean = True
        Public Property debug As Boolean = False
    End Class

    ''' <summary>
    ''' The drop.edge function operates in the following way:
    '''
    ''' If there Is no undirected arc between from And To, no action Is taken.
    ''' If there Is an undirected arc between from And To, it Is removed.
    ''' If there Is a directed arc between from And To, no action Is taken.
    ''' </summary>
    <RFunc("drop.edge")> Public Class dropEdge : Inherits bnlearnBase
        Public Property x As RExpression
        Public Property from As RExpression
        Public Property [to] As RExpression
        Public Property debug As Boolean = False
    End Class
End Namespace