#Region "Microsoft.VisualBasic::18affe5934a765f7492af4b81d4159ed, analysis\RNA-Seq\Toolkits.RNA-Seq\Correlations\GenesCOExpr.vb"

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

    ' Module GenesCOExpr
    ' 
    '     Function: Calculate, CalculateCorrelationMatrix, CalculatePccMatrix, (+2 Overloads) CalculateRegulations, CalculateSPccMatrix
    '               Find, Load, MergeChipData, MergeChipData2, Reconstruction
    '               SortMaxliklyhood
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Correlations.Correlations
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT
Imports SMRUCC.genomics.InteractionModel

''' <summary>
''' Gene co-expression analysis.(基因共表达分析)
''' </summary>
''' <remarks></remarks>
''' 
<Package("Gene.CO-Expression", Publisher:="xie.guigang@gmail.com")>
Public Module GenesCOExpr

    ''' <summary>
    ''' 首先进行关联分析，然后根据转录组数据筛选出调控事件
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("Regulation.Calculate")>
    Public Function CalculateRegulations(RPKM As IO.File, Optional Level As Integer = 2) As Rule()
        ''在这里因为要保持顺序，所以在编码的阶段不可以使用并行化拓展
        ''获取基因列表，在第一列
        'Dim GeneExpressions = DataServicesExtension.LoadCsv(RPKM.Skip(1))
        'Dim GeneIDList = (From row In GeneExpressions Select row.Tag).ToArray
        'Call $"Chip data load done!  {GeneIDList.Length} genes and {RPKM.First.Count - 1} experiments in the dataset.".__DEBUG_ECHO
        ''每一个基因都是事务之中的一个项目，每一次转录组实验都是一个Transaction
        'Call Console.WriteLine("AP Engine Services initialized!")
        ''进行表达值的Mapping
        ''首先生成每一次实验的列表，在进行Mapping
        'Dim ExperimentsTag As String() = RPKM.First.Skip(1).ToArray
        'Call Console.WriteLine("Experiments " & String.Join(";  ", ExperimentsTag))
        'Call Console.WriteLine("Start to generate transactions....")
        'Dim Transactions = (From idx As Integer In ExperimentsTag.Sequence
        '                    Select TransID = ExperimentsTag(idx),
        '                        EachGeneLevels = GenerateMapping(Level:=Level, data:=(From GeneSample In GeneExpressions Select GeneSample.ChunkBuffer(idx)).ToArray)).ToArray
        'Call Console.WriteLine("Encodings.....")
        'Dim Encodes = New Encoding(GeneIDList)
        'Call Console.WriteLine("There are {0} transaction tokens!", Encodes.CodeMappings.Count)
        'Call Console.WriteLine("Encoding transactions....")
        'Dim EncodesTransaction = Encodes.TransactionEncoding((From Trans In Transactions Select New Transaction With {.Name = Trans.TransID, .Items = Trans.EachGeneLevels}).ToArray)
        'Call Console.WriteLine("Applying association analysis....")
        'Dim Result = EncodesTransaction.AnalysisTransactions
        'Call Console.WriteLine("Analysis job done! start to export strong rules.....")
        'Dim InternalRuleString = Function(obj As KeyValuePair(Of String, Integer)) String.Format("{0}[{1}]", obj.Key, obj.Value)
        'Call Console.WriteLine("Transaction decoding...")
        'Dim StrongRules = (From rule In Result.StrongRules Select rule.Confidence, srX = (From obj In Encodes.Decode(rule.X) Select InternalRuleString(obj)).ToArray, srY = (From obj In Encodes.Decode(rule.Y) Select InternalRuleString(obj)).ToArray).ToArray
        'Dim RegulationSelection = (From item In StrongRules Where item.srX.Count = 1 OrElse item.srY.Count = 1 Select item).ToArray '只选择出事件为1的左右被调控的基因进行初筛
        'Dim Output = (From item In RegulationSelection Select New Rule(String.Join("; ", item.srX), String.Join("; ", item.srY), item.Confidence)).ToArray
        'Call Console.WriteLine("[Job Done!]")
        'Return Output
    End Function

    ''' <summary>
    ''' 首先进行关联分析，然后根据转录组数据筛选出调控事件
    ''' </summary>
    ''' <returns>有或者没有，高或者低</returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("regulation.calculation.bi")>
    Public Function CalculateRegulations(ChipData As IO.File) As Rule()
        ''在这里因为要保持顺序，所以在编码的阶段不可以使用并行化拓展
        ''获取基因列表，在第一列
        'Dim GeneExpressions = DataServicesExtension.LoadCsv(ChipData.Skip(1))
        'Dim GeneIDList = (From row In GeneExpressions Select row.Tag).ToArray
        'Call Console.WriteLine("[DEBUG] Chip data load done!  {0} genes and {1} experiments in the dataset.", GeneIDList.Count, ChipData.First.Count - 1)
        ''每一个基因都是事务之中的一个项目，每一次转录组实验都是一个Transaction

        'Call Console.WriteLine("AP Engine Services initialized!")
        ''进行表达值的Mapping
        ''首先生成每一次实验的列表，在进行Mapping
        'Dim ExperimentsTag As String() = ChipData.First.Skip(1).ToArray
        'Call Console.WriteLine("Experiments " & String.Join(";  ", ExperimentsTag))
        'Call Console.WriteLine("Start to generate transactions....")
        'Dim Transactions = (From idx As Integer In ExperimentsTag.Sequence
        '                    Let dat0 = (From GeneSample In GeneExpressions Select GeneSample.ChunkBuffer(idx)).ToArray
        '                    Let Avg As Double = dat0.Average * 0.85
        '                    Select TransID = ExperimentsTag(idx), EachGeneLevels = (From GeneSample In GeneExpressions Where GeneSample.ChunkBuffer(idx) >= Avg Select GeneSample.Tag).ToArray).ToArray '高表达量的基因设为1
        'Call Console.WriteLine("Encodings.....")
        'Dim MappingCodes = New Encoding(GeneIDList)
        'Call Console.WriteLine("There are {0} transaction tokens!", GeneIDList.Count)
        'Call Console.WriteLine("Encoding transactions....")
        'Dim EncodesTransaction = (From Trans In Transactions Let TransactionValue = MappingCodes.EncodingTransaction(Trans.EachGeneLevels) Select Trans.TransID, TransactionValue).ToArray
        'Call Console.WriteLine("Applying association analysis....")
        'Dim Result = ApEngine.AnalysisTransactions(Transactions:=(From obj In EncodesTransaction Select obj.TransactionValue).ToArray)
        'Call Console.WriteLine("Analysis job done! start to export strong rules.....")
        'Call Console.WriteLine("Transaction decoding...")
        'Dim StrongRules = (From rule In Result.StrongRules Select rule.Confidence, srX = MappingCodes.DecodesTransaction(rule.X), srY = MappingCodes.DecodesTransaction(rule.Y)).ToArray
        'Dim RegulationSelection = (From item In StrongRules Where item.srX.Count = 1 OrElse item.srY.Count = 1 Select item).ToArray '只选择出事件为1的左右被调控的基因进行初筛
        'Dim Output = (From item In RegulationSelection Select New Rule(String.Join("; ", item.srX), String.Join("; ", item.srY), item.Confidence)).ToArray
        'Call Console.WriteLine("[Job Done!]")
        'Return Output
    End Function

    ''' <summary>
    ''' Calculate the coefficient matrix of the gene expression profile data for co-expression analysis.(计算基因表达的相关性矩阵，用于基因共转录分析)
    ''' </summary>
    ''' <param name="ChipData">基因芯片数据</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Calculate(ChipData As IO.File) As ExprSamples()
        Dim DataSet As ExprSamples() = ToSamples(ChipData, True)
        Dim result = CalculatePccMatrix(DataSet)
        Return result
    End Function

    Public Function CalculatePccMatrix(DataSet As IEnumerable(Of ExprSamples)) As ExprSamples()
        Dim value = CalculateCorrelationMatrix(DataSet, AddressOf Correlations.GetPearson)
        Return value
    End Function

    Public Function CalculateSPccMatrix(DataSet As IEnumerable(Of ExprSamples)) As ExprSamples()
        Dim value = CalculateCorrelationMatrix(DataSet, AddressOf Correlations.Spearman)
        Return value
    End Function

    Public Function CalculateCorrelationMatrix(DataSet As IEnumerable(Of ExprSamples), correlations As ICorrelation) As ExprSamples()
        Dim object_HANDLES = (From i As Integer
                              In DataSet.Sequence
                              Select handle = i, item_data = DataSet(i)).ToArray
        Dim LQuery = (From obj
                      In object_HANDLES.AsParallel  ' 在后面会通过排序的方法回复原有的顺序，所以这里也可以使用并行化拓展
                      Let currentItem = obj.item_data
                      Let i = obj.handle
                      Let sv = (From hwnd In DataSet Select correlations(X:=currentItem.data, Y:=hwnd.data)).ToArray
                      Let Pccvalue = New ExprSamples With {
                          .locusId = currentItem.locusId,
                          .data = sv
                          }
                      Let value = New With {.Handle = i, .PccValue = Pccvalue}
                      Select value
                      Order By value.Handle Ascending).ToArray

        Return (From item In LQuery Select item.PccValue).ToArray
    End Function

    Public Function Load(csvFile As String) As ExprSamples()
        Dim Data = csvFile.ReadAllLines().Skip(1)
        Dim LQuery = (From line As String
                      In Data
                      Let tokens = line.Split(CChar(","))
                      Select New ExprSamples With {
                          .locusId = tokens(0),
                          .data = (From str As String
                                           In tokens.Skip(1)
                                   Select Val(str)).ToArray}).ToArray
        Return LQuery
    End Function

    Public Function Find(rowId As String, colId As String, pccData As ExprSamples()) As Double
        Dim GeneList = (From item As ExprSamples In pccData Select item.locusId).ToArray
        Dim row = pccData(Array.IndexOf(GeneList, rowId))
        Dim result = row(Array.IndexOf(GeneList, colId))
        Return result
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pccData"></param>
    ''' <param name="Id"></param>
    ''' <param name="cutOff">Pcc value cut off, all of the item have the value large than cutoff value will be return.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SortMaxliklyhood(pccData As ExprSamples(),
                                     Id As String,
                                     cutOff As Double,
                                     Optional AbsolutelyCutOff As Boolean = False) As KeyValuePair(Of String, Double)()
        Dim GeneList = (From item In pccData Select item.locusId).ToArray
        Dim idx = Array.IndexOf(GeneList, Id)
        If idx = -1 Then
            Return New KeyValuePair(Of String, Double)() {}
        End If
        Dim row = pccData(idx)
        Dim LQuery = (From i As Integer In row.data.Sequence
                      Let item = New KeyValuePair(Of Integer, Double)(i, row(i))
                      Select item
                      Order By item.Value Descending).ToArray
        If AbsolutelyCutOff Then
            Dim result = (From item In LQuery
                          Where System.Math.Abs(item.Value) >= cutOff
                          Let rtn_item = New KeyValuePair(Of String, Double)(GeneList(item.Key), item.Value)
                          Select rtn_item
                          Order By rtn_item.Key Ascending).ToArray
            Return result
        Else
            Dim result = (From item In LQuery
                          Where item.Value >= cutOff
                          Let rtn_item = New KeyValuePair(Of String, Double)(GeneList(item.Key), item.Value)
                          Select rtn_item
                          Order By rtn_item.Key Ascending).ToArray
            Return result
        End If
    End Function

    ''' <summary>
    ''' Merge a group set of gene chip data.(合并多个基因芯片数据)
    ''' </summary>
    ''' <param name="FileList"></param>
    ''' <param name="IdCol"></param>
    ''' <param name="ExprDataCols"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function MergeChipData(FileList As String(), IdCol As Integer, ExprDataCols As Integer()) As IO.File
        Dim DataFiles = (From File As String
                         In FileList
                         Select IO.File.Load(File)).ToArray
        Dim IdList = New List(Of String)
        For Each File As File In DataFiles
            Call IdList.AddRange((From row In File.Skip(1) Select row(IdCol)).ToArray)
        Next
        IdList = IdList.Distinct.AsList()
        Call IdList.Sort()

        Dim Table = New RowObject(IdList.Count) {}

        Dim TitleCollection As List(Of String) = New List(Of String)
        For Each File In DataFiles
            Call TitleCollection.AddRange(File.First.Takes(ExprDataCols))
        Next

        For i As Integer = 0 To IdList.Count - 1
            Dim Id As String = IdList(i)
            Dim row As IO.RowObject = New IO.RowObject From {Id}
            Dim DataRows = (From File As IO.File In DataFiles
                            Let search_result = File.FindAtColumn(Id, IdCol)
                            Where Not search_result.IsNullOrEmpty
                            Let result As String() = search_result.First.Takes(ExprDataCols, )
                            Let collection = Function() As String()
                                                 If (From str In result Where Not String.IsNullOrEmpty(str) Select 1).ToArray.Count < result.Count Then
                                                     Return Nothing
                                                 Else
                                                     Return result
                                                 End If
                                             End Function
                            Select collection()).ToArray
            Dim Null As Boolean = False

            For Each item In DataRows
                If item.IsNullOrEmpty Then
                    Null = True
                    Exit For
                End If
                Call row.AddRange(item)
            Next
            If Not Null Then
                Table(i + 1) = row
            Else
                Call $"EXCEPTION:: row object ""{Id}"" null value or part of the data is missing! Skip to process this row.".__DEBUG_ECHO
            End If
        Next

        Table(0) = New IO.RowObject From {"Id"}
        Table(0).AddRange(TitleCollection)

        Dim CsvFile As IO.File = Table
        Call CsvFile.Remove(Function(row As IO.RowObject) row Is Nothing)

        Return CsvFile
    End Function

    ''' <summary>
    ''' WGCNA包对芯片数据的格式要求，芯片数据矩阵的行表示为样本，列表示基因
    ''' </summary>
    ''' <param name="FileList"></param>
    ''' <param name="IdCol"></param>
    ''' <param name="ExprDataCols"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function MergeChipData2(FileList As String(), IdCol As Integer, ExprDataCols As Integer()) As IO.File
        Dim File As File = MergeChipData(FileList, IdCol, ExprDataCols).Skip(1).ToArray
        Dim IdRow As RowObject = (From row In File Select row.First()).ToArray
        Dim retFile As New File
        Call retFile.AppendLine(IdRow)
        Dim Table As RowObject() = (From i As Integer In File.First.Count.Sequence Select New RowObject).ToArray

        For i As Integer = 0 To Table.Count - 1
            Dim sampleId As Integer = i + 1
            Call Table(i).AddRange((From row In File Select row.Column(sampleId)).ToArray)
        Next
        Call retFile.AppendRange(Table)
        retFile.First.InsertAt("ID", 0)
        For i As Integer = 1 To retFile.Count - 1
            retFile(i).InsertAt("Samples_" & i, 0)
        Next

        Return retFile
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Data"></param>
    ''' <param name="CutOffValue">范围在0-1之间的最小阈值，建议取值0.6</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Reconstruction(Data As ExprSamples(), CutOffValue As Double) As Interaction(Of ExprSamples)()
        Dim GeneList = (From item In Data Select item.locusId).ToArray
        Dim RegulationList As List(Of Interaction(Of ExprSamples)) = New List(Of Interaction(Of ExprSamples))
        Dim Sequence = GeneList.Sequence

        For Each item In Data
            Dim list = (From i As Integer
                        In Sequence
                        Let val = item(i)
                        Where System.Math.Abs(val) >= CutOffValue
                        Let regulation = If(val < 0, "repression", "activation")
                        Select New Interaction(Of ExprSamples) With {
                            .Interaction = regulation,
                            .A = item,
                            .B = Data(i)}).ToArray
            Call RegulationList.AddRange(list)
        Next

        Return RegulationList.ToArray
    End Function

    'Public Function Reconstruction(ChipData As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File,
    '                               Model As SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.Model,
    '                               CutOffValue As Double) _
    '    As SMRUCC.genomics.ComponentModel.Interaction(Of PccMatrix.ItemObject)()

    '    Dim Data = Calculate(ChipData)
    '    Dim GeneDBLinks As Dictionary(Of String, GeneObject) =
    '        New Dictionary(Of String, GeneObject)
    '    For Each Gene In Model.BacteriaGenome.Genes
    '        Call GeneDBLinks.Add(Gene.AccessionId, Gene)
    '    Next

    '    Dim regulations = Reconstruction(Data, CutOffValue)
    '    Dim Link As Action(Of Integer) = Sub(idx As Integer)
    '                                         Dim Id As String = regulations(idx).ObjectB.GeneId '被调控对象
    '                                         Dim LQuery = (From tu In Model.BacteriaGenome.TransUnits Where tu.ContainsGene(Id) Select tu).ToArray
    '                                         Dim regRegulator As SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite = Nothing
    '                                         Dim Active As Boolean = String.Equals(regulations(idx).Interaction, "activation")

    '                                         If GeneDBLinks.ContainsKey(regulations(idx).ObjectA.GeneId) Then
    '                                             '     regRegulator = GeneDBLinks(regulations(idx).ObjectA.GeneId).TranslateProtein
    '                                         Else
    '                                             Return
    '                                         End If

    '                                         For jdx As Integer = 0 To LQuery.Count - 1
    '                                             Dim regulator = New SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Regulator With {
    '                                                 .UniqueId = regRegulator.UniqueId,
    '                                                                              .Name = regRegulator.CommonName}
    '                                             Call LQuery(jdx).Regulators.Add(regulator)
    '                                         Next
    '                                     End Sub
    '    For i As Integer = 0 To regulations.Count - 1
    '        Call Link(i)
    '    Next

    '    Return regulations
    'End Function
End Module
