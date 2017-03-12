#Region "Microsoft.VisualBasic::c986bccdc44279ff1d6bc76b216e5791, ..\GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\Compound.vb"

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

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.HtmlParser
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder
Imports SMRUCC.genomics.ComponentModel.EquaionModel

Namespace Assembly.KEGG.DBGET.bGetObject

    <XmlRoot("KEGG.Compound", Namespace:="http://www.kegg.jp/dbget-bin/www_bget?cpd:compound_id")>
    Public Class Compound : Implements ICompoundObject

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
        Public Property KEGG_reaction As String()
        Public Property Pathway As String()
        Public Property [Module] As String()
        Public Property Remarks As String()
        Public Property Enzyme As String()

        Protected Friend _DBLinks As DBLinks
        Public Property DbLinks As String() 'Implements MetaCyc.Schema.CompoundsMapping.ICompoundObject.DBLinks
            Get
                If _DBLinks Is Nothing Then
                    Return New String() {}
                End If
                Return _DBLinks.DBLinks
            End Get
            Set(value As String())
                _DBLinks = New DBLinks(value)
            End Set
        End Property

        Sub New()
        End Sub

        Sub New(dblinks As DBLinks)
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
            Dim Url As String = String.Format("http://www.kegg.jp/Fig/compound/{0}.gif", Entry)
            Call Url.DownloadFile(save)
        End Sub

        Public Function GetPathways() As NamedValue(Of String)()
            Return __parseNamedData(Pathway)
        End Function

        Public Function GetModules() As NamedValue(Of String)()
            Return __parseNamedData([Module])
        End Function

        Public Function GetDBLinks() As DBLink()
            Return _DBLinks.DBLinkObjects.ToArray
        End Function

        Private Shared Function __parseNamedData(strData As String()) As NamedValue(Of String)()
            Dim LQuery = LinqAPI.Exec(Of NamedValue(Of String)) <=
 _
                From s As String
                In strData
                Let Id As String = Regex.Match(s, "[.+?]").Value
                Let value As String = s.Replace(Id, "").Trim
                Select New NamedValue(Of String) With {
                    .Name = Id,
                    .Value = value
                }

            Return LQuery
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
