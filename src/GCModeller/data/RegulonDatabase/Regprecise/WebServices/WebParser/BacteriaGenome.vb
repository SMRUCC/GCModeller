#Region "Microsoft.VisualBasic::9c1764c9db49bc75bb78bfa289fd2d29, ..\GCModeller\data\RegulonDatabase\Regprecise\WebServices\WebParser\BacteriaGenome.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2018 GPL3 Licensed
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

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Data.Regprecise.WebServices
Imports SMRUCC.genomics.Data.Regtransbase.WebServices

Namespace Regprecise

    ''' <summary>
    ''' 微生物的调控组数据模型
    ''' </summary>
    <XmlType("bacterial_regulome", [Namespace]:="http://regprecise.lbl.gov/RegPrecise/genome.jsp?genome_id=taxonomy")>
    Public Class BacteriaRegulome

        ''' <summary>
        ''' {GenomeName, Url}
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement> Public Property genome As JSON.genome
        <XmlElement> Public Property regulons As Regulon

        ''' <summary>
        ''' 这个基因组里面的Regulon的数目
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property NumOfRegulons As Integer
            Get
                If regulons Is Nothing OrElse
                    regulons.regulators.IsNullOrEmpty Then
                    Return 0
                Else
                    Return regulons.regulators.Length
                End If
            End Get
        End Property

        <XmlNamespaceDeclarations()>
        Public xmlns As New XmlSerializerNamespaces

        Sub New()
            xmlns.Add("model", MotifFasta.xmlns)
        End Sub

        Public Overrides Function ToString() As String
            Return genome.ToString
        End Function

        ''' <summary>
        ''' Listing all TF type regulators in this genome.
        ''' </summary>
        ''' <returns></returns>
        Public Function ListRegulators() As String()
            Dim list As String() = (From x As Regulator In regulons.regulators
                                    Where x.type = Types.TF
                                    Select x.locus_tag.name
                                    Distinct).ToArray
            Return list
        End Function

        ''' <summary>
        ''' Listing all regulated genes in this genome.
        ''' </summary>
        ''' <returns></returns>
        Public Function ListRegulatedGenes() As String()
            Dim list As List(Of String) = (From x As Regulator
                                           In regulons.regulators
                                           Select x.operons.Select(Function(o) o.members.Select(Function(g) g.locusId))).ToArray.Unlist.Unlist
            Dim dlist As String() = list.Distinct.ToArray
            Return dlist
        End Function

        ''' <summary>
        ''' 创建用于从KEGG数据库之中下载蛋白质序列的查询数据集合
        ''' </summary>
        ''' <returns></returns>
        Public Function CreateKEGGQuery() As KEGG.WebServices.QuerySource
            Dim lstId As String() = ListRegulatedGenes.Join(ListRegulators).ToArray

            Return New KEGG.WebServices.QuerySource With {
                .genome = genome.name,
                .locusId = lstId.Distinct.ToArray
            }
        End Function
    End Class
End Namespace
