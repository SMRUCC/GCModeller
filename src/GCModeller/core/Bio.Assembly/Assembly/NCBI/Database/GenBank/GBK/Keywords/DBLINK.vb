#Region "Microsoft.VisualBasic::5247c6c93a830a1e7ccb703b5bf187f8, ..\GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\GenBank\GBK\Keywords\DBLINK.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Namespace Assembly.NCBI.GenBank.GBFF.Keywords

    Public Class DBLINK : Inherits KeyWord

        Public Property Db As String
        Public Property Id As String

        Public Overrides Function ToString() As String
            Return $"DBLINK      {Db}: {Id}"
        End Function

        Friend Shared Function Parser(sList As String()) As DBLINK
            Dim s As String = If(sList.IsNullOrEmpty, "", sList.FirstOrDefault)

            If String.IsNullOrEmpty(s) Then
                Return New DBLINK With {.Db = "unknown"}
            End If

            Dim tokens As String() = Mid(s, 13).Trim.Split(":"c)
            Return New DBLINK With {
                .Db = tokens.Get(Scan0).Trim,
                .Id = tokens.Get(1).Trim
            }
        End Function
    End Class
End Namespace
