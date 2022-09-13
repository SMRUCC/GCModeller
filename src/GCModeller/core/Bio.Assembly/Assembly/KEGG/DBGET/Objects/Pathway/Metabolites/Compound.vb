#Region "Microsoft.VisualBasic::4d44bab80239717dbfa3223d60c0499f, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\Metabolites\Compound.vb"

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

    '   Total Lines: 120
    '    Code Lines: 80
    ' Comment Lines: 21
    '   Blank Lines: 19
    '     File Size: 4.28 KB


    '     Class Compound
    ' 
    '         Properties: [Module], category, commonNames, DbLinks, entry
    '                     enzyme, exactMass, formula, Image, KCF
    '                     molWeight, pathway, reactionId, remarks
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: GetDBLinkManager, GetDBLinks, GetLinkDbRDF, GetModules, GetPathways
    '                   ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

Namespace Assembly.KEGG.DBGET.bGetObject

    ''' <summary>
    ''' KEGG的代谢物模型
    ''' </summary>
    Public Class Compound : Inherits XmlDataModel
        Implements INamedValue

        Public Const xmlns_kegg$ = "http://www.kegg.jp/dbget-bin/www_bget?cpd:compound_id"

        ''' <summary>
        ''' KEGG compound ID: ``cpd:C\d+``
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute>
        Public Overridable Property entry As String Implements INamedValue.Key

        ''' <summary>
        ''' Name
        ''' </summary>
        ''' <returns></returns>
        Public Property commonNames As String()
        Public Property formula As String
        Public Property molWeight As Double
        Public Property exactMass As Double

        ''' <summary>
        ''' The <see cref="Entry">compound</see> was involved in these reactions. (http://www.kegg.jp/dbget-bin/www_bget?rn:[KEGG_Reaction_ID])
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property reactionId As String()
        <XmlArray("pathway", [Namespace]:=xmlns_kegg)>
        Public Property pathway As NamedValue()
        <XmlArray("module", [Namespace]:=xmlns_kegg)>
        Public Property [Module] As NamedValue()
        Public Property remarks As String()
        Public Property enzyme As String()

        Public Property category As BriteTerm()

        Protected Friend _DBLinks As DBLinks

        <XmlArray("DBlinks", [Namespace]:=xmlns_kegg)>
        Public Property DbLinks As DBLink()
            Get
                If _DBLinks Is Nothing Then
                    Return {}
                Else
                    Return _DBLinks.DBLinkObjects
                End If
            End Get
            Set
                _DBLinks = New DBLinks(Value)
            End Set
        End Property

        ''' <summary>
        ''' 2D分子结构数据
        ''' </summary>
        ''' <returns></returns>
        Public Property KCF As String
        Public Property Image As String

        <XmlNamespaceDeclarations()>
        Public xmlns As New XmlSerializerNamespaces

        Sub New()
            xmlns.Add("KEGG", xmlns_kegg)
        End Sub

        Sub New(dblinks As DBLinks)
            Call Me.New
            _DBLinks = dblinks
        End Sub

        Public Function GetDBLinkManager() As DBLinks
            Return _DBLinks
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetPathways() As NamedValue(Of String)()
            Return pathway.Select(Function(x) New NamedValue(Of String)(x.name, x.text)).ToArray
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetModules() As NamedValue(Of String)()
            Return [Module].Select(Function(x) New NamedValue(Of String)(x.name, x.text)).ToArray
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetDBLinks() As DBLink()
            Return _DBLinks.DBLinkObjects.ToArray
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0}: {1}", entry, Me.formula)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function GetLinkDbRDF(compound As Compound) As IEnumerable(Of LinkDB.Relationship)
            If InStr(compound.entry, ":") > 0 Then
                Return LinkDB.Relationship.GetLinkDb(compound.entry)
            Else
                Return LinkDB.Relationship.GetLinkDb($"cpd:{compound.entry}")
            End If
        End Function
    End Class
End Namespace
