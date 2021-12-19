Imports Microsoft.VisualBasic.Data.GraphTheory
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
        Public ReadOnly Property taxonomy As String
            Get
                Return label
            End Get
        End Property

        Public Property ggId As String

        Public Overrides ReadOnly Property QualifyName As String
            Get
                If Parent Is Nothing OrElse taxonomy = "." OrElse taxonomy = "/" Then
                    Return taxonomy
                Else
                    Return Parent.QualifyName & ";" & taxonomy
                End If
            End Get
        End Property

    End Class
End Namespace