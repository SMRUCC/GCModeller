Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.metaTraits
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("metaTraits")>
Module metaTraits

    <ExportAPI("load.meta_traits")>
    <RApiReturn(GetType(metaTraitData))>
    Public Function load_metatraits(file As String, Optional env As Environment = Nothing) As Object
        Return TraitAnnotation.CreateProfiles(TraitAnnotation.ParseTable(file)).ToArray
    End Function
End Module
