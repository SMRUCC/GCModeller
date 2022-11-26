#Region "Microsoft.VisualBasic::3da4e74e7c08816cdebc353ddbc2b816, GCModeller\data\GO_gene-ontology\GeneOntology\Files\Obo\Definition.vb"

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

    '   Total Lines: 48
    '    Code Lines: 34
    ' Comment Lines: 4
    '   Blank Lines: 10
    '     File Size: 1.53 KB


    '     Class Definition
    ' 
    '         Properties: definition, evidences, isOBSOLETE
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: (+2 Overloads) Parse, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Namespace OBO

    ''' <summary>
    ''' text parser of <see cref="Term.def"/>
    ''' </summary>
    Public Class Definition

        Public Property definition As String
        Public Property evidences As String()
        Public Property isOBSOLETE As Boolean

        Sub New()
        End Sub

        Public Overrides Function ToString() As String
            Dim OBSOLETE = If(isOBSOLETE, "OBSOLETE. ", "")

            ' add OBSOLETE. tags if it is true
            Return $"""{OBSOLETE}{definition}"" [{evidences.JoinBy(", ")}]"
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function Parse(term As Term) As Definition
            Return Parse(term.def)
        End Function

        Public Shared Function Parse(dataLine As String) As Definition
            Dim info = dataLine.GetStackValue("""", """")
            Dim evidences = dataLine.Match("\[.+?\]", RegexICSng).GetStackValue("[", "]")
            Dim OBSOLETE = InStr(info, "OBSOLETE.") = 1

            If OBSOLETE Then
                info = Mid(info, 10).Trim
            End If

            Return New Definition With {
                .isOBSOLETE = OBSOLETE,
                .definition = info,
                .evidences = evidences _
                    .Split(","c) _
                    .Select(AddressOf Strings.Trim) _
                    .ToArray
            }
        End Function
    End Class
End Namespace
