#Region "Microsoft.VisualBasic::ffb7abfb8ff9174ee1cb3bf83ea30447, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Files\Promoters.vb"

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

    '   Total Lines: 60
    '    Code Lines: 43
    ' Comment Lines: 10
    '   Blank Lines: 7
    '     File Size: 2.49 KB


    '     Class Promoters
    ' 
    '         Properties: AttributeList
    ' 
    '         Function: GetPromoters, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Assembly.MetaCyc.Schema.Reflection
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA.Reflection
Imports Microsoft.VisualBasic

Namespace Assembly.MetaCyc.File.DataFiles

    ''' <summary>
    ''' Frames in this class define transcription start sites.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Promoters : Inherits DataFile(Of Slots.Promoter)

        Public Overrides ReadOnly Property AttributeList As String()
            Get
                Return {
                    "UNIQUE-ID", "TYPES", "COMMON-NAME", "ABSOLUTE-PLUS-1-POS",
                    "BINDS-SIGMA-FACTOR", "CITATIONS", "COMMENT", "COMMENT-INTERNAL",
                    "COMPONENT-OF", "CREDITS", "DATA-SOURCE", "DBLINKS",
                    "DOCUMENTATION", "HIDE-SLOT?", "INSTANCE-NAME-TEMPLATE",
                    "LEFT-END-POSITION", "MEMBER-SORT-FN", "MINUS-10-LEFT",
                    "MINUS-10-RIGHT", "MINUS-35-LEFT", "MINUS-35-RIGHT",
                    "RIGHT-END-POSITION", "SYNONYMS", "TEMPLATE-FILE"
                }
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0}  {1} frame object records.", DbProperty.ToString, FrameObjects.Count)
        End Function

        ''' <summary>
        ''' 获取所有的启动子的序列的集合
        ''' </summary>
        ''' <param name="Genome"></param>
        ''' <returns></returns>
        ''' <remarks>启动子的序列长度取250bp</remarks>
        Public Function GetPromoters(Genome As FASTA.FastaSeq) As FASTA.FastaFile
            Dim pList As List(Of FASTA.FastaSeq) = New List(Of FASTA.FastaSeq)
            For Each Promoter In Me
                Dim Seq As FASTA.FastaSeq = New FASTA.FastaSeq With {
                    .Headers = New String() {Promoter.Identifier}
                }
                Dim d As Integer = Promoter.Direction
                If d = 1 Then
                    Seq.SequenceData = Mid(Genome.SequenceData, Val(Promoter.AbsolutePlus1Pos) - 250, 250)
                ElseIf d = -1 Then
                    Seq.SequenceData = Mid(Genome.SequenceData, Val(Promoter.AbsolutePlus1Pos), 250)
                    Call FASTA.FastaSeq.Complement(Seq)
                Else
                    Continue For
                End If

                Call pList.Add(Seq)
            Next

            Return pList
        End Function
    End Class
End Namespace
