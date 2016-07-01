Imports LANS.SystemsBiology.AnalysisTools.CellPhenotype.SSystem
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Public Module Compiler

    Public Delegate Function ICompiler(input As String, out As String, autoFix As Boolean) As Boolean

    ''' <summary>
    ''' 格式名称的大小写不敏感
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Compilers As IReadOnlyDictionary(Of String, ICompiler) =
        New HashDictionary(Of ICompiler) From {
 _
        {"script", AddressOf ScriptCompiler},
        {"sbml", AddressOf SBMLCompiler}
    }

    Public Function ScriptCompiler(input As String, out As String, autoFix As Boolean) As Boolean
        Return Script.ScriptCompiler.Compile(input, autoFix).Save(out)
    End Function

    Public Function SBMLCompiler(input As String, out As String, autoFix As Boolean) As Boolean
        Return SBML.Compile(input, autoFix).Save(out)
    End Function
End Module
