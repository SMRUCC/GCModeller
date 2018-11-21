Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports SMRUCC.genomics.Metagenomics

Namespace v2

    ''' <summary>
    ''' 虚拟细胞数据模型
    ''' </summary>
    <XmlRoot(NameOf(VirtualCell), [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class VirtualCell : Inherits XmlDataModel

        ''' <summary>
        ''' 物种注释信息
        ''' </summary>
        ''' <returns></returns>
        Public Property taxonomy As Taxonomy
        ''' <summary>
        ''' 基因组结构模型，包含有基因的列表，以及转录调控网络
        ''' </summary>
        ''' <returns></returns>
        Public Property genome As Genome

        ''' <summary>
        ''' 代谢组网络结构
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("metabolome", [Namespace]:=GCMarkupLanguage)>
        Public Property MetabolismStructure As MetabolismStructure

        Public Const GCMarkupLanguage$ = "http://CAD_software.gcmodeller.org/XML/schema_revision/GCMarkup_1.0"

        <XmlNamespaceDeclarations()>
        Public xmlns As New XmlSerializerNamespaces

        Sub New()
            Call xmlns.Add("GCModeller", SMRUCC.genomics.LICENSE.GCModeller)
        End Sub

        Public Overrides Function ToString() As String
            Return taxonomy.ToString
        End Function

    End Class
End Namespace