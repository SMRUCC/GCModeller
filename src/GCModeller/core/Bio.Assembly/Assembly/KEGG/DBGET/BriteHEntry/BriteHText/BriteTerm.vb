#Region "Microsoft.VisualBasic::b9f0cafe38298618520601e1d4b5591f, Bio.Assembly\Assembly\KEGG\DBGET\BriteHEntry\BriteHText\BriteTerm.vb"

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

    '     Class BriteTerm
    ' 
    '         Properties: [class], category, entry, order, subcategory
    ' 
    '         Function: BuildPath, GetInformation, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports Microsoft.VisualBasic.Scripting.SymbolBuilder.VBLanguage

Namespace Assembly.KEGG.DBGET.BriteHEntry

    ''' <summary>
    ''' A general brite term
    ''' </summary>
    Public Class BriteTerm

        ''' <summary>
        ''' A
        ''' </summary>
        ''' <returns></returns>
        Public Property [class] As String
        ''' <summary>
        ''' B
        ''' </summary>
        ''' <returns></returns>
        Public Property category As String
        ''' <summary>
        ''' C
        ''' </summary>
        ''' <returns></returns>
        Public Property subcategory As String
        ''' <summary>
        ''' D
        ''' </summary>
        ''' <returns></returns>
        Public Property order As String
        ''' <summary>
        ''' ``{compoundID => name}``
        ''' </summary>
        ''' <returns></returns>
        Public Property entry As KeyValuePair

        Public Overrides Function ToString() As String
            Return entry.ToString
        End Function

        Friend Shared Function GetInformation(resourceName$, entryIDPattern$) As BriteTerm()
            Static satellite As New ResourcesSatellite(GetType(LICENSE))

            Dim resource$ = Nothing

            If resourceName.IsPattern(Patterns.Identifer, RegexICSng) Then
                resource = satellite.GetString(resourceName)
            ElseIf resourceName.IsURLPattern Then
                With resourceName.Split("?"c).Last.Match("[0-9a-zA-Z_]+\.keg")
                    If Not .StringEmpty Then
                        resource = satellite.GetString(.Replace(".keg", ""))
                    End If
                End With

                If resource.StringEmpty Then
                    resource = resourceName.GET
                End If
            ElseIf resourceName.FileExists Then
                resource = resourceName.ReadAllText
            Else
                Throw New NotImplementedException(resourceName)
            End If

            Dim htext = BriteHTextParser.Load(resource)
            Dim terms = TreeParser.Deflate(htext, entryIDPattern).ToArray

            Return terms
        End Function

        Public Function BuildPath(EXPORT$, directoryOrganized As Boolean, Optional class$ = "") As String
            With Me
                If directoryOrganized Then
                    Dim t As New List(Of String) From {
                        EXPORT,
                        BriteHText.NormalizePath(.class),
                        BriteHText.NormalizePath(.category),
                        BriteHText.NormalizePath(.subcategory)
                    }

                    If Not [class].StringEmpty Then
                        Call t.Insert(index:=1, item:=[class])
                    End If

                    Return String.Join("/", t)
                Else
                    Return EXPORT
                End If
            End With
        End Function
    End Class
End Namespace
