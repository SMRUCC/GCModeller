Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.metaTraits

Module metaTraitsTest

    Sub readFiles()
        Dim annos = TraitAnnotation.ParseTable("C:\Users\Administrator\Downloads\ncbi_species_summary_no_predictions.tsv").ToArray

        Pause()
    End Sub
End Module
