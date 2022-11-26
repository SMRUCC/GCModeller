#Region "Microsoft.VisualBasic::3b06a90518118b508627a780812962d9, GCModeller\engine\IO\GCMarkupLanguage\v2\Debugger.vb"

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

    '   Total Lines: 22
    '    Code Lines: 9
    ' Comment Lines: 9
    '   Blank Lines: 4
    '     File Size: 654 B


    '     Module Debugger
    ' 
    '         Function: checkModel
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging

Namespace v2

    ''' <summary>
    ''' Model file debugger
    ''' </summary>
    Public Module Debugger

        ''' <summary>
        ''' this function is mainly address at check the errors in the virtual cell component networking.
        ''' </summary>
        ''' <param name="vcell"></param>
        ''' <param name="log"></param>
        ''' <returns></returns>
        <Extension>
        Public Function checkModel(vcell As VirtualCell, log As LogFile) As LogFile

        End Function
    End Module
End Namespace
