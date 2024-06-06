﻿#Region "Microsoft.VisualBasic::c3def328f2cce2344091a63fcdeaf3ae, engine\Compiler\AssemblyScript\Compiler.vb"

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

    '   Total Lines: 20
    '    Code Lines: 15 (75.00%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 5 (25.00%)
    '     File Size: 687 B


    '     Class Compiler
    ' 
    '         Function: Build, CompileImpl
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.CompilerServices

Namespace AssemblyScript

    Public Class Compiler : Inherits Compiler(Of VirtualCell)

        Dim registry As Registry
        Dim session As Environment

        Public Shared Function Build(vhd As String, registry As Registry) As VirtualCell
            Dim assemblyScript As VHDFile = VHDFile.Parse(vhd)
        End Function

        Protected Overrides Function CompileImpl(args As CommandLine) As Integer
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace
