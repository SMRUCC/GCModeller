Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.metaTraits

Module metaTraitsTest

    Sub readFiles()
        Dim annos = TraitAnnotation.ParseTable("C:\Users\Administrator\Downloads\ncbi_species_summary_no_predictions.tsv").ToArray
        Dim microbials = TraitAnnotation.CreateProfiles(annos).OrderByDescending(Function(a) a.traits.Length).ToArray

        Call microbials.First.GetJson.SaveTo("Z:/metaTrait.json")

        Pause()
    End Sub
End Module
