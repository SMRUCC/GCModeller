Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Metagenomics

Namespace PICRUSt

    ''' <summary>
    ''' the data tree is index via two index:
    ''' 
    ''' 1. greengenes OTU id index
    ''' 2. NCBI taxonomy tree index
    ''' </summary>
    ''' <remarks>
    ''' tree value is the bytes offset
    ''' </remarks>
    Public Class ko_13_5_precalculated : Inherits Tree(Of Long)

        ''' <summary>
        ''' one of the node in the taxonomy tree
        ''' </summary>
        ''' <returns></returns>
        Public Property taxonomy As TaxonomyRanks

        ''' <summary>
        ''' 相同的taxonomy可能会映射到多个greengenes编号？
        ''' </summary>
        ''' <returns></returns>
        Public Property ggId As New List(Of String)

        Public Overrides ReadOnly Property QualifyName As String
            Get
                If Parent Is Nothing OrElse label = "." OrElse label = "/" Then
                    Return "/"
                Else
                    Return (Parent.QualifyName & ";" & $"{taxonomy.Description.ToLower}__{label}").Trim("/"c, ";"c, " "c, ASCII.TAB, "."c)
                End If
            End Get
        End Property

    End Class
End Namespace