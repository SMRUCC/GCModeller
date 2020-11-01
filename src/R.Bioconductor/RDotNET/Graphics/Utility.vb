#Region "Microsoft.VisualBasic::ee8f565c7d5954d177f4154628cb4c44, RDotNET\Graphics\Utility.vb"

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

    '     Module Utility
    ' 
    '         Function: ReadDouble
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

Namespace Graphics

    Module Utility

        <Extension>
        Public Function ReadDouble(pointer As IntPtr, offset%) As Double
            Dim value = New Double(0) {}
            Marshal.Copy(IntPtr.Add(pointer, offset), value, 0, value.Length)
            Return value(0)
        End Function
    End Module
End Namespace
