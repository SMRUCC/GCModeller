#Region "Microsoft.VisualBasic::7faeb7ae12b5c0781fb932bd2213e282, ..\GCModeller\CLI_tools\GCModeller\CLI\Annotations.vb"

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

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv

Partial Module CLI

    <ExportAPI("--Interpro.Build", Usage:="--Interpro.Build /xml <interpro.xml>")>
    Public Function BuildFamilies(args As CommandLine.CommandLine) As Integer
        Dim DbPath As String = args("/xml")
        Dim DbXml = SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Xml.LoadDb(DbPath)
        Dim Families = SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Xml.BuildFamilies(DbXml)
        Call Families.SaveTo(DbXml.FilePath.TrimFileExt & ".Families.csv")

        Return 0
    End Function
End Module
