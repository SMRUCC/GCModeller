#Region "Microsoft.VisualBasic::7703f1a2c9d20fabb0a52605ac412997, ..\httpd\HTTPServer\REST\Program\Configs.vb"

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

Imports Microsoft.VisualBasic.Serialization.JSON

Public Class Configs
    Public Property Portal As Integer
    Public Property WWWroot As String
    Public Property App As String

    Public Shared ReadOnly Property DefaultFile As String = HOME & "/httpd.json"

    Public Shared Function LoadDefault() As Configs
        Try
            Return JsonContract.LoadJsonFile(Of Configs)(DefaultFile)
        Catch ex As Exception
            ex = New Exception(DefaultFile, ex)
            Call LogException(ex)
            Dim __new As New Configs With {
                .Portal = 80,
                .WWWroot = HOME & "/wwwroot/"
            }
            Call __new.Save()
            Return __new
        End Try
    End Function

    Sub Save()
        Call Me.GetJson.SaveTo(DefaultFile)
    End Sub
End Class

