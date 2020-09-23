#Region "Microsoft.VisualBasic::11a07dd0ef45f73ef9e913b9293bfb43, analysis\RNA-Seq\Toolkits.RNA-Seq\Matrix\API.vb"

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

    '     Module API
    ' 
    '         Function: __getBirefDicts, AssociatesPathwaysInfo, CreatePccMatrix, LoadChipData, Log2Profiles
    '                   Log2Values, MergeDataMatrix, SelectLog2Genes, ToSamples
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Linq.JoinExtensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.ComponentModel.Annotation

Namespace dataExprMAT

    ''' <summary>
    ''' Gene expression chip data.(基因芯片数据)
    ''' </summary>
    ''' <remarks></remarks>
    <Package("Analysis.Chipdata", Publisher:="xie.guigang@gmail.com")>
    Public Module API

        <ExportAPI("ToSamples")>
        Public Function ToSamples(source As IEnumerable(Of IExprMAT)) As ExprMAT()
            Return source.Select(
                Function(x) New ExprMAT With {
                    .dataExpr0 = x.dataExpr0,
                    .LocusId = x.LocusId}).ToArray
        End Function

        '<Command("associate.kegg_modules")>
        'Public Shared Function AssociatesPathwaysInfo(ChipData As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File, Pathways As KEGGModuleBrief()) _
        '    As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File

        '    Dim DictPathwayBrief = CreateBirefDictionary(Pathways)

        '    For i As Integer = 1 To ChipData.Count - 1
        '        Dim ChipdataRow = ChipData(i)
        '        Dim GeneId As String = ChipdataRow.First

        '        If Not DictPathwayBrief.ContainsKey(GeneId) Then
        '            Call ChipdataRow.Add("")
        '            Continue For
        '        End If

        '        Call ChipdataRow.Add(DictPathwayBrief(GeneId))
        '    Next

        '    Call ChipData.First.Add("KEGG.Modules")
        '    Return ChipData
        'End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="path">CSV document which contains the gene expression value.</param>
        ''' <returns></returns>
        <ExportAPI("Read.Chipdata")>
        Public Function LoadChipData(path As String) As MatrixFrame
            Call $"Start to load chipdata from csv file {path.ToFileURL}".__DEBUG_ECHO
            Dim CsvDoc As IO.File = IO.File.Load(path)
            Dim MAT As MatrixFrame = MatrixFrame.Load(CsvDoc)
            Return MAT
        End Function

        <ExportAPI("PccMatrix.Create.From.Chipdata")>
        Public Function CreatePccMatrix(chipdata As String) As PccMatrix
            Return CreatePccMAT(chipdata, True)
        End Function

        <ExportAPI("Load.Log2_Profiles")>
        Public Function Log2Profiles(path As String) As String()()
            Dim Lines As String() = path.ReadAllLines
            Return (From strLine As String In Lines Select Strings.Split(strLine, vbTab)).ToArray
        End Function

        <ExportAPI("RPKM.Log2")>
        Public Function Log2Values([operator] As MatrixFrame, profile As String()()) As File
            Return [operator].Log2(samples:=profile)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="dat"></param>
        ''' <param name="expr"></param>
        ''' <param name="level">上调或者下调的log2的最低倍数</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Select.Log2")>
        Public Function SelectLog2Genes(dat As MatrixFrame, expr As IEnumerable(Of String), Optional level As Double = 2) As String()
            Dim Cols As String() = (From briefID As String
                                    In expr
                                    Let lstId As String() = (From col As String In dat.LstExperiments
                                                             Where InStr(col, briefID) > 0
                                                             Select col).ToArray
                                    Select lstId).ToArray.Unlist.Distinct.ToArray
            Dim Dict As Dictionary(Of String, Double()) = dat.ToDictionary
            Dim buffer As New List(Of String)

            level = Math.Abs(level)

            For Each Col As String In Cols
                Call dat.SetColumn(ExperimentId:=Col)
                Dim LQuery = (From row As KeyValuePair(Of String, Double())
                              In Dict.AsParallel
                              Where Math.Abs(row.Value(dat.__pCol)) >= level
                              Select row.Key).ToArray
                Call buffer.AddRange(LQuery)
            Next

            Return buffer.Distinct.ToArray
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="DIR">放着多个实验所得到的芯片数据的文件夹</param>
        ''' <param name="RemoveGaps">在芯片数据之中所缺失的部分的数据是否进行移除，否则将会使用数值0来代替</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Chipdata.Merge")>
        Public Function MergeDataMatrix(<Parameter("Source.DIR")> DIR As String,
                                        <Parameter("Gaps.Removes?",
                                                   "If the gene is not found the source part of the data, then should this function removes this incomplete data? 
                                                    Otherwise this function will using ZERO value to fill the gaps, default action is not removes.")>
                                        Optional RemoveGaps As Boolean = False) As IO.File
            Dim dataExprMATs As MatrixFrame() = (From Path As String
                                                 In FileIO.FileSystem.GetFiles(DIR, FileIO.SearchOption.SearchTopLevelOnly, "*.csv")
                                                 Select dataExprMAT.API.LoadChipData(Path)).ToArray

            Dim lstLocusId As String()() = (From File As MatrixFrame
                                            In dataExprMATs
                                            Select lst = File.LstLocusId).ToArray
            Dim IdList As String()

            If RemoveGaps Then
                IdList = lstLocusId.Intersection
            Else
                IdList = (From strId As String
                          In lstLocusId.IteratesALL
                          Where Not String.IsNullOrEmpty(strId)
                          Select strId.ToUpper.Trim  ' 为什么会多了几个基因号？？ 是大小写还是有空格的问题？
                          Distinct).ToArray
                Call $"{IdList.Length} genes in the data set!".__DEBUG_ECHO
            End If

            Dim HeadTitle As New IO.RowObject From {"GeneId"}
            Dim GeneRows = (From strId As String
                            In IdList
                            Select New IO.RowObject From {strId}).ToArray

            For Each dataMAT0 As MatrixFrame In dataExprMATs
                Dim ExperimentIdlist As String() = dataMAT0.LstExperiments

                Call $"Processing {dataMAT0.ToString}.....".__DEBUG_ECHO

                For Each ExperimentId As String In ExperimentIdlist
                    Call dataMAT0.SetColumn(ExperimentId:=ExperimentId, calAvg:=False)

                    For i As Integer = 0 To IdList.Length - 1
                        GeneRows(i).Add(dataMAT0.GetValue(IdList(i)))
                    Next
                Next

                HeadTitle += From strId As String In ExperimentIdlist Select String.Join(".", dataMAT0.Name, strId)
            Next

            Dim DataFrame As IO.File = New IO.File + HeadTitle + GeneRows
            Return DataFrame
        End Function

        ''' <summary>
        ''' Assign the pathway information in to the genes to study which pathway was affect by the gene mutation from the chipdata log2 value.
        ''' </summary>
        ''' <param name="ChipData"></param>
        ''' <param name="Pathways"></param>
        ''' <param name="head"></param>
        ''' <returns></returns>
        Public Function AssociatesPathwaysInfo(ChipData As IO.File,
                                               Pathways As IEnumerable(Of PathwayBrief),
                                               head As String) As IO.File

            Dim DictPathwayBrief = __getBirefDicts(Pathways)
            'Dim PathwayGenes As String() = DictPathwayBrief.Keys.ToArray

            'Dim AssociateWithPathways = Function(GeneId As String) As String '获取所有与之相关的代谢途径的编号
            '                                '    Dim Buffer As String = ""

            '                                If DictPathwayBrief.ContainsKey(GeneId) Then  '目标基因是代谢途径之中的一个酶
            '                                    Return String.Join("; ", DictPathwayBrief(GeneId))
            '                                End If

            '                                ''还需要查找出与之相关的代谢途径
            '                                'Dim LQuery = (From strId As String
            '                                '              In PathwayGenes
            '                                '              Let pcc As Double = pccmatrix.GetValue(GeneId, strId)
            '                                '              Where pcc <> 1 AndAlso (pcc > pccCutoff OrElse pcc <= -0.65)
            '                                '              Select strId).ToArray
            '                                'Dim AssociatedPathwayIdCollection As List(Of String) = New List(Of String)

            '                                'For Each Line In (From strId As String In LQuery Select DictPathwayBrief(strId)).ToArray '得到了代谢途径的编号列表
            '                                '    Call AssociatedPathwayIdCollection.AddRange(Line)
            '                                'Next

            '                                'If LQuery.IsNullOrEmpty Then
            '                                '    Return New String() {Buffer, ""}
            '                                'Else
            '                                '    Return New String() {Buffer, String.Join("; ", (From strId As String In AssociatedPathwayIdCollection Select strId Distinct).ToArray)}
            '                                'End If
            '                            End Function

            For i As Integer = 1 To ChipData.Count - 1
                Dim ChipdataRow = ChipData(i)
                Dim GeneId As String = ChipdataRow.First
                Dim PathwayData As String = If(DictPathwayBrief.ContainsKey(GeneId), DictPathwayBrief(GeneId), "")  ' AssociateWithPathways(GeneId)

                Call ChipdataRow.Add(PathwayData)
            Next

            Call ChipData.First.Add(head)
            Return ChipData
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Briefs"></param>
        ''' <returns>{GeneId, Pathways}</returns>
        ''' <remarks></remarks>
        Private Function __getBirefDicts(Briefs As IEnumerable(Of PathwayBrief)) As Dictionary(Of String, String)
            Dim LQuery = (From pathway As PathwayBrief
                          In Briefs
                          Let associats As String() = pathway.GetPathwayGenes
                          Where Not associats.IsNullOrEmpty
                          Select (From sId As String
                                  In associats
                                  Select sId,
                                      pathway.EntryId).ToArray).ToArray

            Dim lstLocus As List(Of String) = New List(Of String)
            For Each Item In LQuery
                Dim lst As String() = (From brief In Item Select brief.sId).ToArray
                Call lstLocus.AddRange(lst)
            Next
            lstLocus = (From sId As String In lstLocus Select sId Distinct).AsList

            Dim lstBriefs = LQuery.Unlist
            Dim DictPathwayBrief As Dictionary(Of String, String) = New Dictionary(Of String, String)

            For Each sId As String In lstLocus
                Dim BriefQuery = (From item In lstBriefs Where String.Equals(item.sId, sId) Select item).ToArray
                Dim array As String() = (From item In BriefQuery
                                         Select item.EntryId
                                         Distinct
                                         Order By EntryId Ascending).ToArray
                Call DictPathwayBrief.Add(sId, String.Join("; ", array))
            Next

            Return DictPathwayBrief
        End Function
    End Module
End Namespace
