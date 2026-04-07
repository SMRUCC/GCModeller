#Region "Microsoft.VisualBasic::c184a53d1b6ea462224cfc55b4a3abc1, analysis\Metagenome\Metagenome\Tools\greengenes\otu_taxonomy.vb"

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

    '   Total Lines: 59
    '    Code Lines: 39 (66.10%)
    ' Comment Lines: 11 (18.64%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 9 (15.25%)
    '     File Size: 2.06 KB


    '     Class otu_taxonomy
    ' 
    '         Properties: ID, Taxonomy
    ' 
    '         Function: (+2 Overloads) Load, Parser, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Metagenomics

Namespace greengenes

    ''' <summary>
    ''' otu id mappingg to taxonomy lineage information
    ''' </summary>
    Public Class otu_taxonomy : Implements INamedValue

        ''' <summary>
        ''' the otu id
        ''' </summary>
        ''' <returns></returns>
        Public Property ID As String Implements IKeyedEntity(Of String).Key
        ''' <summary>
        ''' the taxonomy lineage information
        ''' </summary>
        ''' <returns></returns>
        Public Property Taxonomy As Taxonomy

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return Taxonomy.CreateTable.Value.TaxonomyString
        End Function

        Public Shared Function Load(path As String) As IEnumerable(Of otu_taxonomy)
            Return path _
                .IterateAllLines _
                .AsParallel _
                .Select(AddressOf Parser)
        End Function

        Public Shared Iterator Function Load(file As Stream) As IEnumerable(Of otu_taxonomy)
            Using reader As New StreamReader(file)
                Dim line As Value(Of String) = ""

                Do While Not (line = reader.ReadLine) Is Nothing
                    Yield Parser(line)
                Loop
            End Using
        End Function

        Private Shared Function Parser(line As String) As otu_taxonomy
            Dim data = line.GetTagValue(vbTab, trim:=True)
            Dim table = BIOMTaxonomy.TaxonomyParser(data.Value)
            Dim taxonomy As New Taxonomy(table)

            Return New otu_taxonomy With {
                .ID = data.Name,
                .Taxonomy = taxonomy
            }
        End Function
    End Class
End Namespace
