Imports System.Collections.ObjectModel
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.SecurityString.MD5Hash

Namespace Interpreter.Linker.APIHandler

    Public Structure ParameterWithAlias
        Dim ParameterInfo As System.Reflection.ParameterInfo, [Alias] As Parameter

        Public ReadOnly Property ParameterType As Type
            Get
                Return ParameterInfo.ParameterType
            End Get
        End Property

        Sub New(ParameterInfo As System.Reflection.ParameterInfo, [Alias] As Parameter)
            Me.ParameterInfo = ParameterInfo
            Me.Alias = If([Alias] Is Nothing, New Parameter(ParameterInfo.Name.ToLower), [Alias])
        End Sub

        Public Overrides Function ToString() As String
            If String.IsNullOrEmpty([Alias].Description) Then
                Return [Alias].Alias
            Else
                Return [Alias].Alias & ": " & [Alias].Description
            End If
        End Function
    End Structure
End Namespace