#Region "Microsoft.VisualBasic::7168de394bfdb93999be662a365972de, GCModeller\data\SABIO-RK\Dumps\TabularDump.vb"

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

    '   Total Lines: 83
    '    Code Lines: 37
    ' Comment Lines: 29
    '   Blank Lines: 17
    '     File Size: 3.68 KB


    '     Class SabiorkEntity
    ' 
    '         Properties: SabiorkId
    ' 
    '         Function: ToString
    ' 
    '     Class CompoundSpecie
    ' 
    '         Properties: CommonNames, DBLinks, KEGG_Compound
    ' 
    '         Function: CreateDBLinksData, CreateObjects, GetDBLinkManager, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports SMRUCC.genomics.Assembly.MetaCyc.Schema
Imports SMRUCC.genomics.Data.SABIORK.SBML

Namespace TabularDump

    Public MustInherit Class SabiorkEntity : Implements INamedValue

        Public Property SabiorkId As String Implements INamedValue.Key

        Public Overrides Function ToString() As String
            Return SabiorkId
        End Function
    End Class

    Public Class CompoundSpecie : Inherits SabiorkEntity

        <Column("kegg.compound")> Public Property KEGG_Compound As String

        Public Overrides Function ToString() As String
            Return CommonNames.First
        End Function

        Public Shared Function CreateObjects(SABIORK_DATA As SabiorkSBML) As CompoundSpecie()
            'Dim LQuery = (From cs As SBMLParser.CompoundSpecie In SABIORK_DATA.CompoundSpecies
            '              Where String.IsNullOrEmpty(GetIdentifier(cs.Identifiers, "uniprot"))
            '              Let ref As CompoundSpecie = New CompoundSpecie With {
            '                  .SabiorkId = cs.Id,
            '                  .DBLinks = CreateDBLinksData(cs),
            '                  .CommonNames = New String() {cs.Name},
            '                  .KEGG_Compound = GetIdentifier(cs.Identifiers, "kegg.compound")
            '              }
            '              Select ref).ToArray
            'Return LQuery
        End Function

        Private Shared Function CreateDBLinksData(cs As SBMLParser.CompoundSpecie) As String()
            'Dim SabiorkId As String = New DBLinkManager.DBLink() With {.DBName = "Sabio-rk", .AccessionId = cs.Id}.GetFormatValue
            'Dim KEGG_Compound = GetIdentifier(cs.Identifiers, "kegg.compound")
            'Dim CheBI As String() = (From strValue As String
            '                         In GetIdentifiers(cs.Identifiers, "chebi")
            '                         Select New DBLinkManager.DBLink() With {
            '                             .DBName = "CheBI",
            '                             .AccessionId = strValue.Replace("CHEBI:", "")}.GetFormatValue).ToArray

            'KEGG_Compound = If(String.IsNullOrEmpty(KEGG_Compound), "", New DBLinkManager.DBLink() With {.DBName = "KEGG.Compound", .AccessionId = KEGG_Compound}.GetFormatValue)

            'Dim List As List(Of String) = New List(Of String)
            'Call List.Add(SabiorkId)
            'Call List.Add(KEGG_Compound)
            'Call List.AddRange(CheBI)

            'Return (From strValue As String In List Where Not String.IsNullOrEmpty(strValue) Select strValue Distinct).ToArray
        End Function

        Public Function GetDBLinkManager() As DBLinkManager
            Return _DBLinks
        End Function

#Region "Implements SMRUCC.genomics.Assembly.MetaCyc.Schema.CompoundsMapping.ICompoundObject"

        Public Property CommonNames As String()

        Protected Friend _DBLinks As DBLinkManager

        ''' <summary>
        ''' MetaCyc.Schema.DBLinkManager.DBLink
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property DBLinks As String() 'Implements Assembly.MetaCyc.Schema.CompoundsMapping.ICompoundObject.DBLinks
            Get
                Return _DBLinks.DBLinks
            End Get
            Set(value As String())
                _DBLinks = New DBLinkManager(value)
            End Set
        End Property
#End Region
    End Class
End Namespace
