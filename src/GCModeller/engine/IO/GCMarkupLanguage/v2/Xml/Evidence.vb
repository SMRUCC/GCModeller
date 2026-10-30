Imports System.Xml.Serialization

Namespace v2

    ''' <summary>
    ''' 虚拟细胞模型之中的文献证据索引。
    ''' 
    ''' 模型之中的基因、蛋白质、反应以及调控关系都通过文献的编号来引用本表之中的记录，
    ''' 从而达到“数据可溯源”的目的。为了控制模型文件的体积，本表不保存文献的正文内容。
    ''' </summary>
    <XmlType("evidence", [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class Evidence

        ''' <summary>
        ''' 文献在本模型之中的唯一编号，通常为BioCyc数据库的 ``PUB-xxxx`` 编号
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property id As String

        <XmlAttribute> Public Property pubmed As String

        <XmlAttribute> Public Property doi As String

        <XmlAttribute> Public Property year As Integer

        ''' <summary>
        ''' 证据的类型：实验证据(exp)、计算证据(comp)或者推断(inf)
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property evidenceType As String

        <XmlElement> Public Property title As String

        <XmlElement> Public Property authors As String()

        <XmlElement> Public Property source As String

        Public Overrides Function ToString() As String
            Return $"[{id}] {title}"
        End Function

    End Class

    ''' <summary>
    ''' BioCyc/GO的类(分类)层次节点
    ''' </summary>
    <XmlType("classTerm", [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class ClassTerm

        <XmlAttribute> Public Property id As String

        <XmlAttribute> Public Property name As String

        ''' <summary>
        ''' 当前类节点的父类编号
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property parent As String

        <XmlAttribute> Public Property abbreviation As String

        Public Overrides Function ToString() As String
            Return $"{id} -> {parent}"
        End Function

    End Class

End Namespace
