#Region "Microsoft.VisualBasic::c36f8e795d67b6f76ddf55f2a458bb33, GCModeller\engine\Compiler\AssemblyScript\Commands\Entry.vb"

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

    '   Total Lines: 68
    '    Code Lines: 52
    ' Comment Lines: 0
    '   Blank Lines: 16
    '     File Size: 2.27 KB


    '     Class Entry
    ' 
    ' 
    ' 
    '     Class EntryIdVector
    ' 
    '         Properties: id
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetEntries, ToString
    ' 
    '     Class CategoryEntry
    ' 
    '         Properties: categoryPath, className, matchPattern
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetEntries, mapKEGGObject, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.GCModeller.Compiler.AssemblyScript.Script

Namespace AssemblyScript.Commands

    Public MustInherit Class Entry

        Public MustOverride Function GetEntries() As String()

    End Class

    Public Class EntryIdVector : Inherits Entry

        Public Property id As String()

        Sub New(tokens As Token())
            id = tokens _
                .Where(Function(a) a.name <> Script.Tokens.comma) _
                .Select(Function(a)
                            Return Command.stripValueString(a.text)
                        End Function) _
                .ToArray
        End Sub

        Public Overrides Function GetEntries() As String()
            Return id
        End Function

        Public Overrides Function ToString() As String
            Return GetEntries.JoinBy(",")
        End Function
    End Class

    Public Class CategoryEntry : Inherits Entry

        Public Property className As KEGGObjects
        Public Property categoryPath As String()
        Public Property matchPattern As String

        Sub New(tokens As Token())
            Dim className As String = tokens(0).text
            Dim reference As String = Command.stripValueString(tokens(2).text)
            Dim refTokens As String() = reference.Split("\"c)

            Me.className = mapKEGGObject(className)
            Me.categoryPath = refTokens.Take(refTokens.Length - 1).ToArray
            Me.matchPattern = refTokens(refTokens.Length - 1)
        End Sub

        Private Shared Function mapKEGGObject(className As String) As KEGGObjects
            Select Case className.ToUpper
                Case "KO" : Return KEGGObjects.Orthology
                Case "MAP" : Return KEGGObjects.Pathway
                Case "EC" : Return KEGGObjects.Reaction
                Case Else
                    Throw New DataException(className)
            End Select
        End Function

        Public Overrides Function GetEntries() As String()
            Throw New NotImplementedException()
        End Function

        Public Overrides Function ToString() As String
            Return $"{className.Description}:""{categoryPath.JoinBy("\")}\{matchPattern}"""
        End Function
    End Class
End Namespace
