#Region "Microsoft.VisualBasic::0fb99b857598c63d701c139c75470346, engine\IO\GCTabular\CsvTabularData\Metabolite.vb"

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

    '     Class Metabolite
    ' 
    '         Properties: ChEBI, CommonNames, Formula, Identifier, InitialAmount
    '                     KEGGCompound, MetaboliteType, MetaCycId, MolWeight, n_FluxAssociated
    '                     PUBCHEM
    ' 
    '         Function: (+3 Overloads) CreateObject, ToString, TrimSBMLMetaboliteName
    '         Class MappingComponentModel
    ' 
    '             Properties: CHEBI, CommonNames, Identifier, KEGGCompound, PUBCHEM
    ' 
    '             Function: GenerateCompoundMappingModel
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.MetaCyc.Schema
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage
Imports SMRUCC.genomics.Model.SBML

Namespace FileStream

    ''' <summary>
    ''' 代谢物对象，当前的这个类型的对象是GCModeller计算引擎内的所有的模拟基础
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Metabolite
        Implements IMetabolite
        Implements FLuxBalanceModel.IMetabolite

        <Column("Unique-Id")> Public Property Identifier As String Implements FLuxBalanceModel.IMetabolite.Key
        <Column("Mol.Weight")>
        Public Property MolWeight As Double
        <Column("KEGG.Compound")>
        Public Property KEGGCompound As String Implements IMetabolite.KEGGCompound
        Public Property ChEBI As String() Implements IMetabolite.ChEBI
        Public Property PUBCHEM As String 'Implements MetaCyc.Schema.CompoundsMapping.ICompoundObject.PUBCHEM
        Public Property Formula As String
        Public Property MetaCycId As String

        ''' <summary>
        ''' 与本代谢物相关的流对象的数目，计算规则：
        ''' 当处于不可逆反应的时候：处于产物边，计数为零，处于底物边，计数为1
        ''' 当处于可逆反应的时候：无论是处于产物边还是底物边，都被计数为0.5
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property n_FluxAssociated As Integer
        Public Property MetaboliteType As GCML_Documents.XmlElements.Metabolism.Metabolite.MetaboliteTypes
        ''' <summary>
        ''' 本代谢物对象在系统初始化的时候的初始数量
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Column("Initial.Quantity")>
        Public Property InitialAmount As Double Implements FLuxBalanceModel.IMetabolite.InitializeAmount
        <CollectionAttribute("Common-Names")> Public Property CommonNames As String() 'Implements MetaCyc.Schema.CompoundsMapping.ICompoundObject.CommonNames

        Public Overrides Function ToString() As String
            If CommonNames.IsNullOrEmpty Then
                Return Identifier
            Else
                Return CommonNames.First
            End If
        End Function

        Public Shared Function CreateObject(Metabolite As Level2.Elements.Specie, Compounds As MetaCyc.File.DataFiles.Compounds) As Metabolite
            Dim CommonNames As String() = Nothing, DBLinks As DBLinkManager = Nothing
            Dim MetaboliteId As String = Metabolite.ID.Replace("_CCO-IN", "").Replace("_CCO-OUT", "").ToUpper
            Dim Compound = Compounds.Item(MetaboliteId)
            Dim MetaboliteDataModel As FileStream.Metabolite

            If Compound Is Nothing Then
                CommonNames = New String() {TrimSBMLMetaboliteName(Metabolite.name)}
                DBLinks = MetaCyc.Schema.DBLinkManager.CreateObject(New KeyValuePair(Of String, String)() {New KeyValuePair(Of String, String)("METACYC", MetaboliteId)})
                MetaboliteDataModel = New Metabolite With {
                    .Identifier = MetaboliteId,
                    .InitialAmount = Metabolite.InitialAmount,
                    .CommonNames = CommonNames
                } ', ._DBLinks = DBLinks}
            Else
                MetaboliteDataModel = CreateObject(Compound)
            End If
            MetaboliteDataModel.MetaboliteType = GCML_Documents.XmlElements.Metabolism.Metabolite.MetaboliteTypes.Compound

            Return MetaboliteDataModel
        End Function

        Public Shared Function CreateObject(Metabolite As Level2.Elements.Specie) As Metabolite
            Dim CommonNames As String() = Nothing
            Dim MetaboliteId As String = Metabolite.ID.Replace("_CCO-IN", "").Replace("_CCO-OUT", "").ToUpper
            Dim MetaboliteDataModel As FileStream.Metabolite

            CommonNames = New String() {Metabolite.ID, TrimSBMLMetaboliteName(Metabolite.name)}
            MetaboliteDataModel = New Metabolite With {
                .Identifier = MetaboliteId,
                .InitialAmount = Metabolite.InitialAmount,
                .CommonNames = CommonNames
            } ', ._DBLinks = DBLinks}
            MetaboliteDataModel.MetaboliteType = GCML_Documents.XmlElements.Metabolism.Metabolite.MetaboliteTypes.Compound
            MetaboliteDataModel.MetaCycId = MetaboliteDataModel.Identifier

            Return MetaboliteDataModel
        End Function

        Private Shared Function TrimSBMLMetaboliteName(strName As String) As String
            If String.IsNullOrEmpty(strName) Then
                Return ""
            End If

            If InStr(strName, "a ", CompareMethod.Text) = 1 Then
                strName = Mid(strName, 2).Trim
            End If
            If strName.Last = ":"c Then
                strName = Mid(strName, 1, Len(strName) - 1)
            End If

            Return strName.Trim
        End Function

        Public Shared Function CreateObject(MetaCycCompound As MetaCyc.File.DataFiles.Slots.Compound) As Metabolite
            Dim CommonNameList As List(Of String) = New List(Of String)
            Call CommonNameList.Add(MetaCycCompound.AbbrevName)
            Call CommonNameList.Add(MetaCycCompound.CommonName)
            If Not MetaCycCompound.Names.IsNullOrEmpty Then Call CommonNameList.AddRange(MetaCycCompound.Names)
            If Not MetaCycCompound.Synonyms.IsNullOrEmpty Then Call CommonNameList.AddRange(MetaCycCompound.Synonyms)

            CommonNameList = LinqAPI.MakeList(Of String) <=
 _
                From strValue As String
                In CommonNameList
                Where Not String.IsNullOrEmpty(strValue)
                Select strValue
                Distinct

            Dim DBLinks = MetaCycCompound.GetDBLinkManager
            Call DBLinks.AddEntry(New MetaCyc.Schema.DBLinkManager.DBLink With {.DBName = "METACYC", .AccessionId = MetaCycCompound.Identifier})

            Dim KEGGCompound_Id As String() = (From item In DBLinks("LIGAND-CPD") Select item.AccessionId).ToArray
            Dim MetaboliteDataModel = New Metabolite With {.Identifier = MetaCycCompound.Identifier.ToUpper, .InitialAmount = 1000,
                                                           .KEGGCompound = If(KEGGCompound_Id.IsNullOrEmpty, "", KEGGCompound_Id.First),
                                                           .CommonNames = CommonNameList.ToArray} ', ._DBLinks = DBLinks}
            MetaboliteDataModel.MolWeight = MetaCycCompound.GetMolecularWeight
            Return MetaboliteDataModel
        End Function

        'Public ReadOnly Property CHEBI As String() 'Implements MetaCyc.Schema.CompoundsMapping.ICompoundObject.CHEBI
        '    Get
        '        If _DBLinks.CHEBI.IsNullOrEmpty Then
        '            Return New String() {}
        '        End If
        '        Return (From item In _DBLinks.CHEBI Select item.AccessionId).ToArray
        '    End Get
        'End Property

        ' ''' <summary>
        ' ''' MetaCyc.Schema.DBLinkManager
        ' ''' </summary>
        ' ''' <value></value>
        ' ''' <returns></returns>
        ' ''' <remarks></remarks>
        'Public Property DBLinks As String() Implements MetaCyc.Schema.CompoundsMapping.ICompoundObject.DBLinks
        '    Get
        '        Return _DBLinks.DBLinks
        '    End Get
        '    Set(value As String())
        '        _DBLinks = New MetaCyc.Schema.DBLinkManager(value)
        '    End Set
        'End Property

        Public Class MappingComponentModel

            Public Property Identifier As String

            Public Property CHEBI As String()
            Public Property CommonNames As String()
            Public Property KEGGCompound As String
            Public Property PUBCHEM As String

            Public Shared Function GenerateCompoundMappingModel(data As IEnumerable(Of Metabolite)) As List(Of MappingComponentModel)
                Dim LQuery = LinqAPI.MakeList(Of MappingComponentModel) <=
 _
                    From compound As FileStream.Metabolite
                    In data
                    Select New MappingComponentModel With {
                        ._PUBCHEM = compound.PUBCHEM,
                        .KEGGCompound = compound.KEGGCompound,
                        ._CHEBI = compound.ChEBI,
                        .Identifier = compound.Identifier,
                        .CommonNames = compound.CommonNames
                    }

                Return LQuery
            End Function
        End Class
    End Class

End Namespace
