Imports System.Xml.Serialization

Namespace v2

    ''' <summary>
    ''' 转录调控组：一个调控因子以及其所调控的全部转录单元
    ''' </summary>
    <XmlType("regulon", [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class Regulon

        <XmlAttribute> Public Property id As String

        <XmlAttribute> Public Property name As String

        ''' <summary>
        ''' 调控因子(转录因子)的蛋白质编号
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property regulator As String

        ''' <summary>
        ''' 被当前调控组所调控的转录单元或者基因的编号列表
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("operon")> Public Property members As String()

        <XmlElement> Public Property note As String

        Public Overrides Function ToString() As String
            Return $"[{id}] {regulator}"
        End Function

    End Class

    ''' <summary>
    ''' DNA结合位点(转录因子结合位点)
    ''' </summary>
    <XmlType("bindingSite", [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class BindingSite

        <XmlAttribute> Public Property id As String

        ''' <summary>
        ''' 结合位点所调控的转录单元编号
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property operonId As String

        <XmlAttribute> Public Property gene As String

        <XmlAttribute> Public Property left As Integer

        <XmlAttribute> Public Property right As Integer

        <XmlAttribute> Public Property strand As String

        ''' <summary>
        ''' 结合位点的序列
        ''' </summary>
        ''' <returns></returns>
        <XmlText> Public Property sequence As String

        ''' <summary>
        ''' 结合位点的打分(例如位置权重矩阵的打分)
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property score As Double

        ''' <summary>
        ''' 结合位点的类型：例如 ``TFBS``、``Promoter``、``Terminator``
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property siteType As String

        Public Overrides Function ToString() As String
            Return $"{id}@[{left},{right}]"
        End Function

    End Class

    ''' <summary>
    ''' 启动子(转录起始位点)
    ''' </summary>
    <XmlType("promoter", [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class Promoter

        <XmlAttribute> Public Property id As String

        <XmlAttribute> Public Property operonId As String

        <XmlAttribute> Public Property left As Integer

        <XmlAttribute> Public Property right As Integer

        <XmlAttribute> Public Property strand As String

        ''' <summary>
        ''' 转录起始位点的绝对位置(+1位点)
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property tss As Integer

        ''' <summary>
        ''' 识别当前启动子的sigma因子编号
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property sigmaFactor As String

        ''' <summary>
        ''' -10区的坐标区间
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property minus10Left As Integer
        <XmlAttribute> Public Property minus10Right As Integer

        ''' <summary>
        ''' -35区的坐标区间
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property minus35Left As Integer
        <XmlAttribute> Public Property minus35Right As Integer

        <XmlElement> Public Property note As String

        Public Overrides Function ToString() As String
            Return $"{id}@{tss}"
        End Function

    End Class

    ''' <summary>
    ''' 转录终止子
    ''' </summary>
    <XmlType("terminator", [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class Terminator

        <XmlAttribute> Public Property id As String

        <XmlAttribute> Public Property operonId As String

        <XmlAttribute> Public Property left As Integer

        <XmlAttribute> Public Property right As Integer

        <XmlAttribute> Public Property strand As String

        ''' <summary>
        ''' 终止发生的位点
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property position As Integer

        <XmlElement> Public Property note As String

        Public Overrides Function ToString() As String
            Return $"{id}@{position}"
        End Function

    End Class

    ''' <summary>
    ''' 蛋白质序列上面的结构域或者功能位点
    ''' </summary>
    <XmlType("proteinFeature", [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class ProteinFeature

        <XmlAttribute> Public Property id As String

        <XmlAttribute> Public Property protein As String

        ''' <summary>
        ''' 特征的类别：结构域、活性位点、修饰位点等等
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property featureType As String

        <XmlAttribute> Public Property left As Integer

        <XmlAttribute> Public Property right As Integer

        <XmlAttribute> Public Property residue As String

        <XmlElement> Public Property note As String

        Public Overrides Function ToString() As String
            Return $"{featureType}@{left}-{right}"
        End Function

    End Class

End Namespace
