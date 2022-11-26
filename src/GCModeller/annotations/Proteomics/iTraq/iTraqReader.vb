#Region "Microsoft.VisualBasic::aa7c131d6c8113c1f519fbf15ff05275, GCModeller\annotations\Proteomics\iTraq\iTraqReader.vb"

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

    '   Total Lines: 79
    '    Code Lines: 56
    ' Comment Lines: 10
    '   Blank Lines: 13
    '     File Size: 3.26 KB


    ' Class iTraqReader
    ' 
    '     Properties: AAs, calcPI, Coverage, Description, ID
    '                 MW, Peptides, Proteins, PSMs, Score
    '                 UniquePeptides
    ' 
    '     Function: GetSampleGroups, ToString
    '     Structure SampleValue
    ' 
    '         Properties: Group
    ' 
    '         Function: PopulateData, ToString
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language

''' <summary>
''' iTraq data reader.(iTraq蛋白组下机数据转录原始结果文件的数据模型)
''' </summary>
Public Class iTraqReader : Inherits DataSet
    Implements INamedValue

    <Column("Accession")>
    Public Overrides Property ID As String Implements IKeyedEntity(Of String).Key
    Public Property Description As String
    Public Property Score As String
    Public Property Coverage As String

    <Column("# Proteins")> Public Property Proteins As String
    <Column("# Unique Peptides")> Public Property UniquePeptides As String
    <Column("# Peptides")> Public Property Peptides As String
    <Column("# PSMs")> Public Property PSMs As String
    <Column("# AAs")> Public Property AAs As String
    <Column("MW [kDa]")> Public Property MW As String
    <Column("calc. pI")> Public Property calcPI As String

    Public Overrides Function ToString() As String
        Return $"{ID} {Description}"
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="symbols">已经经过了<see cref="Combinations"/>操作之后的结果</param>
    ''' <returns></returns>
    Public Function GetSampleGroups(symbols As IEnumerable(Of iTraqSymbols)) As Dictionary(Of SampleValue)
        Dim groups As New List(Of SampleValue)
        Dim count$
        Dim var$

        For Each group As iTraqSymbols In symbols
            If Properties.ContainsKey(group.Symbol) Then
                count = group.Symbol & " Count"
                var = group.Symbol & " Variability [%]"

                ' FoldChange
                ' 因为这里主要是为了获取FoldChange数据，所以对于其他的可能不存在的数据就直接忽略掉了
                groups += New SampleValue With {
                    .Group = group.AnalysisID,
                    .Count = Properties.TryGetValue(count),
                    .FoldChange = Properties(group.Symbol),
                    .Variability = Properties.TryGetValue(var)
                }
            End If
        Next

        Return groups.ToDictionary
    End Function

    Public Structure SampleValue : Implements INamedValue

        Public Property Group As String Implements IKeyedEntity(Of String).Key

        Dim FoldChange#
        Dim Count%
        Dim Variability#

        Public Iterator Function PopulateData() As IEnumerable(Of KeyValuePair(Of String, Double))
            Yield New KeyValuePair(Of String, Double)(Group, FoldChange)
            Yield New KeyValuePair(Of String, Double)(Group & " Count", Count)
            Yield New KeyValuePair(Of String, Double)(Group & " Variability [%]", Variability)
        End Function

        Public Overrides Function ToString() As String
            Return $"{Group}: foldChange={FoldChange}, count={Count}, variability={Variability}"
        End Function
    End Structure
End Class
