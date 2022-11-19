#Region "Microsoft.VisualBasic::2d706f989f86fd77c30934d0cefa1df6, GCModeller\core\Bio.Assembly\Assembly\NCBI\Taxonomy\Tree\names.dmp.vb"

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

    '   Total Lines: 85
    '    Code Lines: 44
    ' Comment Lines: 33
    '   Blank Lines: 8
    '     File Size: 3.08 KB


    '     Class names
    ' 
    '         Properties: name_class, name_txt, tax_id, unique_name
    ' 
    '         Function: BuildTaxiIDFinder, FileParser, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language

Namespace Assembly.NCBI.Taxonomy

    ''' <summary>
    ''' Taxonomy names file (``names.dmp``):
    ''' + tax_id		-- the id of node associated with this name
    ''' + name_txt		-- name itself
    ''' + unique name	-- the unique Variant Of this name If name Not unique
    ''' + name Class    -- (Of synonym, common name, ...)
    ''' </summary>
    Public Class names

        ''' <summary>
        ''' -- the id Of node associated With this name
        ''' </summary>
        ''' <returns></returns>
        Public Property tax_id As Integer
        ''' <summary>
        ''' -- name itself
        ''' </summary>
        ''' <returns></returns>
        Public Property name_txt As String
        ''' <summary>
        ''' -- the unique Variant Of this name If name Not unique
        ''' </summary>
        ''' <returns></returns>
        Public Property unique_name As String
        ''' <summary>
        ''' -- (Of synonym, common name, ...)
        ''' </summary>
        ''' <returns></returns>
        Public Property name_class As String

        Public Overrides Function ToString() As String
            Return $"({tax_id}) {name_txt}"
        End Function

        ''' <summary>
        ''' Parsering data from ``names.dmp``
        ''' </summary>
        ''' <param name="dump$">``names.dmp``</param>
        ''' <returns></returns>
        Public Shared Iterator Function FileParser(dump$) As IEnumerable(Of names)
            For Each line As String In dump.IterateAllLines
                Dim tokens$() = line.StringSplit("\t|\t")

                Yield New names With {
                    .tax_id = CInt(tokens(Scan0)),
                    .name_txt = tokens(2),
                    .unique_name = tokens(4),
                    .name_class = tokens(6)
                }
            Next
        End Function

        ''' <summary>
        ''' Helper function for <see cref="NcbiTaxonomyTree"/>, which is required of taxid for build tree.
        ''' </summary>
        ''' <param name="dump$"></param>
        ''' <returns></returns>
        Public Shared Function BuildTaxiIDFinder(dump$) As Func(Of String, names())
            Dim names = FileParser(dump) _
                .GroupBy(Function(x) x.name_txt.ToLower) _
                .ToDictionary(Function(name) name.Key,
                              Function(list)
                                  Return list.ToArray
                              End Function)

            Return Function(name$) As names()
                       If name.StringEmpty Then
                           Return Nothing
                       Else
                           With name.ToLower
                               If Not names.ContainsKey(.ByRef) Then
                                   Return {}
                               Else
                                   Return names(.ByRef)
                               End If
                           End With
                       End If
                   End Function
        End Function
    End Class
End Namespace
