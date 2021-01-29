
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.Rsharp.Runtime.Internal.Object

<Package("regprecise")>
Public Module RegPrecise

    ''' <summary>
    ''' load regprecise database from a given file.
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <ExportAPI("read.regprecise")>
    Public Function readRegPrecise(file As String) As TranscriptionFactors
        Return file.LoadXml(Of TranscriptionFactors)
    End Function

    <ExportAPI("motif.raw")>
    Public Function exportRegPrecise(regprecise As TranscriptionFactors) As List
        Return regprecise _
            .ExportByFamily _
            .ToDictionary(Function(name) name.Key,
                          Function(family)
                              Return CObj(family.Value)
                          End Function) _
            .DoCall(Function(data)
                        Return New List With {.slots = data}
                    End Function)
    End Function
End Module
