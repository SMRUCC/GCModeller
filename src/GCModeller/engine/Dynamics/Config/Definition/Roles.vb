Imports Microsoft.VisualBasic.ComponentModel.Settings.Inf

Namespace Definitions

    ''' <summary>
    ''' Role of the general compound
    ''' </summary>
    Public Enum GeneralCompound
        Compound
        DNA
        RNA
        Protein
    End Enum

    ''' <summary>
    ''' 
    ''' </summary>
    ''' 
    <ClassName("transmembrane")>
    Public Class Transmembrane

        ''' <summary>
        ''' Defines the compound unique id set as the passive transmembrane in the simulation
        ''' </summary>
        ''' <returns>an array of the target compound unique id</returns>
        Public Property passive As String()

        ''' <summary>
        ''' 逐物质的被动通透系数(0~1)。
        ''' 
        ''' 出现在本表之中的物质即使没有列在 <see cref="passive"/> 之中，也被视为可被动通透；
        ''' 未出现在本表、同时也没有列出名称的物质默认不可被动通透(返回 0)。
        ''' </summary>
        ''' <returns></returns>
        Public Property permeability As Dictionary(Of String, Double)

        ''' <summary>
        ''' 列在 <see cref="passive"/> 之中但没有显式系数的物质所使用的缺省通透系数
        ''' </summary>
        ''' <returns></returns>
        Public Property defaultPermeability As Double = 0.2

        ''' <summary>
        ''' 某个化合物的被动通透系数：0 表示不可被动通透
        ''' </summary>
        ''' <param name="id">化合物的唯一编号</param>
        ''' <returns></returns>
        Public Function PermeabilityOf(id As String) As Double
            If id.StringEmpty(, True) Then
                Return 0.0
            End If

            Dim coefficient As Double = 0.0

            If permeability IsNot Nothing Then
                If permeability.TryGetValue(id, coefficient) Then
                    Return ClampPermeability(coefficient)
                End If
            End If

            If passive IsNot Nothing Then
                For Each item As String In passive
                    If String.Equals(item, id, StringComparison.OrdinalIgnoreCase) Then
                        Return ClampPermeability(defaultPermeability)
                    End If
                Next
            End If

            Return 0.0
        End Function

        ''' <summary>
        ''' 把通透系数钳制到 [0, 1]，并过滤 NaN
        ''' </summary>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Public Shared Function ClampPermeability(value As Double) As Double
            If Double.IsNaN(value) OrElse value <= 0.0 Then
                Return 0.0
            ElseIf value >= 1.0 Then
                Return 1.0
            Else
                Return value
            End If
        End Function

    End Class
End Namespace
