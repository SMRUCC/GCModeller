#Region "Microsoft.VisualBasic::13d9b1d9788c91cb52557c1bcda625a4, GCModeller\engine\Compiler\AssemblyScript\Commands\metadata\Maintainer.vb"

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

    '   Total Lines: 31
    '    Code Lines: 20
    ' Comment Lines: 3
    '   Blank Lines: 8
    '     File Size: 903 B


    '     Class Maintainer
    ' 
    '         Properties: authorName, email
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: Execute, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.GCModeller.Compiler.AssemblyScript.Script

Namespace AssemblyScript.Commands

    ''' <summary>
    ''' maintainer or author information(meta data shortcut)
    ''' </summary>
    Public Class Maintainer : Inherits Command

        Public Property authorName As String
        Public Property email As String

        Sub New(tokens As Token())
            Dim info = tokens.Skip(1).ToArray

            authorName = info(0).text
            email = Strings.Trim(info.ElementAtOrDefault(1)?.text).Trim(""""c, "<"c, ">"c).Trim
        End Sub

        Sub New()
        End Sub

        Public Overrides Function Execute(env As Environment) As Object
            Throw New NotImplementedException()
        End Function

        Public Overrides Function ToString() As String
            Return $"MAINTAINER {authorName} <{email}>"
        End Function
    End Class
End Namespace
