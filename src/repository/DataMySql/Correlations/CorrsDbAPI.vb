#Region "Microsoft.VisualBasic::8857af36f4315f6c5da904c897307478, ..\GCModeller\analysis\annoTools\DataMySql\Correlations\CorrsDbAPI.vb"

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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Oracle.LinuxCompatibility.MySQL
Imports SMRUCC.genomics.Analysis
Imports SMRUCC.genomics.Analysis.RNA_Seq
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools.WGCNA
Imports SMRUCC.genomics.Analysis.RNA_Seq.WGCNA
Imports SMRUCC.genomics.Data.Model_Repository.MySQL.Tables

<PackageNamespace("GCModeller.Gene.Correlations",
                  Publisher:="xie.guigang@gcmodeller.org",
                  Category:=APICategories.ResearchTools)>
Public Module CorrsDbAPI

    Public Const PCC_SIGNIFICANT_THAN As String = "SELECT * FROM correlations.xcb where (g1_entity = '{0}' or g2_entity = '{0}') and abs(pcc) >= {1};"
    Public Const PCC_GREATER_THAN As String = "SELECT * FROM correlations.xcb where (g1_entity = '{0}' or g2_entity = '{0}') and pcc >= {1};"

    <ExportAPI("DB.Init()")>
    Public Function DBInit() As Correlations
        Return New Correlations
    End Function

    <ExportAPI("Correlates")>
    Public Function GetCorrelates(Db As Correlations, id1 As String, id2 As String) As xcb
        Dim correlation As xcb = Db.GetCorrelation(id1, id2)
        Return correlation
    End Function

    ''' <summary>
    ''' 这个函数是自动计算WGCNA相关性的
    ''' </summary>
    ''' <param name="raw">第一列是基因的编号列表，其他的列都是基因的表达数据</param>
    ''' <param name="uri"></param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("Write.Database")>
    Public Function WriteDatabase(raw As DocumentStream.File, Optional uri As ConnectionUri = Nothing) As Boolean
        Dim WGCNA As String = RTools.WGCNA.CallInvoke(raw.FilePath, raw.FilePath.ParentPath & "/annotations.csv")
        Return WriteDatabase(raw, WGCNA, uri)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="raw">第一列是基因的编号列表，其他的列都是基因的表达数据</param>
    ''' <param name="uri"></param>
    ''' <returns></returns>
    ''' <param name="WGCNA">WGCNA脚本所计算出来的Cytoscape的边文件</param>
    <ExportAPI("Write.Database")>
    Public Function WriteDatabase(raw As DocumentStream.File, WGCNA As String, Optional uri As ConnectionUri = Nothing) As Boolean
        Dim WGCNA_MAT = FastImports(WGCNA)
        Dim Samples = MatrixAPI.ToSamples(raw, True)
        Dim PccMatrix = CreatePccMAT(Samples)
        Dim sPccMAT = CreateSPccMAT(Samples)

        Try
            Call __commits(GetMySQLClient(uri, DBName:="correlations"), WGCNA_MAT, PccMatrix, sPccMAT)
        Catch ex As Exception
            Call App.LogException(ex)
            Return False
        End Try

        Return True
    End Function

    <ExportAPI("Write.Database")>
    Public Function WriteDatabase(raw As String, WGCNA As String, Optional uri As ConnectionUri = Nothing) As Boolean
        Return WriteDatabase(DocumentStream.File.Load(raw), WGCNA, uri)
    End Function

    ''' <summary>
    ''' 先排序在进行遍历，不需要再进行随机查找了，太耗费时间了
    ''' </summary>
    ''' <param name="MySQL"></param>
    ''' <param name="WGCNA_MAT"></param>
    ''' <param name="PccMatrix"></param>
    ''' <param name="sPccMAT"></param>
    Private Sub __commits(MySQL As MySQL, WGCNA_MAT As WGCNAWeight, PccMatrix As PccMatrix, sPccMAT As PccMatrix)
        Call "Create data sets and commit transactions...".__DEBUG_ECHO

        For i As Integer = 0 To PccMatrix.lstGenes.Length - 1  ' pcc和spcc这两个对象的基因的排布顺序是一致的
            Dim pcc = PccMatrix(i), spp = sPccMAT(i)
            Dim g1 As String = pcc.locusId
            Dim Transaction As New List(Of xcb)

            For ii2 As Integer = 0 To PccMatrix.lstGenes.Length - 1  ' 由于在创建矩阵的时候没有打破一一对应的关系，所以这里可以使用遍历的方式来替代随机查询加快计算提交的过程
                Dim g2 As String = PccMatrix.lstGenes(ii2)
                Dim WGCNA = WGCNA_MAT.Find(g1, g2)
                Dim weight As Double = If(WGCNA Is Nothing, 0, WGCNA.Weight)
                Dim line As New xcb With {
                    .g1_entity = g1,
                    .g2_entity = g2,
                    .pcc = pcc.Values(ii2),
                    .spcc = spp.Values(ii2),
                    .wgcna_weight = weight
                }
                Call Transaction.Add(line)
            Next

            Dim exp As Exception = Nothing
            Dim t As String = String.Join(vbCrLf, Transaction.ToArray(Function(ds) ds.GetInsertSQL))

            If Not MySQL.CommitTransaction(t, exp) Then
                Call App.LogException(exp)
                Call exp.PrintException
            Else
                Call $"[{g1}] Job Done!".__DEBUG_ECHO
            End If
        Next

        Call "Job Done!".__DEBUG_ECHO
    End Sub

    ' 下面的代码是使用随机查找来得到数据的，但是效率非常低

    'Dim DataSet As Generic.IEnumerable(Of GCModeller.Workbench.DatabaseServices.Model_Repository.MySQL.Tables.xcb) =
    '    PccMatrix.lstGenes.ToArray(
    '        Function(p1) _
    '            MySQL.CommitTransaction(
    '                String.Join(vbCrLf, (
    '                    From p2 As String
    '                    In PccMatrix.lstGenes
    '                    Select New SMRUCC.genomics.GCModeller.Workbench.DatabaseServices.Model_Repository.MySQL.Tables.xcb With {
    '                        .pcc = PccMatrix.GetValue(p1, p2, Parallel:=False),
    '                        .g1_entity = p1,
    '                        .g2_entity = p2,
    '                        .spcc = sPccMAT.GetValue(p1, p2, Parallel:=False),
    '                        .wgcna_weight = WGCNA_MAT.GetValue(p1, p2, Parallel:=False)}).ToArray _
    '                            .ToArray(Function(row) row.GetInsertSQL, Parallel:=False))), Parallel:=True).ToArray

    'Call "Job Done!".__DEBUG_ECHO
End Module
