#Region "Microsoft.VisualBasic::2682e2cec6102bde132dcf3c75502a26, GCModeller\core\Bio.Assembly\Assembly\ELIXIR\EBI\ChEBI\EntityModel\XML\ChEBIEntity.vb"

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

    '   Total Lines: 84
    '    Code Lines: 60
    ' Comment Lines: 17
    '   Blank Lines: 7
    '     File Size: 3.16 KB


    '     Class ChEBIEntity
    ' 
    '         Properties: charge, chebiAsciiName, chebiId, ChemicalStructures, Citations
    '                     CompoundOrigins, DatabaseLinks, definition, entityStar, Formula
    '                     Formulae, inchi, inchiKey, IupacNames, mass
    '                     OntologyChildren, OntologyParents, RegistryNumbers, SecondaryChEBIIds, smiles
    '                     status, Synonyms
    ' 
    '         Function: FindDatabaseLinkValue, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel

Namespace Assembly.ELIXIR.EBI.ChEBI.XML

    ''' <summary>
    ''' The complete entity including synonyms, database links and chemical structures.
    ''' (ChEBI数据库之中的一个对某种代谢物的完整的描述的数据模型，
    ''' <see cref="INamedValue"/>的主键为<see cref="chebiId"/>主ID)
    ''' </summary>
    ''' <remarks>
    ''' 这个对象的XML布局是根据ChEBI的Web Services来生成的，所以为了能够正确的读取ChEBI的数据，不能够再随意修改了
    ''' return节点之中的数据
    ''' </remarks>
    Public Class ChEBIEntity
        Implements INamedValue
        Implements IMolecule

        ''' <summary>
        ''' Chebi的主ID
        ''' </summary>
        ''' <returns></returns>
        Public Property chebiId As String Implements INamedValue.Key, IMolecule.EntryId
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

        Public Function FindDatabaseLinkValue(type As AccessionTypes) As String
            Dim typeStr$ = type.Description
            Dim link As DatabaseLinks = DatabaseLinks _
                .SafeQuery _
                .Where(Function(l) l.type = typeStr) _
                .FirstOrDefault

            Return link?.data
        End Function
    End Class
End Namespace
