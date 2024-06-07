﻿#Region "Microsoft.VisualBasic::28a9d835d7068010e324d9073dcfadca, engine\Compiler\AssemblyScript\Commands\Add.vb"

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

    '   Total Lines: 28
    '    Code Lines: 19 (67.86%)
    ' Comment Lines: 3 (10.71%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 6 (21.43%)
    '     File Size: 854 B


    '     Class Add
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Execute, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.GCModeller.Compiler.AssemblyScript.Script

Namespace AssemblyScript.Commands

    ''' <summary>
    ''' add item nodes to cellular network
    ''' </summary>
    Public Class Add : Inherits Modification

        Sub New(tokens As Token())
            tokens = tokens.Skip(1).ToArray

            If tokens.Any(Function(a) a.name = Script.Tokens.comma) Then
                entry = New EntryIdVector(tokens)
            Else
                entry = New CategoryEntry(tokens)
            End If
        End Sub

        Public Overrides Function Execute(env As Environment) As Object
            Throw New NotImplementedException()
        End Function

        Public Overrides Function ToString() As String
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace
