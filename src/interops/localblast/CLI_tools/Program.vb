#Region "Microsoft.VisualBasic::7770c5d15116de8e19d567c9ec1161d8, localblast\CLI_tools\Program.vb"

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

    '   Total Lines: 10
    '    Code Lines: 5 (50.00%)
    ' Comment Lines: 4 (40.00%)
    '    - Xml Docs: 75.00%
    ' 
    '   Blank Lines: 1 (10.00%)
    '     File Size: 216 B


    ' Module Program
    ' 
    '     Function: Main
    ' 
    ' /********************************************************************************/

#End Region

Module Program

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    Public Function Main() As Integer
        Return GetType(CLI).RunCLI(App.CommandLine)
    End Function
End Module
