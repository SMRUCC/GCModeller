#Region "Microsoft.VisualBasic::a98db64637c6541a22bd5f639ff20444, data\Xfam\Pfam\Pipeline\Database\SequenceTable.vb"

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

    '   Total Lines: 20
    '    Code Lines: 15 (75.00%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 5 (25.00%)
    '     File Size: 683 B


    '     Class PfamCsvRow
    ' 
    '         Properties: Ends, SequenceData, Start
    ' 
    '         Function: CreateObject
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.SequenceModel

Namespace Pipeline.Database

    Public Class PfamCsvRow : Inherits PfamEntryHeader
        Implements IPolymerSequenceModel

        Public Property Start As Integer
        Public Property Ends As Integer
        Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData

        Public Shared Function CreateObject(FastaData As PfamFasta) As PfamCsvRow
            Dim row As PfamCsvRow = FastaData.ShadowCopy(Of PfamCsvRow)()
            row.Start = FastaData.location.Left
            row.Ends = FastaData.location.Right

            Return row
        End Function
    End Class
End Namespace
