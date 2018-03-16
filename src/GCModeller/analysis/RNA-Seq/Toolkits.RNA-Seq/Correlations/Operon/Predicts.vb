#Region "Microsoft.VisualBasic::1d970a744fdd9c312258c8d0a963c146, analysis\RNA-Seq\Toolkits.RNA-Seq\Correlations\Operon\Predicts.vb"

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

    '     Module Predicts
    ' 
    '         Function: __getStrands, ConvertToCsvModel, Predicts, SaveOperonData, SaveToDoor
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace Operon

    ''' <summary>
    ''' 由于在这里operon里面的基因都是共转录的，而非调控关系，所以必须选择PCC矩阵，并且pcc的值不能够为负数
    ''' </summary>
    ''' <remarks></remarks>
    <Package("Operon.Predict", Category:=APICategories.UtilityTools)>
    Public Module Predicts

        <ExportAPI("Write.Csv.Operons")>
        Public Function SaveOperonData(data As IEnumerable(Of DOOR.CsvModel.Operon), saveCsv As String) As Boolean
            Return data.SaveTo(saveCsv, False)
        End Function

        <ExportAPI("LDM.Csv")>
        Public Function ConvertToCsvModel(operons As DOOR.Operon()) As DOOR.CsvModel.Operon()
            Dim LQuery = (From operon As DOOR.Operon In operons
                          Let OperonData = operon.ConvertToCsvData
                          Select OperonData
                          Order By OperonData.DoorId Ascending).ToArray
            Return LQuery
        End Function

        <ExportAPI("Write.DOOR")>
        Public Function SaveToDoor(datas As IEnumerable(Of DOOR.Operon), saveto As String) As Boolean
            Return DOOR.SaveFile(data:=datas.ToArray, Path:=saveto)
        End Function

        <Extension> Private Function __getStrands(source As DOOR.OperonGene(), strand As Strands) As List(Of DOOR.OperonGene)
            Dim LQuery = (From g As DOOR.OperonGene In source.AsParallel
                          Where g.Location.Strand = strand
                          Select g
                          Order By g.Synonym Ascending).AsList
            Return LQuery
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="PccCutoff"></param>
        ''' <param name="Distance">上一个基因与当前的基因的位置之差的值大于这个阈值的时候，认为二者不在同一个操纵子之中</param>
        ''' <returns>首先假设Door数据库之中的操纵子之中的基因之间的距离是合理的正确的</returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Prediction")>
        Public Function Predicts(PTT As PTT, PCC As PccMatrix,
                                 <Parameter("locus.Prefix",
                                            "Gene name code should be a prefix for the gene name, for example is that ""XC_"" in the gene id ""XC_1184"" is the required gene name code parameter.")>
                                 locusPrefix As String,
                                 <Parameter("PCC.Cut")> Optional PccCutoff As Double = 0.7,
                                 Optional Distance As Integer = 500) As DOOR.CsvModel.Operon()

            Dim geneObjs As DOOR.OperonGene() = (From x As ComponentModels.GeneBrief In PTT.GeneObjects Select New DOOR.OperonGene(x)).ToArray
            Dim ForwardGene = geneObjs.__getStrands(Strands.Forward)
            Dim ReverseGene = geneObjs.__getStrands(Strands.Reverse)
            Dim PredictedOperon As List(Of DOOR.CsvModel.Operon) = New List(Of DOOR.CsvModel.Operon)
            Dim OperonId As Integer

            Do While ForwardGene.Count > 0
                Dim FirstGene = ForwardGene.First
                Dim Id As Integer = Val(Regex.Match(FirstGene.Synonym, "\d+").Value)
                Dim Operon As List(Of DOOR.OperonGene) = New List(Of DOOR.OperonGene)

                Call Operon.Add(FirstGene)

                Dim NewOperon = Sub()
                                    Call ForwardGene.Remove(FirstGene)
                                    For Each gene As DOOR.OperonGene In Operon
                                        Call ForwardGene.Remove(gene)
                                    Next

                                    Call PredictedOperon.Add(New DOOR.CsvModel.Operon With {.Direction = "+", .DoorId = OperonId, .NumOfGenes = Operon.Count, .Genes = (From Gene In Operon Select Gene.Synonym).ToArray})
                                    OperonId += 1
                                End Sub

                For idx As Integer = 1 To ForwardGene.Count - 1
                    Id += 1
                    If ForwardGene.Count = 1 Then
                        Call NewOperon()
                        Exit For
                    End If

                    Dim NextGeneId As String = String.Format("{0}{1}", locusPrefix, Format(Id, "0000"))
                    Dim NextGene = ForwardGene(idx)
                    If String.Equals(NextGene.Synonym, NextGeneId) Then
                        Dim Pccvalue As Double = PCC.GetValue(FirstGene.Synonym, NextGeneId)
                        If Pccvalue > PccCutoff Then
                            Call Operon.Add(NextGene)
                        Else
                            Call NewOperon()
                            Exit For
                        End If
                    Else
                        Call NewOperon()
                        Exit For
                    End If
                Next
            Loop

            OperonId = -1

            Do While ReverseGene.Count > 0
                Dim FirstGene = ReverseGene.First
                Dim Id As Integer = Val(Regex.Match(FirstGene.Synonym, "\d+").Value)
                Dim Operon As List(Of DOOR.OperonGene) = New List(Of DOOR.OperonGene)
                Call Operon.Add(FirstGene)

                Dim NewOperon = Sub()
                                    Call ReverseGene.Remove(FirstGene)
                                    For Each gene As DOOR.OperonGene In Operon
                                        Call ReverseGene.Remove(gene)
                                    Next

                                    Call PredictedOperon.Add(New DOOR.CsvModel.Operon With {.Direction = "-", .DoorId = OperonId, .NumOfGenes = Operon.Count, .Genes = (From Gene In Operon Select Gene.Synonym).ToArray})
                                    OperonId -= 1
                                End Sub

                For idx As Integer = 1 To ReverseGene.Count - 1
                    Id -= 1

                    If ForwardGene.Count = 1 Then
                        Call NewOperon()
                        Exit For
                    End If
                    Dim NextGeneId As String = String.Format("{0}{1}", locusPrefix, Format(Id, "0000"))
                    Dim NextGene = ReverseGene(idx)
                    If String.Equals(NextGene.Synonym, NextGeneId) Then
                        Dim Pccvalue As Double = PCC.GetValue(FirstGene.Synonym, NextGeneId)
                        If Pccvalue > PccCutoff Then
                            Call Operon.Add(NextGene)
                        Else
                            Call NewOperon()
                            Exit For
                        End If
                    Else
                        Call NewOperon()
                        Exit For
                    End If
                Next
            Loop

            Return PredictedOperon.ToArray
        End Function
    End Module
End Namespace
