#Region "Microsoft.VisualBasic::f972247a68a00fb8d44db3a1f6481fb4, GO_gene-ontology\GO_Annotation\toGO.vb"

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

    ' Class toGO
    ' 
    '     Properties: entry, map2GO_id, map2GO_term, name
    ' 
    '     Function: Parse2GO, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

''' <summary>
''' External annotation map to go term
''' </summary>
Public Class toGO

    ' Pfam:PF00001 7tm_1 > GO:G protein-coupled receptor activity ; GO:0004930

    ''' <summary>
    ''' Annotation entry id, example as pfamId.
    ''' (请注意，一个entry可能会map到好几个go term上面)
    ''' </summary>
    ''' <returns></returns>
    Public Property entry As String
    ''' <summary>
    ''' The common name of the target <see cref="entry"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property name As String
    ''' <summary>
    ''' The name of the go term
    ''' </summary>
    ''' <returns></returns>
    Public Property map2GO_term As String
    ''' <summary>
    ''' The mapped go term id
    ''' </summary>
    ''' <returns></returns>
    Public Property map2GO_id As String

    Public Overrides Function ToString() As String
        Return $"Dim {entry}[{name}] As {map2GO_id} = '{map2GO_term}'"
    End Function

    Public Shared Iterator Function Parse2GO(file As String) As IEnumerable(Of toGO)
        Dim lines As String() = file.SolveStream _
            .LineTokens _
            .SkipWhile(Function(line) line.First = "!"c) _
            .ToArray
        Dim tokens$()
        Dim from As NamedValue(Of String)
        Dim mapTo As NamedValue(Of String)
        Dim mapping As toGO

        For Each line As String In lines
            tokens = line.Split(">"c)
            from = tokens(0).GetTagValue(" ", trim:=True)
            mapTo = tokens(1).GetTagValue(";", trim:=True)
            mapping = New toGO With {
                .entry = from.Name,
                .name = from.Value,
                .map2GO_term = mapTo.Name,
                .map2GO_id = mapTo.Value
            }

            Yield mapping
        Next
    End Function

End Class
