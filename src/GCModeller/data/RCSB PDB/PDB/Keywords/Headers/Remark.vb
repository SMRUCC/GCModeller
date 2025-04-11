Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace Keywords

    Public Enum RemarkCategory
        ' 主要分类编码
        GeneralText = 1          ' REMARK 1: 自由文本注释
        ExperimentalMethod = 2   ' REMARK 2: 实验方法（如X射线、NMR）
        Crystallography = 3      ' REMARK 3: 结晶学数据（分辨率、R因子）
        Refinement = 4           ' REMARK 4: 结构修正参数

        DataProcessing = 5

        DataCollection = 200     ' REMARK 200-299: 数据收集参数（探测器、波长等）
        Validation = 300         ' REMARK 300: 模型验证（Ramachandran图）
        Superimposition = 400    ' REMARK 400: 结构叠加或比对

        ProcessingInformation = 100   ' REMARK 100: PDB处理元数据
        Crystallization = 280         ' REMARK 280: 晶体溶剂含量/结晶条件
        Symmetry = 290                ' REMARK 290: 晶体对称性操作
        BiomoleculeAssembly = 350     ' REMARK 350: 生物分子组装变换矩阵
        MissingResiduesAtoms = 465    ' REMARK 465/470: 缺失残基/原子
        GeometryCheck = 500           ' REMARK 500: 几何/立体化学检查
        RelatedEntries = 900          ' REMARK 900: 相关PDB条目

        NoncovalentInteractions = 600
        HydrophobicInteractions = 700
        OtherImportantFeatures = 800

        Other = 999              ' 其他未分类的REMARK
    End Enum

    Public Class Remark : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_REMARK
            End Get
        End Property

        Public Property RemarkText As Dictionary(Of RemarkCategory, String)

        Dim cache As New List(Of NamedValue(Of String))

        Friend Shared Function Append(ByRef remark As Remark, str As String) As Remark
            If remark Is Nothing Then
                remark = New Remark
            End If
            remark.cache.Add(str.GetTagValue(" ", trim:=True, failureNoName:=False))
            Return remark
        End Function

        Friend Overrides Sub Flush()
            RemarkText = cache _
                .GroupBy(Function(line) GetRemarkCategory(Integer.Parse(line.Name))) _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return a.Select(Function(t) t.Value).JoinBy(vbCrLf)
                              End Function)
        End Sub

        Public Shared Function GetRemarkCategory(code As Integer) As RemarkCategory
            Select Case code
                Case 1 : Return RemarkCategory.GeneralText
                Case 2 : Return RemarkCategory.ExperimentalMethod
                Case 3 : Return RemarkCategory.Crystallography
                Case 4 : Return RemarkCategory.Refinement
                Case 5 : Return RemarkCategory.DataProcessing
                Case 200 To 299
                    Return RemarkCategory.DataCollection
                Case 300
                    ' 根据实际内容覆盖标准分类：此处REMARK 300指向生物组装
                    Return RemarkCategory.BiomoleculeAssembly
                Case 350 : Return RemarkCategory.BiomoleculeAssembly
                Case 400 : Return RemarkCategory.Superimposition
                Case 100 : Return RemarkCategory.ProcessingInformation
                Case 280 : Return RemarkCategory.Crystallization
                Case 290 : Return RemarkCategory.Symmetry
                Case 465, 470
                    Return RemarkCategory.MissingResiduesAtoms
                Case 500 : Return RemarkCategory.GeometryCheck

                Case 600 : Return RemarkCategory.NoncovalentInteractions
                Case 700 : Return RemarkCategory.HydrophobicInteractions
                Case 800 : Return RemarkCategory.OtherImportantFeatures

                Case 900 : Return RemarkCategory.RelatedEntries

                Case Else
                    Return RemarkCategory.Other
            End Select
        End Function
    End Class
End Namespace