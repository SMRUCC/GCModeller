#Region "Microsoft.VisualBasic::d12ffb18a86a033878e5d578cbe1574d, core\test\equationparsertest.vb"

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

    '   Total Lines: 18
    '    Code Lines: 13 (72.22%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 5 (27.78%)
    '     File Size: 599 B


    ' Module equationparsertest
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes

Module equationparsertest

    Sub Main()

        Dim eq1$ = "C00323 + 3 C00083 <=> C15525 + 4 C00010 + 3 C00011"
        Dim eq2$ = "C00024 + C00400 <=> C00010 + C09870"
        Dim eq3$ = "C00024 + C00083 + 2C00005 + 4C00080 <=> C02843 + C00010 + C00011 + 2C00006 + C00001"

        Call Equation.TryParse(eq1).GetJson.debug
        Call Equation.TryParse(eq2).GetJson.debug
        Call Equation.TryParse(eq3).GetJson.debug

        Pause()
    End Sub
End Module
