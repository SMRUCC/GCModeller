#Region "Microsoft.VisualBasic::b6bb6130a68b350ab283700f734d3319, analysis\RNA-Seq\Toolkits.RNA-Seq\Correlations\Operon\Corrects.vb"

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

    '     Module Corrects
    ' 
    '         Function: __correctOperon, __splitOperon, CorrectDoorOperon
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Assembly.DOOR
Imports SMRUCC.genomics.ComponentModel.Loci
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic

Namespace Operon

    ''' <summary>
    ''' 根据转录组数据来修正操纵子
    ''' </summary>
    ''' <remarks></remarks>
    <Package("Operon.Corrects", Category:=APICategories.UtilityTools, Description:="Corrects the DOOR operon data based on the RNA-Seq data.")>
    Public Module Corrects

        ''' <summary>
        ''' 由于假设认为DOOR里面的operon是依照基因组上面的位置来作为一个计算因素的，所以在这里的预测数据在物理位置上面都是满足的，假若基因不是一个operon里面的，则只需要分割operon就好了
        ''' </summary>
        ''' <param name="PCC"></param>
        ''' <param name="DOOR"></param>
        ''' <param name="pccCutoff">应该是一个正数</param>
        ''' <returns></returns>
        <ExportAPI("DOOR.Operon.Correct")>
        Public Function CorrectDoorOperon(PCC As PccMatrix,
                                          DOOR As DOOR,
                                          <Parameter("Cutoff.PCC", "value should be positive")>
                                          Optional pccCutoff As Double = 0.45) As SMRUCC.genomics.Assembly.DOOR.Operon()
            Dim parallel As Boolean = True
#If DEBUG Then
            parallel = False
#End If
            Dim LQuery = DOOR.DOOROperonView.Operons.Select(Function(operon) __correctOperon(operon, PCC, pccCutoff))  ' 首先假设Door数据库之中的操纵子之中的基因之间的距离是合理的正确的
            Dim lstCorrected As SMRUCC.genomics.Assembly.DOOR.Operon() =
                (From x In LQuery.Unlist Select x Order By x.Key Ascending).ToArray
            Return lstCorrected
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Operon"></param>
        ''' <returns></returns>
        Private Function __correctOperon(Operon As SMRUCC.genomics.Assembly.DOOR.Operon,
                                         PCC As PccMatrix,
                                         pccCut As Double) As SMRUCC.genomics.Assembly.DOOR.Operon()
            If Operon.NumOfGenes = 1 Then
                Return New SMRUCC.genomics.Assembly.DOOR.Operon() {Operon}
            Else
                Dim source As OperonGene()
                If Operon.Strand = Strands.Forward Then
                    source = (From x In Operon.Value Select x Order By x.Location.Left Ascending).ToArray
                Else
                    source = (From x In Operon.Value Select x Order By x.Location.Right Descending).ToArray
                End If
                Return __splitOperon(Operon.OperonID, source, Operon.InitialX, 1, pccCut, PCC)
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Operon"></param>
        ''' <param name="idx">参数请从1开始设置</param>
        ''' <param name="structGenes">在递归的最开始阶段需要根据链的方向进行排序操作的</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function __splitOperon(Operon As String, structGenes As OperonGene(), Initial As OperonGene,
                                       idx As Integer,
                                       cutoff As Double,
                                       PccMatrix As PccMatrix) As SMRUCC.genomics.Assembly.DOOR.Operon()
            Dim cLst As List(Of SMRUCC.genomics.Assembly.DOOR.Operon) =
                New List(Of SMRUCC.genomics.Assembly.DOOR.Operon)

            For i As Integer = 0 To structGenes.Length - 1
                Dim Pcc As Double = PccMatrix.GetValue(structGenes(i).Synonym, Initial.Synonym)

                If Pcc < 0 OrElse Pcc < cutoff Then 'Operon之中的第一个基因和其他的基因之间的Pcc必须要大于0
                    Dim newPart As OperonGene() = structGenes.Take(i).ToArray
                    Dim newOperon As SMRUCC.genomics.Assembly.DOOR.Operon =
                        New SMRUCC.genomics.Assembly.DOOR.Operon($"{Operon}.{idx}", newPart)

                    Call cLst.Add(newOperon)

                    idx += 1
                    newPart = structGenes.Skip(i).ToArray

                    If Not newPart.IsNullOrEmpty Then
                        Call cLst.AddRange(__splitOperon(Operon, newPart, newPart.First, idx, cutoff, PccMatrix))
                    End If

                    Exit For
                End If
            Next

            If cLst.IsNullOrEmpty Then ' 原先的数据没有错误，则返回原来的数据
                If idx > 1 Then
                    Operon = $"{Operon}.{idx}"
                End If
                Dim opr As New SMRUCC.genomics.Assembly.DOOR.Operon(Operon, structGenes)
                Return {opr}
            End If

            Return cLst.ToArray
        End Function
    End Module
End Namespace
