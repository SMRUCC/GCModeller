#Region "Microsoft.VisualBasic::d64ea8363a941e82944cdb2a2e14a1bb, GCModeller\data\RegulonDatabase\Regprecise\WebServices\DataModels\Regulation.vb"

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

    '   Total Lines: 235
    '    Code Lines: 55
    ' Comment Lines: 169
    '   Blank Lines: 11
    '     File Size: 8.40 KB


    '     Class regulog
    ' 
    '         Properties: effector, pathway, regulationType, regulatorFamily, regulatorName
    '                     regulogId, taxonName
    ' 
    '     Class regulon
    ' 
    '         Properties: effector, genomeId, genomeName, pathway, regulationType
    '                     regulatorFamily, regulatorName, regulogId, regulonId
    ' 
    '     Class regulator
    ' 
    '         Properties: locusTag, name, regulatorFamily, regulonId, vimssId
    ' 
    '         Function: ToString
    ' 
    '     Class gene
    ' 
    '         Properties: [function], locusTag, name, regulonId, vimssId
    ' 
    '     Class site
    ' 
    '         Properties: geneLocusTag, geneVIMSSId, position, regulonId, score
    '                     sequence
    ' 
    '     Class regulonRef
    ' 
    '         Properties: foundObjName, foundObjType, genomeName, regulatorName, regulonId
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace Regprecise.WebServices.JSON

    ''' <summary>
    ''' /regulogs?collectionType={type}&amp;collectionId={id};
    ''' /regulog?regulogId={id}
    ''' </summary>
    Public Class regulog
        ''' <summary>
        ''' effector molecule or environmental signal of a regulator
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property effector As String
        ''' <summary>
        ''' type of regulation: either TF (transcription factor) or RNA
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property regulationType As String
        ''' <summary>
        ''' family of regulator
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property regulatorFamily As String
        ''' <summary>
        ''' name of regulator
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property regulatorName As String
        ''' <summary>
        ''' identifier of regulog
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property regulogId As Integer
        ''' <summary>
        ''' name of taxonomic group
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property taxonName As String
        ''' <summary>
        ''' metabolic pathway or biological process controlled by a regulator 
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property pathway As String
    End Class

    ' {"regulon":{"effector":"Deoxyribonucleotides","genomeId":"437","genomeName":"Blastopirellula marina DSM 3645","pathway":"Deoxyribonucleotide biosynthesis","regulationType":"TF","regulatorFamily":"NrdR","regulatorName":"NrdR","regulogId":"4334","regulonId":"41968"}}

    ''' <summary>
    ''' /regulons?[regulogId={regulogId},genomeId={genomeId}]
    ''' /regulon?regulonId={id}
    ''' </summary>
    Public Class regulon

        ''' <summary>
        ''' effector molecule or environmentla signal of a regulator
        ''' </summary>
        ''' <returns></returns>
        Public Property effector As String
        ''' <summary>
        ''' identifier of a genome
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property genomeId As Integer
        ''' <summary>
        ''' name of a genome
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property genomeName As String
        ''' <summary>
        ''' metabolic pathway or biological process controlled by regulator
        ''' </summary>
        ''' <returns></returns>
        Public Property pathway As String
        ''' <summary>
        ''' type of regulation: either TF (transcription factor) or RNA
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property regulationType As String
        ''' <summary>
        ''' famliy of a regulator
        ''' </summary>
        ''' <returns></returns>
        Public Property regulatorFamily As String
        ''' <summary>
        ''' name of a regulator
        ''' </summary>
        ''' <returns></returns>
        Public Property regulatorName As String
        ''' <summary>
        ''' identifier of a regulog
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property regulogId As Integer
        ''' <summary>
        ''' identifier of a regulon
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property regulonId As Integer
    End Class

    ''' <summary>
    ''' /regulators?[regulonId={regulonId},regulogId={regulogId}]
    ''' </summary>
    Public Class regulator
        ''' <summary>
        ''' locus tag of regulator gene in GeneBank
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property locusTag As String
        ''' <summary>
        ''' name of regulator
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property name As String
        ''' <summary>
        ''' family of a regulator
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property regulatorFamily As String
        ''' <summary>
        ''' identifier of regulon to which a regulator belongs to
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property regulonId As Integer
        ''' <summary>
        ''' identifier of regulator gene in MicrobesOnline database 
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property vimssId As Integer

        Public Overrides Function ToString() As String
            Return $"> {locusTag}|{name}|{regulatorFamily}|{regulonId}|{vimssId}"
        End Function
    End Class

    ''' <summary>
    ''' /genes?[regulonId={regulonId},regulogId={regulogId}]
    ''' </summary>
    Public Class gene
        ''' <summary>
        ''' gene function
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property [function] As String
        ''' <summary>
        ''' locus tag of a gene in GeneBank
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property locusTag As String
        ''' <summary>
        ''' name of gene
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property name As String
        ''' <summary>
        ''' identifier of a regulon 
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property regulonId As Integer
        ''' <summary>
        ''' identifier of gene in MicrobesOnline database 
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property vimssId As Integer
    End Class

    ''' <summary>
    ''' /sites?[regulonId={regulonId},regulogId={regulogId}]
    ''' </summary>
    Public Class site
        ''' <summary>
        ''' locus tag of a downstream gene in GeneBank
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property geneLocusTag As String
        ''' <summary>
        ''' identifier of a downstream gene in MicrobesOnline database 
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property geneVIMSSId As Integer
        ''' <summary>
        ''' position of a regulatory site relative to the start of a downstream gene 
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property position As Integer
        ''' <summary>
        '''  identifier of regulon
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property regulonId As Integer
        ''' <summary>
        ''' score of a regualtory site
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property score As Double
        ''' <summary>
        ''' sequence of a regualtory site
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property sequence As String
    End Class

    ''' <summary>
    ''' /searchRegulons?objType={type}&amp;text={text}
    ''' </summary>
    Public Class regulonRef

        ''' <summary>
        ''' found object name (or locusTag)
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property foundObjName As String
        ''' <summary>
        ''' found object type (either 'gene' or 'regulator')
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property foundObjType As String
        ''' <summary>
        ''' name of genome
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property genomeName As String
        ''' <summary>
        ''' the name of regulator
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property regulatorName As String
        ''' <summary>
        ''' identifier of regulon
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property regulonId As Integer
    End Class
End Namespace
