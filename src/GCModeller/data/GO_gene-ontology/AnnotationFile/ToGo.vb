#Region "Microsoft.VisualBasic::ba617f8a182976f2d5739caefa1dba2f, ..\GCModeller\data\GO_gene-ontology\AnnotationFile\ToGo.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
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
Imports Microsoft.VisualBasic.Language

''' <summary>
''' The reference links between the Go database and other biological database.
''' (Go数据库和其他的生物学数据库的相互之间的外键连接)
''' </summary>
''' <remarks></remarks>
Public Class ToGo

    Public Property DbXrefID As String
    Public Property GO_ID As String
    Public Property FunctionAnnotation As String

    Public Overrides Function ToString() As String
        Return DbXrefID & " <===> " & GO_ID
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Path"></param>
    ''' <param name="DbXrefHead">COG/EC/MetaCyc/KEGG/Pfam/Reactome/SMART</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function LoadDocument(Path As String, DbXrefHead As String) As ToGo()
        Dim OffSet As Integer = Len(DbXrefHead) + 2
        Dim Document As String() = LinqAPI.Exec(Of String) <=
 _
            From s As String
            In IO.File.ReadAllLines(Path)
            Where s.First <> "!"c
            Select Mid(s, OffSet)

        Dim LQuery = LinqAPI.Exec(Of ToGo) <=
 _
            From s As String
            In Document.AsParallel
            Let Tokens As String() = s.Split
            Let annotation As String = Regex.Match(s, "GO:.+;").Value
            Select New ToGo With {
                .DbXrefID = Tokens.First,
                .GO_ID = Tokens.Last,
                .FunctionAnnotation = annotation
            }

        Return LQuery
    End Function
End Class
