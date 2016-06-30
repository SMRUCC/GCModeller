Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.HybridsScripting
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.ShoalShell
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq.Extensions

<[PackageNamespace]("VBMath",
                    Description:="This package enable Shoal language can be doing the mathematics calculation.",
                    Publisher:="xie.guigang@gmail.com")>
<LanguageEntryPoint("VBMath", "Language feature extension of the Shoal Language.")>
Public Module API

    <EntryInterface(InterfaceTypes.EntryPointInit)>
    Public Function Initialize() As Boolean
        'Do not needs any initialization 
        Return True
    End Function

    <ExportAPI("Evaluate")>
    <EntryInterface(InterfaceTypes.Evaluate)>
    Public Function Evaluate(Script As String) As Object
        Return ScriptEngine.Shell(Script)
    End Function

    <ExportAPI("Set.Variable")>
    <EntryInterface(InterfaceTypes.SetValue)>
    Public Function SetValue(var As String, value As Object) As Boolean
        Try
            Call Mathematical.ScriptEngine.SetVariable(var, value)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    <ExportAPI("Ranks")>
    Public Function Ranks(data As Generic.IEnumerable(Of Double), Optional Log2Rank As Boolean = False, Optional Level As Integer = 100) As Integer()
        If Log2Rank Then
            Return data.Log2Ranks(Level).ToArray(Function(n) CInt(n))
        Else
            Return data.GenerateMapping(Level)
        End If
    End Function

    <ExportAPI("Load.Vector")>
    Public Function LoadVector(path As String) As Double()
        Dim LQuery = (From line As String In IO.File.ReadAllLines(path) Select Val(line)).ToArray
        Return LQuery
    End Function
End Module
