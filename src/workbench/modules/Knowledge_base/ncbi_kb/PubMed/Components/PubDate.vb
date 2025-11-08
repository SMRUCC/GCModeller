#Region "Microsoft.VisualBasic::2967b1852a5b328e264ab31bdede8ed4, modules\Knowledge_base\ncbi_kb\PubMed\Components\PubDate.vb"

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

'   Total Lines: 25
'    Code Lines: 19 (76.00%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 6 (24.00%)
'     File Size: 804 B


'     Class PubDate
' 
'         Properties: DateType, Day, Hour, Minute, Month
'                     PubStatus, Year
' 
'         Function: ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Globalization
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ValueTypes

Namespace PubMed

    Public Class PubDate

        <XmlAttribute> Public Property PubStatus As String
        <XmlAttribute> Public Property DateType As String

        Public Property Year As String
        Public Property Month As String
        Public Property Day As String
        Public Property Hour As String
        Public Property Minute As String

        Sub New()
        End Sub

        Sub New([date] As String)
            ' 2022 Dec 27
            Const format As String = "yyyy MMM dd"
            Static provider As IFormatProvider = CultureInfo.InvariantCulture

            Dim result As DateTime = DateTime.ParseExact([date], format, provider)

            Year = result.Year
            Month = result.Month
            Day = result.Day
        End Sub

        Public Overrides Function ToString() As String
            Return $"[{PubStatus}] {Year}-{Month}-{Day}"
        End Function

        Public Overloads Shared Narrowing Operator CType(d As PubDate) As Date
            Return New Date(d.Year, DateTimeHelper.GetMonthInteger(d.Month), d.Day)
        End Operator
    End Class
End Namespace
