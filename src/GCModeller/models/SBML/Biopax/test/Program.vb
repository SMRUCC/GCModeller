Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.MetabolicModel
Imports SMRUCC.genomics.Model.Biopax.Level3
Imports SMRUCC.genomics.Model.SBML.SBGN

Module Program

    Sub Main(args As String())
        Call graph_test()
    End Sub

    Sub graph_test()
        Dim layout = sbgnFile.ReadXml("E:\GCModeller\src\GCModeller\models\SBML\data\R-HSA-211945.sbgn")

        Call Console.WriteLine(layout.GetJson)

        Pause()
    End Sub

    Sub readerTest()
        Dim docs = "D:\GCModeller\src\GCModeller\models\SBML\data\smpdb_PW000266.owl"
        Dim pathway = File.LoadDoc(docs)
        Dim loader = ResourceReader.LoadResource(pathway)
        Dim reactions As MetabolicReaction() = loader.GetAllReactions.ToArray
        Dim compounds = loader.GetAllCompounds.ToArray

        Pause()
    End Sub
End Module
