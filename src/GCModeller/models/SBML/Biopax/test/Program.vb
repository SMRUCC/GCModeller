Imports SMRUCC.genomics.MetabolicModel
Imports SMRUCC.genomics.Model.Biopax.Level3

Module Program
    Sub Main(args As String())
        Dim docs = "E:\GCModeller\src\GCModeller\models\SBML\data\Escherichia_coli.owl"
        Dim pathway = File.LoadDoc(docs)
        Dim loader = ResourceReader.LoadResource(pathway)
        Dim reactions As MetabolicReaction() = loader.GetAllReactions.ToArray

        Pause()
    End Sub
End Module
