#Region "Microsoft.VisualBasic::186021c2bc31c642c10a38694703b77f, ..\GCModeller\CLI_tools\eggHTS\Test\Module2.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.genomics.Visualize

Module Module2

    Sub Main()

        Dim sample = EntityObject.LoadDataSet("G:\GCModeller\GCModeller\R\vocano\qlfTable.csv")
        Call Volcano.PlotDEGs(sample, pvalue:="PValue", displayLabel:=LabelTypes.DEG).Save("x:\test.png")
    End Sub
End Module
