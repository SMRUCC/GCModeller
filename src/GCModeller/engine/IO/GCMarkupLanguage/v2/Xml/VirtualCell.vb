#Region "Microsoft.VisualBasic::4747d2df0def19a7571807d753c1df0d, engine\IO\GCMarkupLanguage\v2\Xml\VirtualCell.vb"

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


    ' Code Statistics:

    '   Total Lines: 135
    '    Code Lines: 80 (59.26%)
    ' Comment Lines: 28 (20.74%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 27 (20.00%)
    '     File Size: 5.10 KB


    '     Class Traits
    ' 
    '         Properties: phenotype
    ' 
    '         Function: ToString
    ' 
    '     Class VirtualCell
    ' 
    '         Properties: cellular_id, genome, metabolismStructure, taxonomy, traits
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: Summary, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.CompilerServices
Imports SMRUCC.genomics.Metagenomics

Namespace v2

    ''' <summary>
    ''' The virtual cell model xml file 
    ''' </summary>
    ''' <remarks>
    ''' 虚拟细胞数据模型Xml文件
    ''' </remarks>
    <XmlRoot(NameOf(VirtualCell), [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class VirtualCell : Inherits ModelBaseType

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
        Public Property metabolismStructure As MetabolismStructure

        ''' <summary>
        ''' the intra-cellular environment id, will be used for identify the compound source between different cell source.
        ''' usually be the organism taxonomy scientific name, or taxid, something.
        ''' </summary>
        ''' <returns></returns>
        Public Property cellular_id As String = "Intracellular"

        Public Property traits As Traits

        ''' <summary>
        ''' 细胞模型之中的区室定义表，模型之中的所有区室编号都必须在本表之中存在
        ''' </summary>
        ''' <returns></returns>
        <XmlArray("compartments")>
        <XmlArrayItem("compartment")>
        Public Property compartments As Compartment()

        ''' <summary>
        ''' 细胞的生理状态参数
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("physiology")>
        Public Property physiology As Physiology

        ''' <summary>
        ''' 模型所引用的文献证据索引表
        ''' </summary>
        ''' <returns></returns>
        <XmlArray("evidence")>
        <XmlArrayItem("publication")>
        Public Property evidence As Evidence()

        ''' <summary>
        ''' BioCyc/GO的类层次定义表，用于描述化合物、蛋白质以及代谢通路的分类归属
        ''' </summary>
        ''' <returns></returns>
        <XmlArray("classTerms")>
        <XmlArrayItem("classTerm")>
        Public Property classTerms As ClassTerm()

        ''' <summary>
        ''' 模型数据的来源描述，通常为 ``BioCyc EcoCyc 29.0``
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property dataSource As String

        ''' <summary>
        ''' 当前的模型文件所遵循的XML schema的版本号
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property schemaVersion As String = "3.0"

        Public Const GCMarkupLanguage$ = "https://bioCAD.gcmodeller.org/XML/schema_revision/GCMarkup_3.0"

        Sub New()
            Call MyBase.New()
            Call xmlns.Add("GCModeller", SMRUCC.genomics.LICENSE.GCModeller)
        End Sub

        Sub New(copy As VirtualCell)
            Call Me.New

            taxonomy = New Taxonomy(copy.taxonomy)
            properties = New CompilerServices.[Property](copy.properties)
            cellular_id = copy.cellular_id
            genome = New Genome(copy.genome)
            metabolismStructure = New MetabolismStructure(copy.metabolismStructure)
            compartments = copy.compartments.SafeQuery.ToArray
            evidence = copy.evidence.SafeQuery.ToArray
            classTerms = copy.classTerms.SafeQuery.ToArray
            physiology = copy.physiology
            traits = copy.traits
            dataSource = copy.dataSource
            schemaVersion = copy.schemaVersion
        End Sub

        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder
            Dim lv As i32 = Scan0

            If taxonomy Is Nothing Then
                taxonomy = New Taxonomy With {.scientificName = "no name"}
            End If

            Call (taxonomy.scientificName Or taxonomy.species.AsDefault) _
                .DoCall(AddressOf sb.AppendLine)

            For Each level As String In taxonomy.Select(TaxonomyRanks.Genus)
                Call sb.AppendLine("  " & New String(" "c, ++lv) & level)
            Next

            Return sb.ToString
        End Function

        ''' <summary>
        ''' 进行虚拟细胞模型的摘要文本输出
        ''' </summary>
        ''' <param name="model"></param>
        ''' <returns></returns>
        Public Shared Function Summary(model As VirtualCell) As String
            Dim sb As New StringBuilder
            Dim type$

            Call sb.AppendLine(model.ToString)
            Call sb.AppendLine()
            Call sb.AppendLine("genomes:")

            For Each replicon As replicon In model.genome.replicons
                type = If(replicon.isPlasmid, "plasmid", "chromosome")
                sb.AppendLine($" [{replicon.genomeName}, {type}] {replicon.GetGeneNumbers} genes")
            Next

            Call sb.AppendLine()
            Call sb.AppendLine($"transcript regulations: {model.genome.regulations.TryCount}")
            For Each family As NamedValue(Of Integer) In model.genome.regulations _
                .SafeQuery _
                .GroupBy(Function(r) If(r.motif Is Nothing, "unknown", r.motif.family)) _
                .Select(Function(g)
                            Return New NamedValue(Of Integer)(g.Key, g.Count)
                        End Function) _
                .OrderByDescending(Function(g) g.Value)

                Call sb.AppendLine($"  {family.Name}: {family.Value}")
            Next

            Call sb.AppendLine()
            Call sb.AppendLine("metabolism structure:")
            Call sb.AppendLine($"  enzymes: {model.metabolismStructure.enzymes.TryCount}")
            Call sb.AppendLine($"  reactions:")
            Call sb.AppendLine()
            Call sb.AppendLine($"    {model.metabolismStructure.reactions.AsEnumerable.Count(Function(r) r.is_enzymatic)} is enzymatic.")
            Call sb.AppendLine($"    {model.metabolismStructure.reactions.AsEnumerable.Count(Function(r) Not r.is_enzymatic)} is non-enzymatic.")
            Call sb.AppendLine($"  compounds: {model.metabolismStructure.compounds.TryCount}")
            Call sb.AppendLine($"  pathways: {model.metabolismStructure.maps.SafeQuery.Select(Function(m) m.pathways.TryCount).Sum}")

            If Not model.compartments.IsNullOrEmpty Then
                Call sb.AppendLine()
                Call sb.AppendLine("compartments:")

                For Each compartment As Compartment In model.compartments
                    Call sb.AppendLine($"  {compartment}")
                Next
            End If

            Return sb.ToString
        End Function
    End Class
End Namespace
