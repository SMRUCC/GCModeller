Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter

Namespace Runtime.MMU

    Public Interface IPageUnit : Inherits IAddressOf

        ReadOnly Property PageType As IPAGE_TYPES
        ReadOnly Property Name As String
        Property Value As Object
        ReadOnly Property Type As String
        ReadOnly Property [ReadOnly] As Boolean
        ''' <summary>
        ''' 在使用变量申明语句的时候的注释信息
        ''' </summary>
        ''' <returns></returns>
        Property [REM] As String
        ''' <summary>
        ''' 得到变量的值的当前的数据类型
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property [TypeOf] As Type
        ''' <summary>
        ''' 详细信息
        ''' </summary>
        ''' <returns></returns>
        Function View() As String
    End Interface

    Public Enum IPAGE_TYPES
        PageMapping
        ''' <summary>
        ''' Variable
        ''' </summary>
        MMU
        ''' <summary>
        ''' Constant
        ''' </summary>
        SMMU
    End Enum

    Public Module PageUnitView

        ''' <summary>
        ''' 详细信息
        ''' </summary>
        ''' <returns></returns>
        Public Function View(PageUnit As MMU.IPageUnit) As String
            Dim sbr As StringBuilder = New StringBuilder(1024)

            Call sbr.AppendLine($"{If(PageUnit.[ReadOnly], "[ReadOnly] ", "")}({NameOf(PageUnit.Address)}-> &{PageUnit.Address})")
            Call sbr.AppendLine($"{NameOf(PageUnit.Name)}:=  {PageUnit.Name}{vbTab}{vbTab}{PageUnit.[REM]}")
            Call sbr.AppendLine($"{NameOf(PageUnit.Type)}:=  {PageUnit.Type}")
            Call sbr.AppendLine()

            If PageUnit.Value() Is Nothing Then
                Call sbr.AppendLine("  = null")
                Return sbr.ToString
            End If

            Dim type As Type = PageUnit.Value.GetType
            Dim array As Object()

            If System.Array.IndexOf(type.GetInterfaces, GetType(System.Collections.IEnumerable)) > -1 Then
                array = InputHandler.CastArray(Of Object)(PageUnit.Value)
            Else
                array = {PageUnit.Value()}
            End If

            Call sbr.AppendLine("  =")

            For i As Integer = 0 To array.Length - 1
                Call sbr.AppendLine($"    [{i}] { InputHandler.ToString(array(i))}")
            Next

            Return sbr.ToString
        End Function
    End Module
End Namespace