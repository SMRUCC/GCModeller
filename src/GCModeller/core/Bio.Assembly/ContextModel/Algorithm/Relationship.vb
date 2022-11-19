#Region "Microsoft.VisualBasic::5b2ddab44c39b7064157077dbecd14de, GCModeller\core\Bio.Assembly\ContextModel\Algorithm\Relationship.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 75
    '    Code Lines: 47
    ' Comment Lines: 17
    '   Blank Lines: 11
    '     File Size: 2.26 KB


    '     Class Relationship
    ' 
    '         Properties: Gene, locus_tag, Relation
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ATGDist, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci
Imports stdNum = System.Math

Namespace ContextModel

    ''' <summary>
    ''' 描述位点在基因组上面的位置，可以使用<see cref="ToString"/>函数获取得到位置描述
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    Public Class Relationship(Of T As IGeneBrief)
        Implements IReadOnlyId

        ''' <summary>
        ''' Target gene object
        ''' </summary>
        ''' <returns></returns>
        Public Property Gene As T
        Public Property Relation As SegmentRelationships

        ''' <summary>
        ''' <see cref="IGeneBrief.Key"/>
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property locus_tag As String Implements IReadOnlyId.Identity
            Get
                If Gene Is Nothing Then
                    Return ""
                Else
                    Return Gene.Key
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

            If stdNum.Abs(d1) < stdNum.Abs(d2) Then
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
