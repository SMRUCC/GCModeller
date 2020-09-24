#Region "Microsoft.VisualBasic::9e87f7f606ac1743ccde678b05434435, analysis\RNA-Seq\Toolkits.RNA-Seq\Correlations\MatrixAPI.vb"

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

    ' Module MatrixAPI
    ' 
    '     Function: __checkThreshold, __createMatrix, __getData, CreateCorrelationMatrix, (+2 Overloads) CreatePccMAT
    '               (+2 Overloads) CreateSPccMAT, J, (+2 Overloads) Sampling, SavePccMatrix, SaveRegulationNetwork
    '               Similarity, ToSamples, TryGenerateDraftsRegulationNetwork
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Parallel.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT

<Package("PCC.Matrix", Publisher:="xie.guigang@gmail.com", Category:=APICategories.UtilityTools)>
Public Module MatrixAPI

    ''' <summary>
    ''' 将目标芯片数据转换为每一个基因的计算样本，在本方法之中没有涉及到将目标数据集计算为相关系数矩阵的操作
    ''' </summary>
    ''' <param name="Data"></param>
    ''' <param name="FirstRowTitle">Is first row in the table is the title description row, not a valid data row?(数据集中的第一行是否为标题行)</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("Get.ChipdataSamples")>
    Public Function ToSamples(Data As IO.File, FirstRowTitle As Boolean) As ExprSamples()
        Dim ds = Data.ToArray

        If FirstRowTitle Then
            ds = ds.Skip(1).ToArray
        End If

        Call "Start parsing samples data from the raw matrix...".__DEBUG_ECHO

        Dim sw As Stopwatch = Stopwatch.StartNew
        Dim lstId As String() = ds.Select(Function(row) row.First).ToArray
        Dim LQuery = (From row As RowObject
                      In ds.AsParallel   ' 在后面通过查字典的方式保证一一对应关系，所以这里可以使用并行化
                      Let sample As ExprSamples = ExprSamples.ConvertObject(row)
                      Select sample).ToDictionary(Function(sample) sample.locusId)
        Call "Ordering data...".__DEBUG_ECHO
        Dim samples As ExprSamples() = lstId.Select(Function(id) LQuery(id))

        Call $"Parsing samples work complete!   {sw.ElapsedMilliseconds}ms....".__DEBUG_ECHO

        Return samples
    End Function

    ''' <summary>
    ''' Paiwise sample redundancy
    ''' </summary>
    ''' <param name="sample1"></param>
    ''' <param name="sample2"></param>
    ''' <param name="C">cut-off threshold, We used 0.4 for this threshold, which is roughly optimized</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("Sample.Redundancy")>
    Public Function J(sample1 As ExprSamples,
                      sample2 As ExprSamples,
                      <Parameter("C", "Cut-off threshold, We used 0.4 for this threshold, which is roughly optimized")>
                      Optional C As Double = 0.4) As Double
        Dim pcc As Double = Correlations.GetPearson(sample1.data, sample2.data)
        Return Math.Max(0, pcc - C) / (1 - C)
    End Function

    ''' <summary>
    ''' 对每一个基因对之间计算皮尔森相关系数，并返回得到的矩阵
    ''' </summary>
    ''' <param name="rawExpr"></param>
    ''' <param name="FirstLineTitle"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("Matrix.Create")>
    Public Function CreatePccMAT(rawExpr As IO.File, Optional FirstLineTitle As Boolean = True) As PccMatrix
        Call "Start to create the pcc matrix!".__DEBUG_ECHO

        Dim PccValues = GenesCOExpr.CalculatePccMatrix(DataSet:=ToSamples(rawExpr, FirstLineTitle))
        Dim MAT = __createMatrix(PccValues)
        Return MAT
    End Function

    <ExportAPI("Matrix.Create")>
    Public Function CreatePccMAT(samples As IEnumerable(Of ExprSamples)) As PccMatrix
        Dim PccValues = GenesCOExpr.CalculatePccMatrix(samples)
        Dim MAT = __createMatrix(PccValues)
        Return MAT
    End Function

    Private Function __createMatrix(dataset As ExprSamples()) As PccMatrix
        Dim DictPccValues As Dictionary(Of String, ExprSamples) =
            New Dictionary(Of String, ExprSamples)

        For Each sample As ExprSamples In dataset
            Call DictPccValues.Add(sample.locusId.ToUpper, sample)
        Next

        Dim PccObject As PccMatrix = New PccMatrix(DictPccValues, DictPccValues.Keys.ToArray)
        Call "Create Pcc matrix job done!".__DEBUG_ECHO

        Return PccObject
    End Function

    <ExportAPI("sPcc_Matrix.Create", Info:="Create a spearman correlation matrix.")>
    Public Function CreateSPccMAT(samples As IEnumerable(Of ExprSamples)) As PccMatrix
        Call "Start to create the spcc matrix!".__DEBUG_ECHO

        Dim sPccValues As ExprSamples() = GenesCOExpr.CalculateSPccMatrix(samples)
        Dim MAT = __createMatrix(sPccValues)
        Return MAT
    End Function

    ''' <summary>
    ''' 计算得到斯皮尔曼相关性矩阵
    ''' </summary>
    ''' <param name="ChipData"></param>
    ''' <param name="FirstLineTitle"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("sPcc_Matrix.Create", Info:="Create a spearman correlation matrix.")>
    Public Function CreateSPccMAT(ChipData As IO.File,
                                  Optional FirstLineTitle As Boolean = True) As PccMatrix
        Call "Start to create the spcc matrix!".__DEBUG_ECHO

        Dim sPccValues As ExprSamples() =
            GenesCOExpr.CalculateSPccMatrix(DataSet:=ToSamples(ChipData, FirstLineTitle))
        Dim MAT = __createMatrix(sPccValues)
        Return MAT
    End Function

    ''' <summary>
    ''' 计算PCC和SPCC的混合矩阵，这个函数会首先计算PCC，当PCC的值过低的时候，会计算SPCC的值来替代
    ''' </summary>
    ''' <param name="ChipData"></param>
    ''' <param name="pcc_th1"></param>
    ''' <param name="pcc_th2"></param>
    ''' <param name="spcc_th1"></param>
    ''' <param name="spcc_th2"></param>
    ''' <param name="FirstLineTitle"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("Correlation.Calculate",
               Info:="Calculates the pcc and spcc mix matrix, if the calculated pcc value is too low than the threshold then the function will try using the spcc value to replace it. 
               All of the factor which is not satisfied with pcc and spcc threshold will be set as ZERO.")>
    Public Function CreateCorrelationMatrix(ChipData As IO.File,
                                            Optional pcc_th1 As Double = 0.85,
                                            Optional pcc_th2 As Double = -0.65,
                                            Optional spcc_th1 As Double = 0.75,
                                            Optional spcc_th2 As Double = -0.65,
                                            Optional FirstLineTitle As Boolean = True) As PccMatrix
        Dim hwd_SPcc = Function() CreateSPccMAT(ChipData, FirstLineTitle)
        Dim hwnd_Pcc = Function() CreatePccMAT(ChipData, FirstLineTitle)
        Dim ay_SPcc = hwd_SPcc.BeginInvoke(Nothing, Nothing)
        Dim ay__Pcc = hwnd_Pcc.BeginInvoke(Nothing, Nothing)

        Dim SPcc = hwd_SPcc.EndInvoke(ay_SPcc)
        Dim Pcc = hwnd_Pcc.EndInvoke(ay__Pcc)

        Dim LQueryCache = From i As SeqValue(Of ExprSamples)
                          In Pcc.PccValues.SeqIterator
                          Select i,
                              SPccVector = SPcc.PccValues(i)
        Dim LQuery As IEnumerable(Of ExprSamples) =
            LQuerySchedule.LQuery(LQueryCache, Function(x) __checkThreshold(x.i.value, x.SPccVector, pcc_th1, pcc_th2, spcc_th1, spcc_th2), 20000)

        Return New PccMatrix(lstId:=Pcc.lstGenes, samples:=LQuery.ToDictionary(Function(x) x.locusId)) With {
            .PCC_SPCC_MixedType = True
        }
    End Function

    Private Function __checkThreshold(Pcc As ExprSamples,
                                      SPcc As ExprSamples,
                                      pcc_th1 As Double,
                                      pcc_th2 As Double,
                                      spcc_th1 As Double,
                                      spcc_th2 As Double) As ExprSamples
        For i As Integer = 0 To Pcc.Count - 1
            Dim n As Double = Pcc(i)
            If n >= pcc_th1 OrElse n <= pcc_th2 Then
                Continue For
            End If

            n = SPcc(i)       '如果Pcc不符合标准则使用SPcc来代替

            If n >= spcc_th1 OrElse n <= spcc_th2 Then
                Pcc(i) = n  '如果SPcc符合标准则进行替换
            Else
                Pcc(i) = 0  '将所有不符合条件的参数都删除
            End If
        Next

        Return Pcc '由于是对Pcc对象进行操作的，所以结果返回Pcc
    End Function

    ''' <summary>
    ''' 比较芯片数据和计算数据的相似性
    ''' </summary>
    ''' <param name="BenchmarkQuery">由实验所获取得到的基因芯片基准数据</param>
    ''' <param name="VcellValidate">所计算出来的需要进行验证的计算数据</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("Similarity.Calculate")>
    Public Function Similarity(BenchmarkQuery As PccMatrix, VcellValidate As PccMatrix) As IO.File
        Dim BenchmarkId As String() = New String()() {BenchmarkQuery.lstGenes, VcellValidate.lstGenes}.Intersection   '从基准数据之中移除在校正数据之中缺失的数据
        Dim NewMatrix As List(Of ExprSamples) = New List(Of ExprSamples)

        For Each strid As String In BenchmarkId  '初始化矩阵 
            Dim PccDeviationVector As List(Of Double) = New List(Of Double)

            For Each Interactionid As String In BenchmarkId
                Dim BenchmarkPcc As Double = BenchmarkQuery.GetValue(strid, Interactionid)
                Dim vcellPccvalue As Double = VcellValidate.GetValue(strid, Interactionid)
                Dim b As Double = (vcellPccvalue - BenchmarkPcc) / BenchmarkPcc         '计算差值，之后求取对基准数据的偏差
                Call PccDeviationVector.Add(b)
            Next

            Dim newSample As New ExprSamples With {
                .locusId = strid,
                .data = PccDeviationVector.ToArray
            }

            Call NewMatrix.Add(newSample)
        Next

        Dim CsvResult = ExprSamples.CreateFile(NewMatrix)

        Dim ChunkBuffer As List(Of Double) = New List(Of Double)
        For Each Line In NewMatrix
            Call ChunkBuffer.AddRange(Line.data)
        Next
        Dim avg = ChunkBuffer.Average
        Dim std = ChunkBuffer.StdError

        Call FileIO.FileSystem.WriteAllText("./debug.log", String.Format("Average:= {0};   StdError:= {1};" & vbCrLf, avg, std), append:=True)

        Return CsvResult
    End Function

    <ExportAPI("Create.DraftNetwork")>
    Public Function TryGenerateDraftsRegulationNetwork(PccMatrix As PccMatrix,
                                                       Door As String,
                                                       RegulatorList As Generic.IEnumerable(Of String),
                                                       pcc As Double) As SimpleRegulation()
        Dim Regulations As List(Of SimpleRegulation) = New List(Of SimpleRegulation)
        Dim Operons = SMRUCC.genomics.Assembly.DOOR.Load(Door).DOOROperonView
        Dim OperonPromoterGenes As String() = (From item In Operons.Operons Select item.InitialX.Synonym).ToArray

        For Each RegulatorId As String In RegulatorList
            Dim LQuery As String() = (From GeneId As String
                                      In OperonPromoterGenes
                                      Let p As Double = PccMatrix.GetValue(GeneId, RegulatorId)
                                      Where p >= pcc OrElse p <= -0.65
                                      Select GeneId).ToArray
            Dim OperonList As Assembly.DOOR.Operon() = Operons.Select(LQuery)
            Dim array As SimpleRegulation() = (From operon As Assembly.DOOR.Operon
                                               In OperonList
                                               Let reg As SimpleRegulation = New SimpleRegulation With {
                                                   .Regulator = RegulatorId,
                                                   .Operon = operon.Key,
                                                   .PccValue = PccMatrix.GetValue(operon.InitialX.Synonym, RegulatorId)
                                               }
                                               Select reg).ToArray
            Call Regulations.AddRange(array)
        Next

        Return Regulations.ToArray
    End Function

    <ExportAPI("Write.Csv.DraftNetwork")>
    Public Function SaveRegulationNetwork(data As SimpleRegulation(), saveto As String) As Boolean
        Return data.SaveTo(saveto, False)
    End Function

    ''' <summary>
    ''' (ShellScript API) <see cref="ExprSamples.CreateFile"></see>
    ''' </summary>
    ''' <param name="Pccmatrix"></param>
    ''' <param name="saveto"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("Write.Csv.PccMatrix")>
    Public Function SavePccMatrix(Pccmatrix As PccMatrix,
                                  <Parameter("Path.Save")> SaveTo As String) As IO.File
        Dim CsvResult As IO.File = ExprSamples.CreateFile(Pccmatrix.PccValues)
        Call CsvResult.Save(SaveTo, Encoding.ASCII)
        Return CsvResult
    End Function

    ''' <summary>
    ''' 从虚拟细胞计算出来的转录组数据之中进行采样
    ''' </summary>
    ''' <param name="datas">数据文件夹，里面应该包含有不同的实验条件之下所得到的转录组计算数据</param>
    ''' <param name="TimeId">采样的时间编号</param>
    ''' <param name="FirstLineTitle">第一行是否为标题行</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("Sampling")>
    Public Function Sampling(datas As String, TimeId As Integer, Optional FirstLineTitle As Boolean = False) As IO.File
        Dim CsvBuffer = (From path As String
                         In FileIO.FileSystem.GetFiles(datas, FileIO.SearchOption.SearchTopLevelOnly, "*.csv")
                         Select file = IO.File.Load(path), filePath = path).ToArray
        Dim idBufs As String() = (From file In CsvBuffer Select (From row In If(FirstLineTitle, file.file.Skip(1).ToArray, file.file.ToArray) Select row.First).ToArray).Intersection
        Dim CsvResult As New IO.File
        Dim RowBuffer As New IO.RowObject
        Call RowBuffer.Add("GeneId")
        Call RowBuffer.AddRange((From file In CsvBuffer Select file.FilePath.BaseName).ToArray)
        Call CsvResult.AppendLine(RowBuffer)

        TimeId += 1

        For Each sId As String In idBufs
            Dim RowCollection = (From file In CsvBuffer Select file.file.FindAtColumn(sId, 0).First).ToArray
            RowBuffer = New IO.RowObject From {sId}
            Call RowBuffer.AddRange((From fileLine In RowCollection Select fileLine(TimeId)).ToArray)
            Call CsvResult.AppendLine(RowBuffer)
        Next

        Return CsvResult
    End Function

    <ExportAPI("Get.Sample")>
    Public Function Sampling(from_csv As String, start As Integer, ends As Integer, Optional FirstLineTitle As Boolean = False) As IO.File
        Dim CsvData As IO.File = IO.File.Load(from_csv)
        Dim d As Integer = ends - start
        Dim LQuery = (From row As IO.RowObject
                      In If(FirstLineTitle, CsvData.Skip(1).ToArray, CsvData.ToArray)
                      Select CType(__getData(row, start, d:=d), IO.RowObject)).ToArray
        Return LQuery
    End Function

    Private Function __getData(row As IO.RowObject, start As Integer, d As Integer) As String()
        Dim Chunkbuffer As List(Of String) = New List(Of String) From {row.First}
        Dim data As String() = row.Skip(start).ToArray
        Call Chunkbuffer.AddRange(data.Take(d).ToArray)
        Return Chunkbuffer.ToArray
    End Function
End Module
