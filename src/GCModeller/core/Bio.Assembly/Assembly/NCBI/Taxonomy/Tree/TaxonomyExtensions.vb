#Region "Microsoft.VisualBasic::0612690f7f15ac794347a5e3ba287e06, GCModeller\core\Bio.Assembly\Assembly\NCBI\Taxonomy\Tree\TaxonomyExtensions.vb"

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
    ' Comment Lines: 6
    '   Blank Lines: 5
    '     File Size: 1.17 KB


    '     Module TaxonomyExtensions
    ' 
    '         Function: BuildBIOM
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Metagenomics

Namespace Assembly.NCBI.Taxonomy

    Public Module TaxonomyExtensions

        ''' <summary>
        ''' Create taxonomy lineage string with <see cref="BIOMPrefix"/>
        ''' </summary>
        ''' <param name="nodes"></param>
        ''' <returns></returns>
        ''' 
        <Extension> Public Function BuildBIOM(nodes As IEnumerable(Of TaxonomyNode)) As String
            Dim data As Dictionary(Of String, String) = TaxonomyNode.RankTable(nodes)
            Dim list As New List(Of String)

            For Each r$ In NcbiTaxonomyTree.stdranks.Reverse
                If data.ContainsKey(r) Then
                    list.Add(data(r$))
                Else
                    list.Add("")
                End If
            Next

            SyncLock BIOMPrefix
                Return list _
                    .SeqIterator _
                    .Select(Function(x) BIOMPrefix(x.i) & x.value) _
                    .JoinBy(";")
            End SyncLock
        End Function
    End Module
End Namespace
