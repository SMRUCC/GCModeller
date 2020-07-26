#Region "Microsoft.VisualBasic::e09b289d0c8e8632ec1a147874a8d480, localblast\LocalBLAST\LocalBLAST\BlastOutput\Reader\Blast+\2.6.0+\blastn\BlastnOutputReader.vb"

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

    '     Module BlastnOutputReader
    ' 
    '         Function: RunParser
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Text

Namespace LocalBLAST.BLASTOutput.BlastPlus

    Public Module BlastnOutputReader

        Public Iterator Function RunParser(path$, Optional encoding As Encodings = Encodings.UTF8) As IEnumerable(Of Query)
            Dim source As IEnumerable(Of String) = QueryBlockIterates(path, encoding)
            Dim q As Query

            For Each queryText As String In source
                q = Query.BlastnOutputParser(queryText)
                Yield q
            Next
        End Function
    End Module
End Namespace
