#Region "Microsoft.VisualBasic::9ecc154da9351aa9ef2db27692a6cd70, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Slots\Compound.vb"

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

    '   Total Lines: 145
    '    Code Lines: 116
    ' Comment Lines: 12
    '   Blank Lines: 17
    '     File Size: 5.93 KB


    '     Class Compound
    ' 
    '         Properties: CHEBI, CommonName, ComponentOf, Components, DBLinks
    '                     Identifier, KEGGCompound, MolecularWeight, MonoisotopicMW, Names
    '                     PUBCHEM, Regulates, Table, Types
    ' 
    '         Function: GetDBLinkManager, GetMolecularWeight, Trim, TrimHTML
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Reflection
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.MetaCyc.Schema
Imports SMRUCC.genomics.Assembly.MetaCyc.Schema.Reflection
Imports SMRUCC.genomics.ComponentModel.EquaionModel

Namespace Assembly.MetaCyc.File.DataFiles.Slots

    ''' <summary>
    ''' The Class Compounds describe small-molecular-weight chemical compounds — typically,
    ''' compounds that are substrates of metabolic reactions or compounds that activate or
    ''' inhibit metabolic enzymes.
    ''' </summary>
    ''' <remarks>
    ''' One of the component in the Class ProtLigandCplxe (Protein-Small-Molecule-Complexes) with class protein
    ''' </remarks>
    Public Class Compound : Inherits Slots.Object
        Implements Regulation.IRegulator
        Implements INamedValue

        <MetaCycField> Public Overrides Property CommonName As String Implements Regulation.IRegulator.CommonName
            Get
                Return MyBase.CommonName
            End Get
            Set(value As String)
                MyBase.CommonName = value
            End Set
        End Property
        <ExternalKey("compounds", "", ExternalKey.Directions.Out)> <MetaCycField(Type:=MetaCycField.Types.TStr)>
        Public Property Components As List(Of String) Implements Regulation.IRegulator.Components
        <ExternalKey("compounds,proteins,protligandcplxes", "", ExternalKey.Directions.In)> <MetaCycField(Type:=MetaCycField.Types.TStr)>
        Public Property ComponentOf As List(Of String) Implements Regulation.IRegulator.ComponentOf

        <MetaCycField(Type:=MetaCycField.Types.TStr)>
        Public Overrides Property Types As List(Of String) Implements Regulation.IRegulator.Types
            Get
                Return MyBase.Types
            End Get
            Set(value As List(Of String))
                MyBase.Types = value
            End Set
        End Property

        <MetaCycField(Name:="UNIQUE-ID")>
        Public Overrides Property Identifier As String Implements Regulation.IRegulator.UniqueId, INamedValue.Key
            Get
                Return MyBase.Identifier
            End Get
            Set(value As String)
                MyBase.Identifier = value
            End Set
        End Property

        <MetaCycField(Name:="MOLECULAR-WEIGHT")> Public Property MolecularWeight As String
        <MetaCycField(Name:="MONOISOTOPIC-MW")> Public Property MonoisotopicMW As String

        <MetaCycField(Type:=MetaCycField.Types.TStr)> Public Shadows Property Names As String()
            Get
                If MyBase.Names.IsNullOrEmpty Then
                    MyBase.Names = (From strValue As String
                                    In New List(Of String) +
                                        Me.AbbrevName + Me.CommonName + Me.Synonyms + Me.Types.ToArray
                                    Let strItem As String = strValue.Trim.ToLower
                                    Where Not String.IsNullOrEmpty(strItem)
                                    Select strItem Distinct).AsList
                End If
                Return MyBase.Names
            End Get
            Set(value As String())
                MyBase.Names = value.AsList
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        <ExternalKey("regulations", "involved in", ExternalKey.Directions.Out)> <MetaCycField(Type:=MetaCycField.Types.TStr)> Public Property Regulates As List(Of String) Implements Regulation.IRegulator.Regulates

        Public Overrides ReadOnly Property Table As [Object].Tables
            Get
                Return Tables.compounds
            End Get
        End Property

        Public Function GetMolecularWeight() As Double
            If Not String.IsNullOrEmpty(MolecularWeight) Then
                Return Val(MolecularWeight)
            Else
                If Not String.IsNullOrEmpty(MonoisotopicMW) Then
                    Return Val(MonoisotopicMW)
                Else
                    Return 0
                End If
            End If
        End Function

        Public Function Trim() As Compound
            Me.CommonName = TrimHTML(Me.CommonName)
            Me.Synonyms = (From s As String In Me.Synonyms Select TrimHTML(s)).ToArray
            Me.AbbrevName = TrimHTML(Me.AbbrevName)
            Return Me
        End Function

        Private Shared Function TrimHTML(str As String) As String
            If String.IsNullOrEmpty(str) Then
                Return ""
            Else
                Return Regex.Replace(str, "<i>|</i>|<SUB>|</SUB>|<SUP>|</SUP>|[&]", "", RegexOptions.IgnoreCase)
            End If
        End Function

        Public ReadOnly Property CHEBI As String()

        <MetaCycField(Name:="DBLINKS", Type:=MetaCycField.Types.TStr)> Public Overrides Property DBLinks As String()
            Get
                Return _DBLinks.DBLinks
            End Get
            Set(value As String())
                _DBLinks = Schema.DBLinkManager.CreateFromMetaCycFormat(value)
                _CHEBI = (From item In _DBLinks.CHEBI Select item.AccessionId).ToArray
            End Set
        End Property

        Public ReadOnly Property PUBCHEM As String

        Public Function GetDBLinkManager() As MetaCyc.Schema.DBLinkManager
            Return MyBase._DBLinks
        End Function

        Public ReadOnly Property KEGGCompound As String
            Get
                Dim DBLinks = _DBLinks.Item("LIGAND-CPD")
                If DBLinks.IsNullOrEmpty Then
                    Return ""
                Else
                    Return DBLinks.First.AccessionId
                End If
            End Get
        End Property
    End Class
End Namespace
