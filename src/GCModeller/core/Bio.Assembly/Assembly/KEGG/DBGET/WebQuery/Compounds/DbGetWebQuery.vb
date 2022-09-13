#Region "Microsoft.VisualBasic::3a3e1235896c93fef2bd3e565b106f4f, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\WebQuery\Compounds\DbGetWebQuery.vb"

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

    '   Total Lines: 57
    '    Code Lines: 48
    ' Comment Lines: 0
    '   Blank Lines: 9
    '     File Size: 2.07 KB


    '     Class DbGetWebQuery
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: dbgetApi, doParse, QueryCompound, QueryGlycan
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Net.Http
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers

Namespace Assembly.KEGG.DBGET.WebQuery.Compounds

    Public Class DbGetWebQuery : Inherits WebQuery(Of String)

        Public Sub New(<CallerMemberName> Optional cache As String = Nothing)
            MyBase.New(
                AddressOf dbgetApi,
                AddressOf Scripting.ToString,
                AddressOf doParse,
 _
                cache:=cache
            )
        End Sub

        Public Shared Function doParse(data$, schema As Type) As Object
            Dim form As New WebForm(data)

            If InStr(data, "No such data was found.") > 0 Then
                Return Nothing
            End If

            Select Case schema
                Case GetType(Compound)
                    Return WebParser.ParseCompound(form)
                Case GetType(Glycan)
                    Return GlycanParser.ParseGlycan(form)
                Case Else
                    Throw New NotImplementedException(schema.FullName)
            End Select
        End Function

        Public Shared Function dbgetApi(id As String) As String
            If id.StartsWith("C") Then
                Return $"http://www.kegg.jp/dbget-bin/www_bget?cpd:{id}"
            ElseIf id.StartsWith("G") Then
                Return $"http://www.kegg.jp/dbget-bin/www_bget?gl:{id}"
            ElseIf id.StartsWith("D") Then
                Return $"http://www.kegg.jp/dbget-bin/www_bget?dr:{id}"
            Else
                Throw New NotImplementedException
            End If
        End Function

        Public Function QueryGlycan(id As String) As Glycan
            Return Me.Query(Of Glycan)(id, ".html")
        End Function

        Public Function QueryCompound(id As String) As Compound
            Return Me.Query(Of Compound)(id, ".html")
        End Function
    End Class
End Namespace
