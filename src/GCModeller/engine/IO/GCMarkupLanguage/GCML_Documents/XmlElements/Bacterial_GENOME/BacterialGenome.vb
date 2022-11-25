#Region "Microsoft.VisualBasic::94ab714acd987e7708d0dd02793aa6c4, GCModeller\engine\IO\GCMarkupLanguage\GCML_Documents\XmlElements\Bacterial_GENOME\BacterialGenome.vb"

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

    '   Total Lines: 37
    '    Code Lines: 19
    ' Comment Lines: 12
    '   Blank Lines: 6
    '     File Size: 2.00 KB


    '     Class BacterialGenome
    ' 
    '         Properties: Comment, CommonName, DbLinks, Genes, Genome
    '                     NCBITaxonomy, OperonCounts, Regulons, Strain, Transcripts
    '                     TransUnits
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.ComponentModels

Namespace GCML_Documents.XmlElements.Bacterial_GENOME

    <XmlType("BacterialGenome", Namespace:="http://code.google.com/p/genome-in-code/GCMarkupLanguage/GCML_Documents.XmlElements.BacteriaGenome/")>
    Public Class BacterialGenome : Inherits T_MetaCycEntity(Of Slots.Specie)

        Public Property DbLinks As String()
        Public Property CommonName As String
        Public Property Comment As String

        Public Property Genome As String
        <XmlAttribute> Public Property NCBITaxonomy As String
        Public Property Strain As String
        <XmlAttribute> Public Property OperonCounts As Integer

        ''' <summary>
        ''' The genes collection in this cell model, a gene object in this model just a carrier 
        ''' of template information.
        ''' (这个模型之中的所有的基因的集合，在本计算机模型之中，基因对象仅仅只是遗传数据的承载者，
        ''' 而真正用于计算转录产物浓度的为转录单元对象)
        ''' </summary>
        ''' <remarks></remarks>
        Public Property Genes As GeneObject()
        Public Property TransUnits As List(Of GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit)
        Public Property Regulons As GCML_Documents.XmlElements.Bacterial_GENOME.Regulon()
        Public Property Transcripts As GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.Transcript()
        ' Public Property ExpressionKinetics As EnzymeCatalystKineticLaw()

        'Public Function GetExpressionKineticsLaw(EnzymeId As String) As EnzymeCatalystKineticLaw
        '    Dim LQuery = (From item In ExpressionKinetics Where String.Equals(EnzymeId, item.Enzyme) Select item).First
        '    Return LQuery
        'End Function
    End Class
End Namespace
