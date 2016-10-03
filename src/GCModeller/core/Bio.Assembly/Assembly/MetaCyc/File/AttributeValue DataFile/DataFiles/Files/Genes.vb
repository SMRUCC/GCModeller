#Region "Microsoft.VisualBasic::3a5d7e0a7a265f811c3527a16a2a0212, ..\GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Files\Genes.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports SMRUCC.genomics.Assembly.MetaCyc.Schema.Reflection
Imports Microsoft.VisualBasic

Namespace Assembly.MetaCyc.File.DataFiles
    ''' <summary>
    ''' Each frame in the class Genes describes a single gene, meaning a region of DNA that defines a 
    ''' coding region for one or more gene products. Multiple gene products may be produced because 
    ''' of modification of an RNA or protein.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Genes : Inherits DataFile(Of Slots.Gene)

        Public Overrides ReadOnly Property AttributeList As String()
            Get
                Return {
                    "UNIQUE-ID", "TYPES", "COMMON-NAME", "ACCESSION-1", "ACCESSION-2", "CENTISOME-POSITION",
                    "CITATIONS", "COMMENT", "COMMENT-INTERNAL", "COMPONENT-OF", "CREDITS", "DATA-SOURCE",
                    "DBLINKS", "DOCUMENTATION", "HIDE-SLOT?", "IN-PARALOGOUS-GENE-GROUP", "INTERRUPTED?",
                    "KNOCKOUT-GROWTH-OBSERVATIONS", "LAST-UPDATE", "LEFT-END-POSITION", "MEMBER-SORT-FN",
                    "PRODUCT", "REGULATED-BY", "RIGHT-END-POSITION", "SYNC-W-ORTHOLOG", "SYNONYMS",
                    "TEMPLATE-FILE", "TRANSCRIPTION-DIRECTION"
                }
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0}  {1} frame object records.", DbProperty.ToString, FrameObjects.Count)
        End Function

        ''' <summary>
        ''' 获取所有基因对象的UniqueId所组成的集合
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetAllGeneIds() As String()
            Dim Query = From Gene In Me.AsParallel Select Gene.Identifier '
            Return Query.ToArray
        End Function

        ''' <summary>
        ''' 尝试着取出所有的基因对象的启动子序列，结果不是很准确，请慎用！！！
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function TryParsePromoters(Genome As SequenceModel.FASTA.FastaToken) As SequenceModel.FASTA.FastaFile
            Dim pList As List(Of SequenceModel.FASTA.FastaToken) = New List(Of SequenceModel.FASTA.FastaToken)
            For Each Gene In Me
                Dim Seq As SequenceModel.FASTA.FastaToken = New SequenceModel.FASTA.FastaToken With {
                    .Attributes = New String() {Gene.Identifier, Gene.Identifier & "_PM", "Di: " & Gene.TranscriptionDirection, Gene.CommonName}
                }
                If String.Equals(Gene.TranscriptionDirection, "+") Then
                    Seq.SequenceData = Mid(Genome.SequenceData, Val(Gene.LeftEndPosition) - 300, 300)
                ElseIf String.Equals(Gene.TranscriptionDirection, "-") Then
                    Seq.SequenceData = Mid(Genome.SequenceData, Val(Gene.RightEndPosition), 300)
                    Call SequenceModel.FASTA.FastaToken.Complement(Seq)
                Else
#If DEBUG Then
                    Console.WriteLine("Unknown direaction: {0}", Gene.Identifier)
#End If
                End If
            Next

            Return pList
        End Function
    End Class
End Namespace
