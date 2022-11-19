#Region "Microsoft.VisualBasic::bac76a77760c6e721829a23fcce81c06, GCModeller\models\Networks\Microbiome\MetabolicComplementation\MetabolicEndPoints.vb"

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

    '   Total Lines: 56
    '    Code Lines: 40
    ' Comment Lines: 6
    '   Blank Lines: 10
    '     File Size: 2.00 KB


    ' Class MetabolicEndPoints
    ' 
    '     Properties: secrete, taxonomy, uptakes
    ' 
    '     Function: ToString
    ' 
    ' Class MetabolicEndPointProfiles
    ' 
    '     Properties: taxonomy
    ' 
    '     Function: CreateProfiles, getCollection, getSize
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Model.Network.Microbiome

''' <summary>
''' 一个微生物物种的代谢网络端点数据模型
''' </summary>
Public Class MetabolicEndPoints

    <XmlAttribute>
    Public Property taxonomy As String

    <XmlElement> Public Property uptakes As String()
    <XmlElement> Public Property secrete As String()

    Public Overrides Function ToString() As String
        Return taxonomy
    End Function

End Class

''' <summary>
''' 宏基因组测序分析所使用到的微生物物种间的代谢网络端点模型的集合
''' </summary>
Public Class MetabolicEndPointProfiles : Inherits ListOf(Of MetabolicEndPoints)

    <XmlElement>
    Public Property taxonomy As MetabolicEndPoints()

    Protected Overrides Function getSize() As Integer
        Return If(taxonomy Is Nothing, 0, taxonomy.Length)
    End Function

    Protected Overrides Function getCollection() As IEnumerable(Of MetabolicEndPoints)
        Return taxonomy.AsEnumerable
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function CreateProfiles(taxonomy As IEnumerable(Of TaxonomyRef), reactions As ReactionRepository, Optional nonEnzymetic As reaction() = Nothing) As MetabolicEndPointProfiles
        Return New MetabolicEndPointProfiles With {
            .taxonomy = taxonomy _
                .SafeQuery _
                .Select(Function(tax)
                            Return tax.DoMetabolicEndPointsAnalysis(reactions, nonEnzymetic)
                        End Function) _
                .Where(Function(tax)
                           Return Not (tax.uptakes.IsNullOrEmpty AndAlso tax.secrete.IsNullOrEmpty)
                       End Function) _
                .ToArray
        }
    End Function
End Class
