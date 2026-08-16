#Region "Microsoft.VisualBasic::61d58e0d764a37ef95e9e3c0841c68c1, src\SessionManager\Extensions.vb"

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

    '   Total Lines: 21
    '    Code Lines: 16 (76.19%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 5 (23.81%)
    '     File Size: 771 B


    ' Module Extensions
    ' 
    '     Function: (+2 Overloads) Open
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Flute.Http.Configurations

<HideModuleName>
Public Module Extensions

    Public Function Open(session_store As String, ssid As String) As SessionFile
        Dim dir As String = $"{session_store}/{ssid.Substring(ssid.Length - 2, 2)}/{ssid.Substring(ssid.Length - 4, 3)}"
        Dim keyfile As String = $"{dir}/{ssid}.keys"
        Dim datafile As String = $"{dir}/{ssid}"
        Dim file As New SessionFile(keyfile, datafile)

        Return file
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Open(ssid As String, config As Configuration) As SessionFile
        Return SessionManager.Open(config.session.session_store, ssid)
    End Function

End Module
