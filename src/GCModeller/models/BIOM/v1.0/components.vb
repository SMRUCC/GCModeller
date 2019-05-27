#Region "Microsoft.VisualBasic::618ffa1c65ca6b6aeae3232aed4fd37f, v1.0\components.vb"

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

'     Class IntegerMatrix
' 
'         Function: LoadFile
' 
'     Class FloatMatrix
' 
'         Function: LoadFile
' 
'     Class StringMatrix
' 
'         Function: LoadFile
' 
'         Class row
' 
'             Properties: id, metadata
' 
'         Class meta
' 
'             Properties: KEGG_Pathways, taxonomy
' 
'         Class column
' 
'             Properties: id, metadata
' 
'         Class columnMeta
' 
'             Properties: BarcodeSequence, BODY_SITE, Description, LinkerPrimerSequence
' 
'             Function: ToString
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Metagenomics

Namespace v10

    ''' <summary>
    ''' BIOM json with integer matrix data
    ''' </summary>
    Public Class IntegerMatrix : Inherits Json(Of Integer)

        Public Overloads Shared Function LoadFile(path$) As IntegerMatrix
            Dim json$ = path.ReadAllText
            Dim biom As IntegerMatrix = JsonContract.EnsureDate(json, "date").LoadJSON(Of IntegerMatrix)
            Return biom
        End Function
    End Class

    ''' <summary>
    ''' BIOM json with double matrix data
    ''' </summary>
    Public Class FloatMatrix : Inherits Json(Of Double)

        Public Overloads Shared Function LoadFile(path$) As FloatMatrix
            Dim json$ = path.ReadAllText
            Dim biom As FloatMatrix = JsonContract.EnsureDate(json, "date").LoadJSON(Of FloatMatrix)
            Return biom
        End Function
    End Class

    ''' <summary>
    ''' BIOM json with string matrix data
    ''' </summary>
    Public Class StringMatrix : Inherits Json(Of String)

        Public Overloads Shared Function LoadFile(path$) As StringMatrix
            Dim json$ = path.ReadAllText
            Dim biom As StringMatrix = JsonContract.EnsureDate(json, "date").LoadJSON(Of StringMatrix)
            Return biom
        End Function
    End Class

    Namespace components

        Public Class row
            Public Property id As String
            Public Property metadata As meta
        End Class

        Public Class meta

            ''' <summary>
            ''' 这个字符串数组就是一个taxonomy对象的数据
            ''' </summary>
            ''' <returns></returns>
            Public Property taxonomy As String()
            Public Property KEGG_Pathways As String()

            ''' <summary>
            ''' 将<see cref="taxonomy"/>属性值转换为标准的物种信息数据模型，
            ''' 如果目标字符串数组是空的，则这个属性返回空值
            ''' </summary>
            ''' <returns></returns>
            Public ReadOnly Property lineage As Taxonomy
                Get
                    If taxonomy.IsNullOrEmpty Then
                        Return Nothing
                    Else
                        Return New Taxonomy(taxonomy)
                    End If
                End Get
            End Property
        End Class

        Public Class column
            Public Property id As String
            Public Property metadata As columnMeta
        End Class

        Public Class columnMeta
            Public Property BarcodeSequence As String
            Public Property LinkerPrimerSequence As String
            Public Property BODY_SITE As String
            Public Property Description As String

            Public Overrides Function ToString() As String
                Return Me.GetJson
            End Function
        End Class
    End Namespace
End Namespace
