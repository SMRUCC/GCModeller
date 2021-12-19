Imports Microsoft.VisualBasic.Data.GraphTheory
Imports SMRUCC.genomics.Metagenomics

Namespace PICRUSt

    ''' <summary>
    ''' the data tree is index via two index:
    ''' 
    ''' 1. greengenes OTU id index
    ''' 2. NCBI taxonomy tree index
    ''' </summary>
    Public Class ko_13_5_precalculated : Inherits Tree(Of Double())

        Public Property taxonomy As Taxonomy
        Public Property ggId As String

    End Class
End Namespace