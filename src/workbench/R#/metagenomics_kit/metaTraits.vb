Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.metaTraits
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.TraitarVB
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("metaTraits")>
Module metaTraitsTool

    <ExportAPI("load.meta_traits")>
    <RApiReturn(GetType(metaTraitData))>
    Public Function load_metatraits(file As String, Optional env As Environment = Nothing) As Object
        Return TraitAnnotation.CreateProfiles(TraitAnnotation.ParseTable(file)).ToArray
    End Function

    <ExportAPI("load.trait_models")>
    Public Function load_traitModels(repo As String) As ModelLoader
        Dim modelLoader As ModelLoader = New ModelLoader(repo).LoadAll()
        Console.WriteLine("表型数: " & modelLoader.PhenotypeCount)
        Return modelLoader
    End Function
End Module
