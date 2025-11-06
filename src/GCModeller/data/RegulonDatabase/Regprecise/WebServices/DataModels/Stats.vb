#Region "Microsoft.VisualBasic::ec02c2c9f591bd6d95a2eae1be5a829d, data\RegulonDatabase\Regprecise\WebServices\DataModels\Stats.vb"

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

    '   Total Lines: 203
    '    Code Lines: 53 (26.11%)
    ' Comment Lines: 140 (68.97%)
    '    - Xml Docs: 99.29%
    ' 
    '   Blank Lines: 10 (4.93%)
    '     File Size: 7.12 KB


    '     Class genomeStat
    ' 
    '         Properties: genomeId, name, rnaRegulonCount, rnaSiteCount, taxonomyId
    '                     tfRegulonCount, tfSiteCount
    ' 
    '         Function: ToString
    ' 
    '     Class searchExtRegulons
    ' 
    '         Properties: foundObjName, foundObjType, genomeName, regulatorName, regulonId
    ' 
    '     Class regulogCollectionStat
    ' 
    '         Properties: className, collectionId, collectionType, name, rnaCount
    '                     rnaRegulogCount, rnaSiteCount, tfCount, tfRegulogCount, tfSiteCount
    '                     totalGenomeCount, totalRegulogCount
    ' 
    '     Class genome
    ' 
    '         Properties: genomeId, name, taxonomyId
    ' 
    '         Function: ToString
    ' 
    '     Class regulogCollection
    ' 
    '         Properties: className, collectionId, collectionType, name
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Xml.Serialization

Namespace Regprecise.WebServices.JSON

    ''' <summary>
    ''' https://regprecise.lbl.gov/Services/rest/genomeStats
    ''' </summary>
    <Description("https://regprecise.lbl.gov/Services/rest/genomeStats")> Public Class genomeStat
        ''' <summary>
        ''' genome identifier
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property genomeId As Integer
        ''' <summary>
        ''' genome name
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property name As String
        ''' <summary>
        ''' total number of RNA-controlled regulons reconstructed in genome
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property rnaRegulonCount As Integer
        ''' <summary>
        ''' total number of RNA regulatory sites in genome
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property rnaSiteCount As Integer
        ''' <summary>
        ''' NCBI taxonomy id
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property taxonomyId As Integer
        ''' <summary>
        ''' total number of TF-controlled regulons reconstructed in genome
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property tfRegulonCount As Integer
        ''' <summary>
        ''' total number of TF binding sites in genome
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property tfSiteCount As Integer

        Public Overrides Function ToString() As String
            Return name
        End Function
    End Class

    ''' <summary>
    ''' /searchExtRegulons?taxonomyId={taxonomyId}&amp;locusTags={locusTags}
    ''' </summary>
    Public Class searchExtRegulons

        ''' <summary>
        ''' identifier Of regulon
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property regulonId As Integer
        ''' <summary>
        ''' name Of genome
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property genomeName As String
        ''' <summary>
        ''' the name Of regulator
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property regulatorName As String
        ''' <summary>
        ''' found Object type 
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property foundObjType As String
        ''' <summary>
        ''' found Object name (Or locusTag)
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property foundObjName As String
    End Class

    ''' <summary>
    ''' /regulogCollectionStats?collectionType={type}
    ''' </summary>
    Public Class regulogCollectionStat
        ''' <summary>
        ''' name of collection class
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property className As String
        ''' <summary>
        ''' identifier of collection
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property collectionId As Integer
        ''' <summary>
        ''' type of collection
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property collectionType As String
        ''' <summary>
        ''' collection name
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property name As String
        ''' <summary>
        ''' number of RNA families in collection
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property rnaCount As Integer
        ''' <summary>
        ''' number of RNA-controlled regulogs in collection
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property rnaRegulogCount As Integer
        ''' <summary>
        ''' number of RNA regulatory sites in collection
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property rnaSiteCount As Integer
        ''' <summary>
        ''' number of different transcription factors in collection
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property tfCount As Integer
        ''' <summary>
        ''' number of TF-controlled regulogs in collection
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property tfRegulogCount As Integer
        ''' <summary>
        ''' number of TF binding sites in collection
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property tfSiteCount As Integer
        ''' <summary>
        ''' total number of genomes that have at least one regulon in collection
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property totalGenomeCount As Integer
        ''' <summary>
        ''' total number of regulogs in collection
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property totalRegulogCount As Integer
    End Class

    ''' <summary>
    ''' https://regprecise.lbl.gov/Services/rest/genomes
    ''' </summary>
    ''' 
    <Description("https://regprecise.lbl.gov/Services/rest/genomes")>
    <XmlType("bacterial.genome", [Namespace]:="https://regprecise.lbl.gov/Services/rest/genomes")>
    Public Class genome

        ''' <summary>
        ''' genome identifier
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property genomeId As Integer
        ''' <summary>
        ''' genome name
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property name As String
        ''' <summary>
        ''' NCBI taxonomy id
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property taxonomyId As Integer

        Public Overrides Function ToString() As String
            Return name
        End Function
    End Class

    ''' <summary>
    ''' /regulogCollections?collectionType={type}
    ''' </summary>
    Public Class regulogCollection
        ''' <summary>
        ''' type of regulog collection
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property collectionType As String
        ''' <summary>
        ''' identifier of collection
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property collectionId As String
        ''' <summary>
        ''' collection name
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property name As String
        ''' <summary>
        ''' name of collection class 
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property className As String
    End Class
End Namespace
