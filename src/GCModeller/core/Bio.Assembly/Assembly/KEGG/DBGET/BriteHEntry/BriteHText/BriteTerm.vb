#Region "Microsoft.VisualBasic::304b063c0b2bd7dee157f7fef9d35bda, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\BriteHEntry\BriteHText\BriteTerm.vb"

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

    '   Total Lines: 80
    '    Code Lines: 43
    ' Comment Lines: 27
    '   Blank Lines: 10
    '     File Size: 2.44 KB


    '     Class BriteTerm
    ' 
    '         Properties: [class], category, entry, kegg_id, order
    '                     subcategory
    ' 
    '         Function: BuildPath, GetInformation, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Text.Xml.Models

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

        ''' <summary>
        ''' <see cref="KeyValuePair.Key"/> of property <see cref="entry"/>
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property kegg_id As String
            Get
                Return entry.Key
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return entry.ToString
        End Function

        Friend Shared Function GetInformation(resourceName$, entryIDPattern$) As BriteTerm()
            Dim htext As htext = htext.GetInternalResource(resourceName)
            Dim terms As BriteTerm() = BriteTreeDeflater _
                .Deflate(htext.Hierarchical, entryIDPattern) _
                .ToArray

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
