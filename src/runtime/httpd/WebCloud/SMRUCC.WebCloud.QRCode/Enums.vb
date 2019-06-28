#Region "Microsoft.VisualBasic::c848fb72099482a974d50baeac91392f, WebCloud\SMRUCC.WebCloud.QRCode\Enums.vb"

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

    ' Enum ModuleType
    ' 
    '     Dark, Light
    ' 
    '  
    ' 
    ' 
    ' 
    ' Enum Mode
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' Enum SymbolType
    ' 
    '     Micro, Normal
    ' 
    '  
    ' 
    ' 
    ' 
    ' Enum ErrorCorrection
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel

''' <summary>
''' Type of an individual module (pixel) of a QR symbol.
''' </summary>
Public Enum ModuleType
    Light
    Dark
End Enum

''' <summary>
''' QR encoding modes
''' </summary>
Public Enum Mode
    ECI = 0
    Numeric = 1
    AlphaNumeric = 2
    [Byte] = 3
    Kanji = 4
    StructuredAppend = 5
    FNC1_FirstPosition = 6
    FNC1_SecondPosition = 7
    Terminator = 8
End Enum

''' <summary>
''' QR symbol types
''' </summary>
Public Enum SymbolType
    Micro
    Normal
End Enum

''' <summary>
''' QR error correction modes
''' </summary>
Public Enum ErrorCorrection
    <Description("Error-Detection Only")>
    None = 0
    <Description("L (7%)")>
    L = 1
    <Description("M (15%)")>
    M = 2
    <Description("Q (25%)")>
    Q = 3
    <Description("H (30%)")>
    H = 4
End Enum
