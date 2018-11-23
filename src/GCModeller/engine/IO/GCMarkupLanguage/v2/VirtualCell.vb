Imports System.Text
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

        ''' <summary>
        ''' 进行虚拟细胞模型的摘要文本输出
        ''' </summary>
        ''' <param name="model"></param>
        ''' <returns></returns>
        Public Shared Function Summary(model As VirtualCell) As String
            Dim sb As New StringBuilder

            Call sb.AppendLine(model.taxonomy.ToString)
            Call sb.AppendLine()
            Call sb.AppendLine("genome:")

            For Each replicon In model.genome.replicons
                Call sb.AppendLine($" [{replicon.genomeName}] {replicon.genes.Length}")
            Next

            Call sb.AppendLine()
            Call sb.AppendLine($"transcript regulations: {model.genome.regulations.Length}")

            Call sb.AppendLine("metabolism structure:")
            Call sb.AppendLine($"  enzymes: {model.MetabolismStructure.Enzymes.Length}")
            Call sb.AppendLine($"  reactions:")
            Call sb.AppendLine()
            Call sb.AppendLine($"    {model.MetabolismStructure.Reactions.Count(Function(r) r.is_enzymatic)} is enzymatic.")
            Call sb.AppendLine($"    {model.MetabolismStructure.Reactions.Count(Function(r) Not r.is_enzymatic)} is non-enzymatic.")

            Return sb.ToString
        End Function

    End Class
End Namespace