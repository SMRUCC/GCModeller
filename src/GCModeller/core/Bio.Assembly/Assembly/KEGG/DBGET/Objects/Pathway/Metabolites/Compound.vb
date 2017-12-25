#Region "Microsoft.VisualBasic::03e3dc0b506ad92d51522be089d7c2c1, ..\GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\Metabolites\Compound.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
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

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder
Imports SMRUCC.genomics.ComponentModel.EquaionModel

Namespace Assembly.KEGG.DBGET.bGetObject

    Public Class Compound : Implements ICompoundObject

        Public Const xmlns_kegg$ = "http://www.kegg.jp/dbget-bin/www_bget?cpd:compound_id"

        ''' <summary>
        ''' KEGG compound ID: ``cpd:C\d+``
        ''' </summary>
        ''' <returns></returns>
        Public Overridable Property Entry As String Implements ICompoundObject.Key, ICompoundObject.KEGG_cpd
        ''' <summary>
        ''' Name
        ''' </summary>
        ''' <returns></returns>
        Public Property CommonNames As String() Implements ICompoundObject.CommonNames
        Public Property Formula As String
        Public Property MolWeight As Double
        Public Property ExactMass As Double

        ''' <summary>
        ''' The <see cref="Entry">compound</see> was involved in these reactions. (http://www.kegg.jp/dbget-bin/www_bget?rn:[KEGG_Reaction_ID])
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property reactionId As String()
        <XmlArray("pathway", [Namespace]:=xmlns_kegg)>
        Public Property Pathway As NamedValue()
        <XmlArray("module", [Namespace]:=xmlns_kegg)>
        Public Property [Module] As NamedValue()
        Public Property Remarks As String()
        Public Property Enzyme As String()

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

        ''' <summary>
        ''' 下载代谢物的结构图
        ''' </summary>
        ''' <param name="save">所下载的结构图的保存文件路径</param>
        Public Sub DownloadStructureImage(save As String)
            Dim Url As String = $"http://www.kegg.jp/Fig/compound/{Entry}.gif"
            Call Url.DownloadFile(save, refer:=$"http://www.kegg.jp/dbget-bin/www_bget?cpd:{Entry}")
        End Sub

        ''' <summary>
        ''' 下载KCF格式的小分子化合物的结构数据
        ''' </summary>
        ''' <param name="save$"></param>
        Public Sub DownloadKCF(save$)
            Call DownloadKCF(Entry, App.CurrentProcessTemp).SaveTo(save, Encodings.ASCII.CodePage)
        End Sub

        Public Shared Function DownloadKCF(cpdID$, Optional saveDIR$ = "./") As String
            Dim url$ = "http://www.kegg.jp/dbget-bin/www_bget?-f+k+compound+" & cpdID
            Dim save$ = saveDIR & "/" & cpdID & ".txt"

            If url.DownloadFile(save, refer:=$"http://www.kegg.jp/dbget-bin/www_bget?cpd:{cpdID}") Then
                Return save.ReadAllText
            Else
                Return Nothing
            End If
        End Function

        Public Function GetPathways() As NamedValue(Of String)()
            Return Pathway.Select(Function(x) New NamedValue(Of String)(x.name, x.text)).ToArray
        End Function

        Public Function GetModules() As NamedValue(Of String)()
            Return [Module].Select(Function(x) New NamedValue(Of String)(x.name, x.text)).ToArray
        End Function

        Public Function GetDBLinks() As DBLink()
            Return _DBLinks.DBLinkObjects.ToArray
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0}: {1}", Entry, Me.Formula)
        End Function

        Public Property CHEBI As String() Implements ICompoundObject.CHEBI
            Get
                If _DBLinks.IsNullOrEmpty OrElse _DBLinks.CHEBI.IsNullOrEmpty Then
                    Return Nothing
                End If
                Return (From item In _DBLinks.CHEBI Select item.Entry).ToArray
            End Get
            Set(value As String())
                For Each ID As String In value
                    Call _DBLinks.AddEntry(New DBLink With {.DBName = "CHEBI", .Entry = ID})
                Next
            End Set
        End Property

        Public Property PUBCHEM As String Implements ICompoundObject.PUBCHEM
            Get
                If _DBLinks.IsNullOrEmpty OrElse _DBLinks.PUBCHEM Is Nothing Then
                    Return ""
                End If
                Return _DBLinks.PUBCHEM.Entry
            End Get
            Set(value As String)
                _DBLinks.AddEntry(New DBLink With {.DBName = "PUBCHEM", .Entry = value})
            End Set
        End Property
    End Class
End Namespace
