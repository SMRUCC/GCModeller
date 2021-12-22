Imports System.Runtime.CompilerServices
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
    ''' 相同的taxonomy可能会映射到多个greengenes编号
    ''' tree value is the mapping table of greengenes id to bytes offset
    ''' </remarks>
    Public Class ko_13_5_precalculated : Inherits Tree(Of Dictionary(Of String, Long))

        ''' <summary>
        ''' one of the node in the taxonomy tree
        ''' </summary>
        ''' <returns></returns>
        Public Property taxonomyRank As TaxonomyRanks

        Public Overrides ReadOnly Property QualifyName As String
            Get
                If Parent Is Nothing OrElse label = "." OrElse label = "/" Then
                    Return "/"
                Else
                    Return getFullName()
                End If
            End Get
        End Property

        Public ReadOnly Property size As Integer
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Data.Count
            End Get
        End Property

        Public ReadOnly Property ggId As String()
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Data.Keys.ToArray
            End Get
        End Property

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function getFullName() As String
            Return (Parent.QualifyName & ";" & $"{taxonomyRank.Description.ToLower}__{label}").Trim("/"c, ";"c, " "c, ASCII.TAB, "."c)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Sub Add(ggid As String, offset As Long)
            Call Data.Add(ggid, offset)
        End Sub

    End Class
End Namespace