#Region "Microsoft.VisualBasic::6a4d4fbbe22efd4def27041b6308366a, Bio.Assembly\Assembly\MetaCyc\File\Tabular DataFiles\Pathways.vb"

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

    '     Class Pathways
    ' 
    '         Properties: DbProperty, Objects
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Xml.Serialization
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.DataTabular
Imports Microsoft.VisualBasic

Namespace Assembly.MetaCyc.File.TabularDataFiles

    ''' <summary>
    ''' (pathways.col) For each pathway in the PGDB, the file lists the genes that 
    ''' encode the enzymes in that pathway.
    ''' </summary>
    ''' <remarks>
    ''' Columns (multiple columns are indicated in parentheses; n is the maximum 
    ''' number of genes for all pathways in the PGDB):
    ''' 
    '''   UNIQUE-ID
    '''   NAME
    '''   GENE-NAME (n)
    '''   GENE-ID (n)
    ''' 
    ''' </remarks>
    Public Class Pathways

        Public Property DbProperty As [Property]
        Public Property Objects As Pathway()

        Public Shared Widening Operator CType(Path As String) As Pathways
            Dim File As MetaCyc.File.TabularFile = Path
            Dim NewObj As Generic.IEnumerable(Of Pathway) =
                From e As RecordLine
                In File.Objects
                Select CType(e.Data, Pathway)

            Return New Pathways With {.DbProperty = File.DbProperty, .Objects = NewObj.ToArray}
        End Operator
    End Class
End Namespace
