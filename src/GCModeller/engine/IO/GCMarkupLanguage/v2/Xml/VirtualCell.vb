#Region "Microsoft.VisualBasic::a038330148ba04fbd22ba121587f1723, engine\IO\GCMarkupLanguage\v2\Xml\VirtualCell.vb"

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

    '     Class VirtualCell
    ' 
    '         Properties: genome, metabolismStructure, taxonomy
    ' 
    '         Constructor: (+1 Overloads) Sub New
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
    ''' 虚拟细胞数据模型Xml文件
    ''' </summary>
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

        Public Const GCMarkupLanguage$ = "https://bioCAD.gcmodeller.org/XML/schema_revision/GCMarkup_2.0"

        Sub New()
            Call MyBase.New

            Call xmlns.Add("GCModeller", SMRUCC.genomics.LICENSE.GCModeller)
        End Sub

        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder
            Dim lv As i32 = Scan0

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
            Call sb.AppendLine($"transcript regulations: {model.genome.regulations.Length}")
            For Each family As NamedValue(Of Integer) In model.genome.regulations _
                .GroupBy(Function(r) r.motif.family) _
                .Select(Function(g)
                            Return New NamedValue(Of Integer)(g.Key, g.Count)
                        End Function) _
                .OrderByDescending(Function(g) g.Value)

                Call sb.AppendLine($"  {family.Name}: {family.Value}")
            Next

            Call sb.AppendLine()
            Call sb.AppendLine("metabolism structure:")
            Call sb.AppendLine($"  enzymes: {model.metabolismStructure.enzymes.Length}")
            Call sb.AppendLine($"  reactions:")
            Call sb.AppendLine()
            Call sb.AppendLine($"    {model.metabolismStructure.reactions.AsEnumerable.Count(Function(r) r.is_enzymatic)} is enzymatic.")
            Call sb.AppendLine($"    {model.metabolismStructure.reactions.AsEnumerable.Count(Function(r) Not r.is_enzymatic)} is non-enzymatic.")

            Return sb.ToString
        End Function

    End Class
End Namespace
