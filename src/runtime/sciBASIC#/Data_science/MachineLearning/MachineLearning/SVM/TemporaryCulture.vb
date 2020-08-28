#Region "Microsoft.VisualBasic::db1f03967a80f310e8af3ba7ff311e92, Data_science\MachineLearning\MachineLearning\SVM\TemporaryCulture.vb"

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

    '     Module TemporaryCulture
    ' 
    '         Sub: [Stop], Start
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Globalization
Imports System.Threading

Namespace SVM
    Friend Module TemporaryCulture
        Private _culture As CultureInfo

        Public Sub Start()
            _culture = Thread.CurrentThread.CurrentCulture
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture
        End Sub

        Public Sub [Stop]()
            Thread.CurrentThread.CurrentCulture = _culture
        End Sub
    End Module
End Namespace

