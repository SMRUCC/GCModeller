#Region "Microsoft.VisualBasic::6655636b8aab15e605a0f12fd33db298, DataTools\Tools\Report\AnnotationResult.vb"

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

    '     Class GenomeAnnotations
    ' 
    '         Properties: GenomeTitle, Proteins
    ' 
    '         Function: __createRecord, CompileResult, SaveHtml, SaveRTF, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.MIME.RTF
Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.genomics.Analysis.Annotations.Reports.DocumentElements
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Reports

    <XmlType("Annotations", Namespace:="http://code.google.com/p/genome-in-code/annotations")>
    Public Class GenomeAnnotations

        <XmlElement> Public Property Proteins As ProteinAnnotationResult()
        <XmlElement> Public Property GenomeTitle As String

        Public Overrides Function ToString() As String
            Return GenomeTitle
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Orthologs">按照物种进行分组的</param>
        ''' <param name="Paralogs"></param>
        ''' <param name="Fasta">该物种基因组的蛋白质序列</param>
        ''' <param name="AnnotationSource"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CompileResult(Orthologs As Dictionary(Of String, BiDirectionalBesthit()),
                                             Paralogs As BestHit(),
                                             Fasta As String, AnnotationSource As AnnotationTool.MetaSource()) As GenomeAnnotations
            Dim Proteins = (From FastaObject In FastaFile.Read(Fasta) Select ID = FastaObject.Headers.First, FastaObject).ToArray
            Dim LQuery As ProteinAnnotationResult() = (From Entry In Proteins
                                                       Select New ProteinAnnotationResult With {
                                                                  .Protein = Entry.ID,
                                                                  .ProteinSequence = Entry.FastaObject.SequenceData,
                                                                  .Paralogs = (From item As BestHit
                                                                               In Paralogs
                                                                               Where String.Equals(Entry.ID, item.QueryName, StringComparison.OrdinalIgnoreCase)
                                                                               Select New Paralogs With
                                                                                      {
                                                                                          .Evalue = item.evalue,
                                                                                          .Hit = item.HitName,
                                                                                          .Identities = item.identities}).ToArray,
                                                                  .Orthologs = (From item In Orthologs
                                                                                Let InternalCreateRecord = __createRecord(item, Entry.ID)
                                                                                Where Not InternalCreateRecord Is Nothing
                                                                                Select InternalCreateRecord).ToArray}).ToArray

            Return New GenomeAnnotations With {.Proteins = LQuery, .GenomeTitle = basename(Fasta)}
        End Function

        Private Shared Function __createRecord(Orthologs As KeyValuePair(Of String, BiDirectionalBesthit()), EntryID As String) As Orthologs
            Dim Obh = (From nnn As BiDirectionalBesthit
                       In Orthologs.Value
                       Where String.Equals(EntryID, nnn.QueryName, StringComparison.OrdinalIgnoreCase)
                       Select nnn).ToArray

            If Obh.IsNullOrEmpty Then
                Return Nothing
            Else
                Dim bbh = Obh.First
                Return New Orthologs With {
                    .Hit = bbh.HitName,
                    .Identities = bbh.Identities,
                    .OrganismSpecies = Orthologs.Key
                }
            End If
        End Function

        ''' <summary>
        ''' 将结果保存为Html文件
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SaveHtml(Path As String) As Boolean
            Throw New NotImplementedException
        End Function

        Public Function SaveRTF(Path As String) As Boolean
            Dim Document As New Rtf

            Call Document.AppendLine(Me.GenomeTitle, New Font(40, True, FontFace.MicrosoftYaHei, False, False, System.Drawing.Color.Red))
            Call Document.AppendLine()
            Call Document.AppendLine(New String("="c, 128))

            Dim SectionBeginFont As New Font(16, True, FontFace.MicrosoftYaHei, False, False, Drawing.Color.Black)
            Dim QueryNameFont As New Font(16, True, FontFace.MicrosoftYaHei, True, False)
            Dim QuerySequenceFont As New Font(16, True, FontFace.MicrosoftYaHei, True, False, Drawing.Color.Blue)

            For Each Protein In Me.Proteins
                Call Document.AppendText("Query Protein:=   ", SectionBeginFont.Clone)
                Call Document.AppendText(Protein.Protein, QueryNameFont)
                Call Document.AppendLine()
                Call Document.AppendLine("Protein Query Sequence:=    ")
                Call Document.AppendLine(Protein.ProteinSequence, QuerySequenceFont)
                Call Document.AppendLine()
                Call Document.AppendLine(New String("-"c, 32))

                If Protein.Orthologs.IsNullOrEmpty Then
                    Call Document.AppendLine("No Orthologous Protein Data!")
                Else
                    Call Document.AppendLine("Orthologous Annotations Data:", New Font(20, True, FontFace.Ubuntu, False, True))
                    Call Document.AppendLine(New String("*", 50))

                    Call Document.AppendText("Orthology.Hit" & vbTab, New Font(16, True, FontFace.MicrosoftYaHei, False, True))
                    Call Document.AppendText("Orthology.OrganismSpecies" & vbTab)
                    Call Document.AppendText("Orthology.Evalue" & vbTab, New Font(18, True, FontFace.MicrosoftYaHei, False, False, Drawing.Color.Red))
                    Call Document.AppendText("Orthology.Identities" & vbTab)
                    Call Document.AppendLine()

                    For Each Orthology In Protein.Orthologs
                        Call Document.AppendText(Orthology.Hit & vbTab, New Font(16, True, FontFace.MicrosoftYaHei, False, True))
                        Call Document.AppendText(Orthology.OrganismSpecies & vbTab)
                        Call Document.AppendText(Orthology.Evalue & vbTab, New Font(18, True, FontFace.MicrosoftYaHei, False, False, Drawing.Color.Red))
                        Call Document.AppendText(Orthology.Identities & vbTab)
                        Call Document.AppendLine()
                    Next
                End If

                If Protein.Paralogs.IsNullOrEmpty Then
                    Call Document.AppendLine("No Paralogs Proteins Data!")
                Else
                    Call Document.AppendLine("Paralogs Data:")
                    Call Document.AppendLine(New String("*", 50))

                    Call Document.AppendText("Paralog.Hit" & vbTab, New Font(24, True, FontFace.MicrosoftYaHei, False, True))
                    Call Document.AppendText("Paralog.Evalue" & vbTab, New Font(24, True, FontFace.MicrosoftYaHei, False, False, Drawing.Color.Red))
                    Call Document.AppendText("Paralog.Identities" & vbTab, New Font(24, True, FontFace.MicrosoftYaHei, False, True))
                    Call Document.AppendLine()

                    For Each Paralog In Protein.Paralogs
                        Call Document.AppendText(Paralog.Hit & vbTab, New Font(16, True, FontFace.MicrosoftYaHei, False, True))
                        Call Document.AppendText(Paralog.Evalue & vbTab, New Font(18, True, FontFace.MicrosoftYaHei, False, False, Drawing.Color.Red))
                        Call Document.AppendText(Paralog.Identities & vbTab)
                        Call Document.AppendLine()
                    Next
                End If

                Call Document.AppendLine()
                Call Document.AppendLine()
                Call Document.AppendLine()
            Next

            Return Document.Save(Path)
        End Function
    End Class
End Namespace
