#Region "Microsoft.VisualBasic::768c25b057d306ddbc8e42c47301063c, ..\GCModeller\models\Networks\Network.BLAST\LDM\BLAST.vb"

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

Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.DocumentFormat.Csv

Namespace LDM

    ''' <summary>
    ''' blast网络
    ''' </summary>
    Public Class BLAST : Implements ISaveHandle

        Public Property Proteins As Protein()
        Public Property BlastHits As Hit()

        Public Function Save(Optional DIR As String = "", Optional encoding As Encoding = Nothing) As Boolean Implements ISaveHandle.Save
            Try
                Call Proteins.SaveTo(DIR & "/Proteins.Csv")
                Call BlastHits.SaveTo(DIR & "/Hits.Csv")

                Return True
            Catch ex As Exception
                Call App.LogException(New Exception(DIR, ex))
            End Try

            Return False
        End Function

        Public Function Save(Optional Path As String = "", Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return Save(Path, encoding.GetEncodings)
        End Function
    End Class
End Namespace
