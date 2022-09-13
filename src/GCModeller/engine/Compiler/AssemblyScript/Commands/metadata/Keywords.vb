#Region "Microsoft.VisualBasic::ecbe1f4d0d0dec962e9b9c0de39ab68b, GCModeller\engine\Compiler\AssemblyScript\Commands\metadata\Keywords.vb"

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

    '   Total Lines: 26
    '    Code Lines: 20
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 841 B


    '     Class Keywords
    ' 
    '         Properties: keywords
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Execute, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.GCModeller.Compiler.AssemblyScript.Script

Namespace AssemblyScript.Commands

    Public Class Keywords : Inherits Command

        Public Property keywords As String()

        Sub New(tokens As Token())
            keywords = tokens.Skip(1) _
                .Where(Function(a)
                           Return Not a.name = Script.Tokens.comma
                       End Function) _
                .Select(Function(a) stripValueString(a.text)) _
                .ToArray
        End Sub

        Public Overrides Function Execute(env As Environment) As Object
            Throw New NotImplementedException()
        End Function

        Public Overrides Function ToString() As String
            Return $"KEYWORDS {keywords.JoinBy(", ")}"
        End Function
    End Class
End Namespace
