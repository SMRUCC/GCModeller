Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Abstract

Namespace SymbolBuilder

    Public Class ScriptBuilder : Inherits Scripting.SymbolBuilder.ScriptBuilder

        Sub New(capacity%)
            Call MyBase.New(capacity)
        End Sub

        Sub New()
            Call MyBase.New(1024)
        End Sub

        ''' <summary>
        ''' AppendLine
        ''' </summary>
        ''' <param name="sb"></param>
        ''' <param name="s"></param>
        ''' <returns></returns>
        Public Overloads Shared Operator +(sb As ScriptBuilder, s As IRToken) As ScriptBuilder
            Call sb.Script.AppendLine(s.RScript)
            Return sb
        End Operator
    End Class
End Namespace