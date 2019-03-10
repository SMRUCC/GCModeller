Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser.Tokens.ParameterName
Imports Microsoft.VisualBasic.Scripting.MetaData.Parameter
Imports System.Reflection

Namespace Interpreter.Linker.APIHandler.Alignment

    Public MustInherit Class SpecialAlignment(Of T)
        Protected InputParam As Dictionary(Of String, Object)
        Protected FuncDef As T

        Sub New(FuncDef As T, ByRef InputParam As Dictionary(Of String, Object))
            Me.FuncDef = FuncDef
            Me.InputParam = InputParam
        End Sub

        Public MustOverride Function OverloadsAlignment() As ParamAlignments
    End Class
End Namespace