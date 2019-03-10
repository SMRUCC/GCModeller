﻿#Region "Microsoft.VisualBasic::582aa8308621545cc9a989afc86a8c49, tutorials\core.test\unitTest.vb"

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

    ' Module unitTest
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Ranges

Module unitTest

    Sub Main()
        Dim KB As New UnitValue(Of ByteSize)(128 * 1024 * 1024, ByteSize.KB)

        Call KB.__DEBUG_ECHO
        Call KB.Scale(ByteSize.GB).__DEBUG_ECHO
        Call KB.Scale(ByteSize.B).__DEBUG_ECHO
        Call KB.Scale(ByteSize.MB).__DEBUG_ECHO
        Call KB.Scale(ByteSize.TB).__DEBUG_ECHO
        Call KB.Scale(ByteSize.KB).__DEBUG_ECHO
        Call KB.Scale(ByteSize.TB).Scale(ByteSize.MB).__DEBUG_ECHO

        Pause()
    End Sub
End Module
