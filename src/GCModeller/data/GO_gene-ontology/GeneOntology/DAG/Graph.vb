Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.genomics.foundation.OBO_Foundry
Imports Microsoft.VisualBasic

Namespace DAG

    Public Class Graph

        ReadOnly __DAG As Dictionary(Of Term)
        ReadOnly _file$

        Public ReadOnly Property header As header

        Sub New(path$)
            Dim obo As GO_OBO = GO_OBO.LoadDocument(path$)
            __DAG = obo.Terms.BuildTree
            _file = path$
        End Sub

        Public Overrides Function ToString() As String
            Return _file.ToFileURL
        End Function

        Public Iterator Function Visit(id As String) As IEnumerable(Of Term())

        End Function
    End Class
End Namespace