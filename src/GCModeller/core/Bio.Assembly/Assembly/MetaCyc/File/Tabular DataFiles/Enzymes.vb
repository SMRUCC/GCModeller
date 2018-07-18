#Region "Microsoft.VisualBasic::3ad03410a35a9443e41d953e35203009, Bio.Assembly\Assembly\MetaCyc\File\Tabular DataFiles\Enzymes.vb"

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

    '     Class Enzymes
    ' 
    '         Properties: DbProperty, Objects
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text

Namespace Assembly.MetaCyc.File.TabularDataFiles

    ''' <summary>
    ''' (enzymes.col) For each enzymatic reaction in the PGDB, the file lists the 
    ''' reaction equation, up to 4 pathways that contain the reaction, up to 4 
    ''' cofactors for the enzyme, up to 4 activators, up to 4 inhibitors, and the 
    ''' subunit structure of the enzyme.
    ''' </summary>
    ''' <remarks>
    ''' Columns (multiple columns are indicated in parentheses):
    ''' 
    '''   UNIQUE-ID
    '''   NAME
    '''   REACTION-EQUATION
    '''   PATHWAYS (4)
    '''   COFACTORS (4)
    '''   ACTIVATORS (4)
    '''   INHIBITORS (4)
    '''   SUBUNIT-COMPOSITION
    ''' 
    ''' </remarks>
    Public Class Enzymes

        Public Property Objects As [Object]()
        Public Property DbProperty As [Property]

        Public Shared Widening Operator CType(spath As String) As Enzymes
            Dim DataFile As MetaCyc.File.TabularFile = spath
            Dim NewObj As Generic.IEnumerable(Of [Object]) =
                From Line As RecordLine
                In DataFile.Objects
                Select [Object].GetData(Line)

            Return New Enzymes With {
                .DbProperty = DataFile.DbProperty,
                .Objects = NewObj.ToArray
            }
        End Operator
    End Class
End Namespace
