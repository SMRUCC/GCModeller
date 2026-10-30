Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.ComponentModel.Annotation

Namespace v2

    ''' <summary>
    ''' the cellular phenotype traits
    ''' </summary>
    Public Class Traits

        ''' <summary>
        ''' the phenoetype terms that annotated for this cell model
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("phenotype")> Public Property phenotype As PhenotypeTrait()

        ''' <summary>
        ''' 细胞在给定培养基条件之下的比生长速率
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property growthRate As Double

        ''' <summary>
        ''' 细胞生长所使用的培养基描述
        ''' </summary>
        ''' <returns></returns>
        <XmlElement> Public Property medium As String

        ''' <summary>
        ''' 氧气利用方式：aerobic / anaerobic / facultative
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property oxygenRequirement As String

        ''' <summary>
        ''' 表型数据的来源描述或者文献编号
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("reference")> Public Property references As String()

        Public Overrides Function ToString() As String
            Return phenotype.GetJson
        End Function

    End Class
End Namespace