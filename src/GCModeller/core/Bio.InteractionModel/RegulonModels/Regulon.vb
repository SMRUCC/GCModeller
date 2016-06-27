
Namespace Regulon

    ''' <summary>
    ''' Regulon的模型的抽象
    ''' </summary>
    Public Interface IRegulon

        Property TFlocusId As String
        Property RegulatedGenes As String()

    End Interface

    ''' <summary>
    ''' 调控作用的抽象
    ''' </summary>
    Public Interface IRegulatorRegulation

        ''' <summary>
        ''' Gene id
        ''' </summary>
        ''' <returns></returns>
        Property LocusId As String
        ''' <summary>
        ''' Regulators gene id
        ''' </summary>
        ''' <returns></returns>
        Property Regulators As String()

    End Interface

    Public Class RegulatorRegulation
        Implements IRegulatorRegulation

        Public Property LocusId As String Implements IRegulatorRegulation.LocusId
        Public Property Regulators As String() Implements IRegulatorRegulation.Regulators
    End Class

    Public Interface ISpecificRegulation
        Property LocusId As String
        Property Regulator As String
    End Interface

    Public MustInherit Class Regulon : Implements IRegulon

        Public Property Id As String
        Public Property RegulatedGenes As String() Implements IRegulon.RegulatedGenes
        Public Property Regulator As String Implements IRegulon.TFlocusId
    End Class

    ''' <summary>
    ''' 用于生成调控数据的数据库的接口
    ''' </summary>
    Public Interface IRegulationDatabase
        ''' <summary>
        ''' 得到数据库之中的所有的调控因子的编号
        ''' </summary>
        ''' <returns></returns>
        Function listRegulators() As String()
        Function IsRegulates(regulator As String, site As String) As Boolean
        Function GetRegulators(site As String) As String()
        Function GetRegulatesSites(regulator As String) As String()
    End Interface

    Public Structure RelationshipScore
        ''' <summary>
        ''' 通常为Regulator
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property InteractorA As String
        ''' <summary>
        ''' 通常为目标调控对象
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property InteractorB As String
        Public Property Score As Double

        Public Function GetConnectedId(Id As String) As String
            If String.Equals(InteractorA, Id) Then
                Return InteractorB
            ElseIf String.Equals(InteractorB, Id) Then
                Return InteractorA
            Else
                Return ""
            End If
        End Function
    End Structure
End Namespace