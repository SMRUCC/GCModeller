#Region "Microsoft.VisualBasic::2066056ce0c57be1055ea1b38f3a9d3b, models\Networks\Network.BLAST\DataModels\BLAST.vb"

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

    '     Class BLAST
    ' 
    '         Properties: BlastHits, Proteins
    ' 
    '         Function: (+2 Overloads) Save
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Text

Namespace LDM

    ''' <summary>
    ''' blast网络
    ''' </summary>
    Public Class BLAST : Implements ISaveHandle

        Public Property Proteins As Protein()
        Public Property BlastHits As Hit()

        Public Function Save(DIR As String, encoding As Encoding) As Boolean Implements ISaveHandle.Save
            Try
                Call Proteins.SaveTo(DIR & "/Proteins.Csv")
                Call BlastHits.SaveTo(DIR & "/Hits.Csv")

                Return True
            Catch ex As Exception
                Call App.LogException(New Exception(DIR, ex))
            End Try

            Return False
        End Function

        Public Function Save(Path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return Save(Path, encoding.CodePage)
        End Function
    End Class
End Namespace
