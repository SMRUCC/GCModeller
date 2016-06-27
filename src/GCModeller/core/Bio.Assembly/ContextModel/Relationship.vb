Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports LANS.SystemsBiology.ComponentModel
Imports LANS.SystemsBiology.ComponentModel.Loci
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language

Namespace ContextModel

    ''' <summary>
    ''' 描述位点在基因组上面的位置，可以使用<see cref="ToString"/>函数获取得到位置描述
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    Public Class Relationship(Of T As IGeneBrief) : Inherits ClassObject
        Implements IReadOnlyId

        ''' <summary>
        ''' Target gene object
        ''' </summary>
        ''' <returns></returns>
        Public Property Gene As T
        Public Property Relation As SegmentRelationships

        ''' <summary>
        ''' <see cref="IGeneBrief.Identifier"/>
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property locus_tag As String Implements IReadOnlyId.Identity
            Get
                If Gene Is Nothing Then
                    Return ""
                Else
                    Return Gene.Identifier
                End If
            End Get
        End Property

        Sub New()
        End Sub

        Sub New(gene As T, rel As SegmentRelationships)
            Me.Gene = gene
            Me.Relation = rel
        End Sub

        Public Function ATGDist(loci As NucleotideLocation) As Integer
            Dim ATG As Integer
            Dim s As Integer

            If Gene.Location.Strand = Strands.Forward Then
                ATG = Gene.Location.Left
                s = 1
            Else
                ATG = Gene.Location.Right
                s = -1
            End If

            Dim d1 As Integer = loci.Left - ATG
            Dim d2 As Integer = loci.Right - ATG

            If Math.Abs(d1) < Math.Abs(d2) Then
                Return d1 * s
            Else
                Return d2 * s
            End If
        End Function

        ''' <summary>
        ''' Gets loci location relationship description with this <see cref="Gene"/> object.
        ''' (位置关系的描述信息)
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return Relation.LocationDescription(Gene)
        End Function
    End Class
End Namespace