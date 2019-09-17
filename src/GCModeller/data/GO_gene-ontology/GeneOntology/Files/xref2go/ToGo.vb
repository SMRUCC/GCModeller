#Region "Microsoft.VisualBasic::d492fb01af0fbbaa4d87358614a4fab6, GO_gene-ontology\GeneOntology\Files\xref2go\ToGo.vb"

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

    '     Class ToGo
    ' 
    '         Properties: DbXrefID, FunctionAnnotation, GO_ID, Parser, xrefId
    ' 
    '         Function: LoadDocument, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language

Namespace xref2go

    ''' <summary>
    ''' The reference links between the Go database and other biological database.
    ''' (Go数据库和其他的生物学数据库的相互之间的外键连接)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ToGo(Of uid As XrefId)

        Public Property DbXrefID As String
        Public Property GO_ID As String
        Public Property FunctionAnnotation As String

        Dim __uid As uid

        Public ReadOnly Property Parser As XrefIdTypes

        Public ReadOnly Property xrefId As uid
            Get
                If __uid Is Nothing Then
                    __uid = DbXrefID.Parse(Of uid)(Parser)
                End If
                Return __uid
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return DbXrefID & " == " & GO_ID
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <param name="DbXrefHead">COG/EC/MetaCyc/KEGG/Pfam/Reactome/SMART</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function LoadDocument(Path As String, DbXrefHead As XrefIdTypes) As ToGo(Of uid)()
            Dim reads = LinqAPI.Exec(Of NamedValue(Of String)) <=
 _
                From s As String
                In IO.File.ReadAllLines(Path)
                Where s.First <> "!"c
                Select s.GetTagValue(" > ", True)

            Dim LQuery = LinqAPI.Exec(Of ToGo(Of uid)) <=
 _
                From s As NamedValue(Of String)
                In reads.AsParallel
                Let Tokens As String() = Strings.Split(s.Value, " ; ")
                Let annotation As String = Tokens(0)
                Let id As String = Tokens(1)
                Select New ToGo(Of uid) With {
                    .DbXrefID = s.Name,
                    .GO_ID = id,
                    .FunctionAnnotation = annotation,
                    ._Parser = DbXrefHead
                }

            Return LQuery
        End Function
    End Class
End Namespace
