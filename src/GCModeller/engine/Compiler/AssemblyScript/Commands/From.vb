#Region "Microsoft.VisualBasic::e61c96e724249eb95bfbe7ae0f2b16e9, GCModeller\engine\Compiler\AssemblyScript\Commands\From.vb"

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

    '   Total Lines: 33
    '    Code Lines: 22
    ' Comment Lines: 3
    '   Blank Lines: 8
    '     File Size: 863 B


    '     Class From
    ' 
    '         Properties: base, tag
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
    ''' from a base model
    ''' </summary>
    Public Class From : Inherits Command

        Public Property base As String
        Public Property tag As String

        Sub New(directiveTokens As Token())
            Dim label = directiveTokens.Skip(1).ToArray

            base = label(0).text

            If label.Length = 1 Then
                tag = "latest"
            Else
                tag = label(2).text
            End If
        End Sub

        Public Overrides Function Execute(env As Environment) As Object
            Throw New NotImplementedException()
        End Function

        Public Overrides Function ToString() As String
            Return $"FROM {base}:{tag};"
        End Function
    End Class
End Namespace
