Imports SMRUCC.genomics.MetabolicModel
Imports SMRUCC.genomics.Model.Biopax.Level3

Module Program
    Sub Main(args As String())
        Dim docs = "D:\GCModeller\src\GCModeller\models\SBML\data\smpdb_PW000266.owl"
        Dim pathway = File.LoadDoc(docs)
        Dim loader = ResourceReader.LoadResource(pathway)
        Dim reactions As MetabolicReaction() = loader.GetAllReactions.ToArray
        Dim compounds = loader.GetAllCompounds.ToArray

        Pause()
    End Sub
End Module
