#Region "Microsoft.VisualBasic::242cc894efb9380e68230112cccb616c, analysis\SequenceToolkit\MotifFinder\Seeds\SeedScanner.vb"

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

'   Total Lines: 16
'    Code Lines: 10 (62.50%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 6 (37.50%)
'     File Size: 420 B


' Class SeedScanner
' 
'     Constructor: (+1 Overloads) Sub New
' 
' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Analysis.SequenceAlignment.BestLocalAlignment
Imports SMRUCC.genomics.SequenceModel.FASTA

Public MustInherit Class SeedScanner

    Protected ReadOnly param As PopulatorParameter
    Protected ReadOnly debug As Boolean

    Sub New(param As PopulatorParameter, debug As Boolean)
        Me.param = param
        Me.debug = debug
    End Sub

    Public MustOverride Iterator Function GetSeeds(regions As FastaSeq()) As IEnumerable(Of HSP)

End Class
