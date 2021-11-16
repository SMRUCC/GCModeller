#Region "Microsoft.VisualBasic::13e3490ac283cf4b6116517f5a7d2b72, DataTools\Interpro\Xml\ToolsAPI.vb"

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

    '     Module ToolsAPI
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: __trim, __trimSegment, BuildFamilies, LoadDb
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace Interpro.Xml

    <Package("Interpro.DbTools", Category:=APICategories.ResearchTools, Publisher:="")>
    Public Module ToolsAPI

        Sub New()
            Call Settings.Session.Initialize()
        End Sub

        <ExportAPI("LoadDB")>
        Public Function LoadDb(<Parameter("Interpro.Xml", "If this parameter is null, then the default file path in the GCModeller repository database will be used.")>
                               Optional path As String = "") As DbArchive
            If Not path.FileExists Then
                path = GCModeller.FileSystem.InterproXml
            End If

            Dim Trim As String = __trim(path)
            Dim DbXml As DbArchive = Trim.LoadFromXml(Of DbArchive)()
            Return DbXml
        End Function

        Private Function __trim(path As String) As String
            Dim source As String = FileIO.FileSystem.ReadAllText(path)
            Dim Tokens As String() = Strings.Split(source, "<abstract>")
            Dim sbr As StringBuilder = New StringBuilder(Tokens(Scan0))

            For Each strToken As String In Tokens.Skip(1)
                Call sbr.Append("<abstract>")
                Call sbr.Append(__trimSegment(strToken))
            Next

            source = sbr.ToString
#If DEBUG Then
            Dim idx As Integer = InStr(source, "</abstract>") + Len("</abstract>")
            Call Mid(source, 1, idx).__DEBUG_ECHO
#End If
            Return source
        End Function

        Private Function __trimSegment(afterTag As String) As String
            Dim Tokens As String() = Strings.Split(afterTag, "</abstract>")
            Dim sbr As StringBuilder = New StringBuilder(Tokens(Scan0))
            Call sbr.Replace("<", "&lt;")
            Call sbr.Append("</abstract>")
            Call sbr.Append(Tokens(1))

            Return sbr.ToString
        End Function

        <ExportAPI("FamilyView.Build")>
        Public Function BuildFamilies(DbXml As DbArchive) As Family()
            Dim dict = DbXml.interpro.ToDictionary(Function(x) x.id)
            Dim LQuery = (From interpro In dict.AsParallel
                          Where String.Equals(interpro.Value.type, "family", StringComparison.OrdinalIgnoreCase)
                          Select Family.CreateObject(interpro.Value, dict)).ToArray
            Return LQuery
        End Function
    End Module
End Namespace
