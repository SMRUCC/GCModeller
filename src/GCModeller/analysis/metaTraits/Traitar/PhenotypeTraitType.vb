' ============================================================================
' PhenotypeTraitType.vb
'
' 表型元数据层：解析 phenotype_traits_and_types.csv，把每一种生物表型按其
' 数据类型映射为一个具体的 LibSVM 模型配置（SvmType + Parameter），并从
' TraitAnnotation 训练记录之中提取出对应的标签值。
'
'   boolean       -> SvmType.C_SVC      二分类
'   categorical   -> SvmType.C_SVC      多分类
'   numeric(cont) -> SvmType.EPSILON_SVR 回归
' ============================================================================

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.MachineLearning.SVM

Namespace Traitar

    ''' <summary>
    ''' 表型的数据类型，决定该表型所对应的 SVM 模型的种类
    ''' </summary>
    Public Enum TraitDataType
        ''' <summary>无法识别的数据类型，该表型不会被训练</summary>
        Unknown = 0
        ''' <summary>boolean 型表型，二分类问题</summary>
        [Boolean] = 1
        ''' <summary>categorical 型表型（factor），多分类问题</summary>
        Categorical = 2
        ''' <summary>numeric (continuous) 型表型，回归问题</summary>
        Numeric = 3
    End Enum

    ''' <summary>
    ''' phenotype_traits_and_types.csv 之中的一行记录：定义了一种生物表型
    ''' 及其数据类型。每一种表型在重构之后的 Traitar 模块之中可能会对应着
    ''' 一个具体的 SVM 模型实例。
    ''' </summary>
    Public Class PhenotypeTrait

        ''' <summary>表型名称，同时是训练数据表之中的 trait_name 字段</summary>
        Public Property trait_name As String
        ''' <summary>
        ''' 原始的数据类型字符串：``boolean`` / ``categorical`` / 
        ''' ``numeric (continuous)``
        ''' </summary>
        Public Property data_type As String
        ''' <summary>计量单位，例如 ``%``, ``boolean``, ``factor``</summary>
        Public Property unit As String
        ''' <summary>主分类</summary>
        Public Property group_1_category As String
        ''' <summary>次分类</summary>
        Public Property group_2_subcategory As String

        Public Overrides Function ToString() As String
            Return $"[{data_type}] {trait_name}"
        End Function

        ''' <summary>
        ''' 解析得到该表型的数据类型枚举值
        ''' </summary>
        Public Function GetDataType() As TraitDataType
            Return PhenotypeTraits.ParseDataType(data_type)
        End Function

        ''' <summary>
        ''' 该表型所对应的 LibSVM 模型类型：数值型做回归，其余做分类
        ''' </summary>
        Public Function GetSvmType() As SvmType
            If GetDataType() = TraitDataType.Numeric Then
                Return SvmType.EPSILON_SVR
            Else
                Return SvmType.C_SVC
            End If
        End Function

        ''' <summary>该表型是否是一个回归问题（numeric 型）</summary>
        Public Function IsRegression() As Boolean
            Return GetDataType() = TraitDataType.Numeric
        End Function

        ''' <summary>
        ''' 创建用于训练该表型模型的 LibSVM 参数对象
        ''' </summary>
        ''' <param name="dimensions">
        ''' 特征维度数量（Pfam 词表长度），用于计算 RBF 核的 gamma 参数；
        ''' 小于等于 0 时使用默认的 0.5。
        ''' </param>
        ''' <param name="kernel">核函数类型，默认为 RBF</param>
        Public Function CreateParameter(Optional dimensions As Integer = 0,
                                        Optional kernel As KernelType = KernelType.RBF) As Parameter
            Return PhenotypeTraits.CreateParameter(Me, dimensions, kernel)
        End Function

    End Class

    Public Module PhenotypeTraits

        ''' <summary>
        ''' 把 csv 之中的数据类型字符串解析为枚举值
        ''' </summary>
        Public Function ParseDataType(data_type As String) As TraitDataType
            If data_type Is Nothing Then
                Return TraitDataType.Unknown
            End If

            Dim tag As String = data_type.Trim.ToLower

            If tag = "boolean" Then
                Return TraitDataType.Boolean
            ElseIf tag = "categorical" Then
                Return TraitDataType.Categorical
            ElseIf tag.StartsWith("numeric") OrElse tag.Contains("continuous") Then
                Return TraitDataType.Numeric
            Else
                Return TraitDataType.Unknown
            End If
        End Function

        ''' <summary>
        ''' 加载 phenotype_traits_and_types.csv 表型定义表
        ''' </summary>
        ''' <param name="csv">csv 文件的文件路径</param>
        Public Function LoadTable(csv As String) As PhenotypeTrait()
            Return csv.LoadCsv(Of PhenotypeTrait)(mute:=True).ToArray
        End Function

        ''' <summary>
        ''' 创建用于训练指定表型模型的 LibSVM 参数对象
        ''' </summary>
        ''' <param name="trait">目标表型的元数据</param>
        ''' <param name="dimensions">特征维度数，用于计算 RBF 的 gamma</param>
        ''' <param name="kernel">核函数类型</param>
        Public Function CreateParameter(trait As PhenotypeTrait,
                                        Optional dimensions As Integer = 0,
                                        Optional kernel As KernelType = KernelType.RBF) As Parameter

            ' gamma 参数千万不可以为零，否则无法进行分类
            Dim gamma As Double = If(dimensions > 0, 1.0 / dimensions, 0.5)

            Return New Parameter With {
                .svmType = trait.GetSvmType(),
                .kernelType = kernel,
                .degree = 3,
                .gamma = gamma,
                .coefficient0 = 0,
                .nu = 0.5,
                .cacheSize = 40,
                .c = 1,
                .EPS = 0.001,
                .P = 0.1,
                .shrinking = True,
                .probability = False
            }
        End Function

        ''' <summary>
        ''' 从一条 TraitAnnotation 训练记录之中提取出标签值
        ''' </summary>
        ''' <param name="trait">训练数据集之中的一行记录</param>
        ''' <param name="type">该表型的数据类型</param>
        ''' <returns>
        ''' 数值型返回 mean（不可解析时回退 median / consensus_value）；
        ''' 布尔型与分类型返回 consensus_value；缺失值（NA/空）返回 Nothing。
        ''' </returns>
        <Extension>
        Public Function GetLabelValue(trait As TraitAnnotation, type As TraitDataType) As String
            Dim raw As String = Nothing

            Select Case type
                Case TraitDataType.Numeric
                    ' 数值型表型的 consensus_value 通常为 NA，真实数值保存在
                    ' mean / median / minimum / maximum 这几个统计字段之中
                    For Each field As String In {trait.mean, trait.median, trait.consensus_value}
                        If IsNumericValue(field) Then
                            raw = field
                            Exit For
                        End If
                    Next
                Case Else
                    raw = trait.consensus_value
            End Select

            Return CleanLabel(raw)
        End Function

        ''' <summary>
        ''' 清理标签文本：去掉空白，并把缺失值统一转换为 Nothing
        ''' </summary>
        Public Function CleanLabel(raw As String) As String
            If raw Is Nothing Then
                Return Nothing
            End If

            raw = raw.Trim

            If raw.Length = 0 Then
                Return Nothing
            ElseIf raw.Equals("NA", StringComparison.OrdinalIgnoreCase) OrElse
                raw.Equals("N/A", StringComparison.OrdinalIgnoreCase) OrElse
                raw.Equals("-", StringComparison.OrdinalIgnoreCase) OrElse
                raw.Equals("null", StringComparison.OrdinalIgnoreCase) Then

                Return Nothing
            End If

            Return raw
        End Function

        ''' <summary>
        ''' 判断字符串是否是一个合法的数值文本
        ''' </summary>
        Public Function IsNumericValue(text As String) As Boolean
            If text Is Nothing Then
                Return False
            End If

            Dim value As Double = 0

            Return Double.TryParse(text.Trim, value)
        End Function

    End Module
End Namespace
