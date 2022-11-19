#Region "Microsoft.VisualBasic::a4bda8277bfe9e2e47fcd2d470998593, GCModeller\engine\Compiler\AssemblyScript\VHDFile.vb"

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

    '   Total Lines: 89
    '    Code Lines: 68
    ' Comment Lines: 10
    '   Blank Lines: 11
    '     File Size: 3.29 KB


    '     Class VHDFile
    ' 
    '         Properties: base, environment, keywords, maintainers, metadata
    '                     modifications
    ' 
    '         Function: Parse
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.Compiler.AssemblyScript.Commands
Imports SMRUCC.genomics.GCModeller.Compiler.AssemblyScript.Script

Namespace AssemblyScript

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' 主要是应用于通过命令行来进行模型文件的自动化构建操作
    ''' </remarks>
    Public Class VHDFile

        ''' <summary>
        ''' build from a base model
        ''' </summary>
        ''' <returns></returns>
        Public Property base As From
        Public Property metadata As Label()
        Public Property maintainers As Maintainer()
        Public Property keywords As Keywords()
        Public Property environment As Env()
        Public Property modifications As Modification()

        Public Shared Function Parse(script As String) As VHDFile
            Dim scanner As New Scanner(script.SolveStream)
            Dim tokenList = scanner.GetTokens.ToArray
            Dim assemblyCode As Token() = tokenList.Where(Function(a) Not a.name = Tokens.comment).ToArray
            Dim directive = assemblyCode _
                .Split(Function(a) a.name = Tokens.keyword, DelimiterLocation.NextFirst) _
                .Where(Function(a) a.Length > 0) _
                .GroupBy(Function(a) a(Scan0).text) _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return a.ToArray
                              End Function)
            Dim vhd As New VHDFile

            If Not directive.ContainsKey("FROM") Then
                Throw New DataException("no base model!")
            ElseIf directive("FROM").Length > 1 Then
                Throw New DataException("duplicated base model")
            Else
                vhd.base = New From(directive("FROM").First)
            End If

            If Not directive.ContainsKey("MAINTAINER") Then
                vhd.maintainers = {New Maintainer With {.authorName = App.UserHOME.BaseName}}
            Else
                vhd.maintainers = directive("MAINTAINER") _
                    .Select(Function(a) New Maintainer(a)) _
                    .ToArray
            End If

            vhd.keywords = directive _
                .TryGetValue("KEYWORDS") _
                .SafeQuery _
                .Select(Function(a) New Keywords(a)) _
                .ToArray
            vhd.metadata = directive _
                .TryGetValue("LABEL") _
                .SafeQuery _
                .Select(Function(a) New Label(a)) _
                .ToArray
            vhd.environment = directive _
                .TryGetValue("ENV") _
                .SafeQuery _
                .Select(Function(a) New Env(a)) _
                .ToArray

            Dim add = directive _
                .TryGetValue("ADD") _
                .SafeQuery _
                .Select(Function(a) DirectCast(New Add(a), Modification)) _
                .AsList
            Dim delete = directive _
                .TryGetValue("DELETE") _
                .SafeQuery _
                .Select(Function(a) DirectCast(New Delete(a), Modification)) _
                .ToArray

            vhd.modifications = add + delete

            Return vhd
        End Function

    End Class
End Namespace
