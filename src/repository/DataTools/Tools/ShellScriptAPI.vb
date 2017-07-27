#Region "Microsoft.VisualBasic::5294a00eb3d2d6ab4b534e43fc92329c, ..\GCModeller\analysis\annoTools\DataTools\Tools\ShellScriptAPI.vb"

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

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.Annotations.Reports

<Package("Annotation.Reports", Publisher:="xie.guigang@gmail.com")>
Public Module ShellScriptAPI

    <ExportAPI("Write.Rtf.Anno_Reports")>
    Public Function SaveReportAsRtf(rpt As GenomeAnnotations, saveRTF As String) As Boolean
        Return rpt.SaveRTF(saveRTF)
    End Function
End Module
