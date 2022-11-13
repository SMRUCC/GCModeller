#Region "Microsoft.VisualBasic::337242bbd630aba96a92ea84b5b61609, GCModeller\data\SABIO-RK\Dumps\DocAPI.vb"

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

    '   Total Lines: 128
    '    Code Lines: 109
    ' Comment Lines: 0
    '   Blank Lines: 19
    '     File Size: 6.94 KB


    '     Module DocAPI
    ' 
    '         Function: GetIdentifier, GetIdentifiers
    ' 
    '         Sub: (+4 Overloads) ExportDatabase
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace SBML

    <Package("Sabio-rk.DbAPI")>
    Public Module DocAPI

        <ExportAPI("GET.Identifier")>
        <Extension>
        Public Function GetIdentifier(strData As String(), Keyword As String) As String
            Dim LQuery As String = LinqAPI.DefaultFirst(Of String) <=
                From strItem As String
                In strData
                Where InStr(strItem, Keyword)
                Select strItem.Split(CChar("/")).Last

            Return LQuery
        End Function

        <ExportAPI("GET.Identifiers")>
        <Extension>
        Public Function GetIdentifiers(strData As String(), Keyword As String) As String()
            Dim LQuery As String() = LinqAPI.Exec(Of String) <=
                From strItem As String
                In strData
                Where InStr(strItem, Keyword)
                Select strItem.Split(CChar("/")).Last
            Return LQuery
        End Function
    End Module
End Namespace
