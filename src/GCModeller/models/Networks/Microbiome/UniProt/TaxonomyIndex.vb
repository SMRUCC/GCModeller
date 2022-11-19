#Region "Microsoft.VisualBasic::e57ff24a5053063d9e78dc86f1901cfd, GCModeller\models\Networks\Microbiome\UniProt\TaxonomyIndex.vb"

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

    '   Total Lines: 39
    '    Code Lines: 24
    ' Comment Lines: 9
    '   Blank Lines: 6
    '     File Size: 1.20 KB


    ' Class TaxonomyIndex
    ' 
    '     Properties: ref
    ' 
    '     Function: Summary
    ' 
    ' Class Summary
    ' 
    '     Properties: lineageGroup, Maps, ncbi_taxon_id, scientificName
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Public Class TaxonomyIndex

    <XmlElement(NameOf(ref))>
    Public Property ref As Summary()

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="uniprotRef$"><see cref="TaxonomyRepository"/></param>
    ''' <param name="maps$"><see cref="MapRepository"/></param>
    ''' <returns></returns>
    Public Shared Function Summary(uniprotRef$, maps$) As TaxonomyIndex
        Return New TaxonomyIndex With {
            .ref = TaxonomyIndexExtensions _
                .IteratesModels(uniprotRef) _
                .Summary(ref:=maps.LoadXml(Of MapRepository)) _
                .ToArray
        }
    End Function
End Class

''' <summary>
''' 用于加速<see cref="PathwayProfile"/>计算的已经预先计算好的基因组摘要数据
''' </summary>
Public Class Summary

    Public Property ncbi_taxon_id As String
    Public Property scientificName As String
    Public Property lineageGroup As NamedValue()
    Public Property Maps As String()

    Public Overrides Function ToString() As String
        Return scientificName
    End Function
End Class
