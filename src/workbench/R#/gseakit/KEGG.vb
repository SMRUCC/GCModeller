Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.XML
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' the kegg background helper
''' </summary>
''' 
<Package("kegg")>
Module KEGG

    ''' <summary>
    ''' gt kegg compound set from a kegg pathway map collection
    ''' </summary>
    ''' <param name="maps"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("compound_set")>
    Public Function compoundSet(<RRawVectorArgument> maps As Object, Optional env As Environment = Nothing) As Object
        Dim coll = pipeline.TryCreatePipeline(Of Map)(maps, env)

        If coll.isError Then
            Return coll.getError
        End If

        Dim cpd_set As list = list.empty

        For Each map As Map In Tqdm.Wrap(coll.populates(Of Map)(env).ToArray)
            Call cpd_set.add(map.EntryId, map.GetCompoundSet.ToDictionary(Function(a) a.Name, Function(a) a.Value))
        Next

        Return cpd_set
    End Function

End Module
