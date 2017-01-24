#Region "Microsoft.VisualBasic::df2209217adbfd738c7596300375dfc5, ..\GCModeller\engine\GCMarkupLanguage\GCML_Documents\XmlElements\Bacterial_GENOME\Regulon.vb"

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

Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Data.csv.IO
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.ComponentModels

Namespace GCML_Documents.XmlElements.Bacterial_GENOME

    ''' <summary>
    ''' 基因组中的一个逻辑上的表达单元，一个调控因子与其相对应的基因的关系对的集合
    ''' </summary>
    ''' <remarks>
    ''' {Regulator, EffectFactor, GeneList}
    ''' </remarks>
    Public Class Regulon : Inherits T_MetaCycEntity(Of Slots.Regulon)

        ''' <summary>
        ''' 在物理上分散但是在逻辑上为一个整体的表达单元
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property GeneList As String()
        <XmlElement> Public Property Regulator As Regulon.RegulatorF

        Public Structure RegulatorF
            ''' <summary>
            ''' Regulator UniqueId
            ''' </summary>
            ''' <remarks></remarks>
            <XmlText> Dim UniqueId As String
            <XmlAttribute> Dim Effect As Double
        End Structure

        Public Overrides Function ToString() As String
            Return Regulator.UniqueId
        End Function

        Public Shared Function Create(Data As RowObject, Model As BacterialModel) As Regulon
            Dim IdList As String() = Data(2).Split(CChar(";"))
            Dim HandleGeneList As String() = (From Id As String In IdList
                                              Let GeneId As String = (From Gene In Model.BacteriaGenome.Genes Where String.Equals(Gene.Identifier, Id) Select Gene.Identifier).First
                                              Select GeneId).ToArray
            Return New Regulon With {
                .Identifier = Data.First,
                .GeneList = HandleGeneList,
                .Regulator = New RegulatorF With {
                    .UniqueId = Data.First,
                    .Effect = Val(Data(1))
                }
            }
        End Function
    End Class
End Namespace
