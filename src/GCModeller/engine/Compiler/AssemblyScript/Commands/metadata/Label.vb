#Region "Microsoft.VisualBasic::ba77c26be87a52140de0a203d5102d8e, GCModeller\engine\Compiler\AssemblyScript\Commands\metadata\Label.vb"

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

    '   Total Lines: 36
    '    Code Lines: 25
    ' Comment Lines: 3
    '   Blank Lines: 8
    '     File Size: 1.32 KB


    '     Class Label
    ' 
    '         Properties: keyValues
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
    ''' add meta data for the given model
    ''' </summary>
    Public Class Label : Inherits Command

        Public Property keyValues As KeyValuePair(Of String, String)()

        Sub New(tokens As Token())
            Dim keyValues = tokens.Skip(1) _
                .Split(Function(a) a.name = Script.Tokens.comma, DelimiterLocation.NotIncludes) _
                .Where(Function(a) Not a.Length = 0) _
                .ToArray

            Me.keyValues = keyValues _
                .Select(Function(a)
                            Dim key As String = stripValueString(a(0).text)
                            Dim val As String = stripValueString(a(2).text)

                            Return New KeyValuePair(Of String, String)(key, val)
                        End Function) _
                .ToArray
        End Sub

        Public Overrides Function Execute(env As Environment) As Object
            Throw New NotImplementedException()
        End Function

        Public Overrides Function ToString() As String
            Return $"LABEL {keyValues.Select(Function(a) $"{a.Key}=""{a.Value}""").JoinBy(", ")}"
        End Function
    End Class
End Namespace
