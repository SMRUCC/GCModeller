#Region "Microsoft.VisualBasic::170863cdded03549ee9387d99a298c3b, GCModeller\engine\Compiler\AssemblyScript\Commands\Command.vb"

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
    '    Code Lines: 14
    ' Comment Lines: 3
    '   Blank Lines: 5
    '     File Size: 634 B


    '     Class Command
    ' 
    '         Properties: commandName
    ' 
    '         Function: stripValueString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace AssemblyScript.Commands

    ''' <summary>
    ''' the assembly compiler commands
    ''' </summary>
    Public MustInherit Class Command

        Public ReadOnly Property commandName As String
            Get
                Return MyClass.GetType.Name
            End Get
        End Property

        Public MustOverride Function Execute(env As Environment) As Object
        Public MustOverride Overrides Function ToString() As String

        Friend Shared Function stripValueString(text As String) As String
            Return Strings.Trim(text).Trim(""""c).Trim(" "c)
        End Function

    End Class
End Namespace
