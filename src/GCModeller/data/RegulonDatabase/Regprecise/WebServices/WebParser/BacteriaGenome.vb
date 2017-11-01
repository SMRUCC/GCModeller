#Region "Microsoft.VisualBasic::44c632640b8d25dcca566eb523234cc6, ..\GCModeller\data\RegulonDatabase\Regprecise\WebServices\WebParser\BacteriaGenome.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
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

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Data.Regprecise.WebServices

Namespace Regprecise

    Public Class BacteriaGenome

        ''' <summary>
        ''' {GenomeName, Url}
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement> Public Property BacteriaGenome As JSONLDM.genome
        <XmlElement> Public Property Regulons As Regulon

        ''' <summary>
        ''' 这个基因组里面的Regulon的数目
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property NumOfRegulons As Integer
            Get
                If Regulons Is Nothing OrElse
                    Regulons.Regulators.IsNullOrEmpty Then
                    Return 0
                Else
                    Return Regulons.Regulators.Length
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return BacteriaGenome.ToString
        End Function

        ''' <summary>
        ''' Listing all TF type regulators in this genome.
        ''' </summary>
        ''' <returns></returns>
        Public Function ListRegulators() As String()
            Dim list As String() = (From x As Regulator In Regulons.Regulators
                                    Where x.Type = Regulator.Types.TF
                                    Select x.LocusTag.Key
                                    Distinct).ToArray
            Return list
        End Function

        ''' <summary>
        ''' Listing all regulated genes in this genome.
        ''' </summary>
        ''' <returns></returns>
        Public Function ListRegulatedGenes() As String()
            Dim list As List(Of String) = (From x As Regulator
                                           In Regulons.Regulators
                                           Select x.lstOperon.Select(Function(o) o.Members.Select(Function(g) g.LocusId))).ToArray.Unlist.Unlist
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
                .genome = BacteriaGenome.name,
                .locusId = lstId.Distinct.ToArray
            }
        End Function
    End Class
End Namespace
