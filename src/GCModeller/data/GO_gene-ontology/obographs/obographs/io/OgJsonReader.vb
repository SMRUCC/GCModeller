Imports System.IO
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace org.geneontology.obographs.io




    Public Class OgJsonReader

        Public Shared Function readFile(fileName As String) As org.geneontology.obographs.model.Graph
            Return readFile(New FileStream(fileName, FileMode.Open))
        End Function

        Public Shared Function readFile(file As FileStream) As org.geneontology.obographs.model.Graph
            Return readInputStream(file)
        End Function

        Public Shared Function readInputStream(stream As Stream) As org.geneontology.obographs.model.Graph
            Return TryCast(stream.LoadJSONObject(GetType(org.geneontology.obographs.model.Graph)), org.geneontology.obographs.model.Graph)
        End Function

    End Class

End Namespace