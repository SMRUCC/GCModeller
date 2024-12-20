Imports Microsoft.VisualBasic.Data.NLP

Module Module1

    Sub testtfidf()
        Dim docs = New String() {"knowledge building needs innovative environments are better at helping their inhabitants explore the adjacent possible", "As a basis for evaluating explanations, creative knowledge building weight of evidence is a poor substitute for the first two criteria listed above.", "A public idea database makes every passing idea visible to everyone else in the organization and do creative work.", "questioning and various disturbances initiate cycles of innovation and creative organization knowledge.", "We need some way to ensure knowledge to spread among environments that any notes that are dropped are dropped."}

        Dim tfIdf As TF_IDF = New TF_IDF(docs)
        For i = 0 To tfIdf.docs.Count - 1
            Console.Write(i + 1.ToString() & vbTab)
            For j = 0 To tfIdf.docs.Count - 1
                Console.Write(tfIdf.getSimilarity(i, j).ToString() & vbTab)
            Next
            Console.WriteLine()
        Next
    End Sub
End Module
