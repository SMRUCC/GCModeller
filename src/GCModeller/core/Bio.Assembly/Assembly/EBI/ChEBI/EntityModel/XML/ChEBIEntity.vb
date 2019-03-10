﻿#Region "Microsoft.VisualBasic::5225865499966b74e6c59de2f95d07f2, Bio.Assembly\Assembly\EBI\ChEBI\EntityModel\XML\ChEBIEntity.vb"

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

    '     Class ChEBIEntity
    ' 
    '         Properties: Address, charge, chebiAsciiName, chebiId, ChemicalStructures
    '                     Citations, CompoundOrigins, DatabaseLinks, definition, entityStar
    '                     Formula, Formulae, inchi, inchiKey, IupacNames
    '                     mass, OntologyChildren, OntologyParents, RegistryNumbers, SecondaryChEBIIds
    '                     smiles, status, Synonyms
    ' 
    '         Function: ToString
    ' 
    '         Sub: Assign
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.ComponentModel

Namespace Assembly.EBI.ChEBI.XML

    ''' <summary>
    ''' The complete entity including synonyms, database links and chemical structures.
    ''' (ChEBI数据库之中的一个对某种代谢物的完整的描述的数据模型，
    ''' <see cref="INamedValue"/>的主键为<see cref="chebiId"/>主ID)
    ''' </summary>
    ''' <remarks>
    ''' 这个对象的XML布局是根据ChEBI的Web Services来生成的，所以为了能够正确的读取ChEBI的数据，不能够再随意修改了
    ''' return节点之中的数据
    ''' </remarks>
    Public Class ChEBIEntity ' : Inherits XmlDataModel
        Implements INamedValue
        Implements IMolecule
        Implements IAddressOf

        ''' <summary>
        ''' Chebi的主ID
        ''' </summary>
        ''' <returns></returns>
        Public Property chebiId As String Implements INamedValue.Key, IMolecule.ID

        Dim id%
        Friend Property Address As Integer Implements IAddress(Of Integer).Address
            Get
                If id = 0 Then
                    id = Val(chebiId.Split(":"c).Last)
                End If
                Return id
            End Get
            Set(value As Integer)
                id = value
            End Set
        End Property

        Public Property chebiAsciiName As String Implements IMolecule.Name
        Public Property definition As String
        Public Property status As String
        Public Property smiles As String
        Public Property inchi As String
        Public Property inchiKey As String
        Public Property charge As Integer
        Public Property mass As Double Implements IMolecule.Mass
        Public Property entityStar As Integer
        <XmlElement>
        Public Property Synonyms As Synonyms()
        <XmlElement>
        Public Property IupacNames As Synonyms()
        <XmlElement>
        Public Property Citations As Synonyms()
        Public Property Formulae As Formulae
        ''' <summary>
        ''' 次级编号，和主编号都代表同一样物质
        ''' </summary>
        ''' <returns></returns>
        <XmlElement>
        Public Property SecondaryChEBIIds As String()
        <XmlElement>
        Public Property RegistryNumbers As RegistryNumbers()
        <XmlElement>
        Public Property ChemicalStructures As ChemicalStructures()
        <XmlElement>
        Public Property DatabaseLinks As DatabaseLinks()
        <XmlElement>
        Public Property OntologyParents As OntologyParents()
        <XmlElement>
        Public Property OntologyChildren As OntologyParents()
        <XmlElement>
        Public Property CompoundOrigins As CompoundOrigin()

        Private Property Formula As String Implements IMolecule.Formula
            Get
                Return Formulae.data
            End Get
            Set(value As String)
                Formulae.data = value
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return chebiAsciiName
        End Function

        Private Sub Assign(address As Integer) Implements IAddress(Of Integer).Assign
            Throw New NotImplementedException()
        End Sub
    End Class
End Namespace
