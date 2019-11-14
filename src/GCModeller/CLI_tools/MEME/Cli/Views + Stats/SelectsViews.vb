#Region "Microsoft.VisualBasic::ee2180062347bdee807839ec1b421461, CLI_tools\MEME\Cli\Views + Stats\SelectsViews.vb"

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

    ' Module CLI
    ' 
    '     Function: Selectes
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.FootprintTraceAPI
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans

Partial Module CLI

    <ExportAPI("/LDM.Selects",
               Usage:="/LDM.Selects /trace <footprints.xml> /meme <memeDIR> [/out <outDIR> /named]")>
    Public Function Selectes(args As CommandLine) As Integer
        Dim trace As String = args("/trace")
        Dim memeDIR As String = args("/meme")
        Dim out As String = args.GetValue("/out", trace.TrimSuffix & "-" & memeDIR.BaseName)
        Dim named As String = If(args.GetBoolean("/named"), memeDIR.BaseName & "-", "")
        Dim result As AnnotationModel() = trace.LoadXml(Of FootprintTrace).Select(memeDIR)

        For Each x As AnnotationModel In result

            If Not String.IsNullOrEmpty(named) Then
                x.Uid = named & x.Uid
            End If

            Dim path As String =
                out & "/" & x.Uid.NormalizePathString & ".xml"
            Call x.SaveAsXml(path)
        Next

        Return 0
    End Function
End Module
