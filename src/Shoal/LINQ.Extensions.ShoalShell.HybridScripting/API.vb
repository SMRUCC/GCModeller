Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.HybridsScripting
Imports Microsoft.VisualBasic.Scripting.ShoalShell
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime
Imports Microsoft.VisualBasic.LINQ.Script

<HybridsScripting.LanguageEntryPoint("LINQ", "SQL like query scripting language for the object oriented database.")>
<[Namespace]("LINQ", Description:="SQL like query scripting language for the object oriented database.")>
Public Module API

    Dim LINQ As DynamicsRuntime

    <ExportAPI("__Init()")>
    <HybridsScripting.EntryInterface(EntryInterface.InterfaceTypes.EntryPointInit)>
    Public Function Initialize() As Boolean
        API.LINQ = New DynamicsRuntime
        Return True
    End Function

    <ExportAPI("EValuate")>
    <HybridsScripting.EntryInterface(EntryInterface.InterfaceTypes.Evaluate)>
    Public Function Evaluate(script As String) As Object
        Return LINQ.Evaluate(script)
    End Function

    <ExportAPI("SetValue")>
    <HybridsScripting.EntryInterface(EntryInterface.InterfaceTypes.SetValue)>
    Public Function SetValue(var As String, value As Object) As Boolean
        If TypeOf value Is IEnumerable Then
            Call LINQ.SetObject(var, DirectCast(value, IEnumerable))
        End If
        Return True
    End Function
End Module
