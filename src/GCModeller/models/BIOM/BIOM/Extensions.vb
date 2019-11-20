#Region "Microsoft.VisualBasic::27b3e92ca38831ef4487b9569e8e1bd1, models\BIOM\BIOM\Extensions.vb"

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

    ' Module Extensions
    ' 
    '     Function: BIOMVersion, ReadAuto
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.IO.HDF5
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Scripting.SymbolBuilder.VBLanguage
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.foundation.BIOM.v10

<HideModuleName> Public Module Extensions

    ''' <summary>
    ''' 自动根据文件拓展名以及hdf5文件版本来选择读取程序
    ''' </summary>
    ''' <param name="path"></param>
    ''' <param name="denseMatrix">
    ''' 当这个参数为真的时候，所返回的json对象之中的矩阵数据都会被强制的转换为全矩阵
    ''' </param>
    ''' <returns></returns>
    Public Function ReadAuto(path As String, Optional denseMatrix As Boolean = False) As v10.BIOMDataSet(Of Double)
        Dim BIOM As v10.BIOMDataSet(Of Double)

        If path.ExtensionSuffix.TextEquals("json") Then
            BIOM = v10.FloatMatrix.LoadFile(path)
        Else
            With BIOMVersion(path)
                If .TextEquals("2.0") Then
                    BIOM = v20.ReadFile(path)
                ElseIf .TextEquals("2.1") Then
                    BIOM = v21.ReadFile(path)
                Else
                    Throw New NotSupportedException
                End If
            End With
        End If

        If denseMatrix AndAlso BIOM.RequiredConvertToDenseMatrix Then
            BIOM.matrix_type = matrix_type.dense
            BIOM.data = BIOM.data.ToDenseMatrix(BIOM.shape)
        End If

        Return BIOM
    End Function

    Public Function BIOMVersion(path As String) As String
        If path.ExtensionSuffix.TextEquals("json") Then
            Return "1.0"
        Else
            Using hdf5 As New HDF5File(path)
                Dim version As Integer() = hdf5.attributes _
                    .AsCharacter _
                    .AsVBIdentifier _
                   !format_version _
                    .LoadJSON(Of Integer())
                Dim verStr$ = version.JoinBy(".")

                Return verStr
            End Using
        End If
    End Function
End Module
