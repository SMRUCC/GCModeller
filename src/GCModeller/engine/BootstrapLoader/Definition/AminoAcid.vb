#Region "Microsoft.VisualBasic::cc17ca281a30c72a90a410dd1f3dde1c, engine\Dynamics\Engine\VCell\Loader\Definition\AminoAcid.vb"

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

    '     Class AminoAcid
    ' 
    '         Properties: A, C, D, E, F
    '                     G, H, I, K, L
    '                     M, N, O, P, Q
    '                     R, S, T, U, V
    '                     W, Y
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Engine.Definitions

    Public Class AminoAcid

        ''' <summary>
        ''' L-Alanine
        ''' </summary>
        ''' <returns></returns>
        Public Property A As String
        ''' <summary>
        ''' L-Arginine
        ''' </summary>
        ''' <returns></returns>
        Public Property R As String
        ''' <summary>
        ''' L-Asparagine
        ''' </summary>
        ''' <returns></returns>
        Public Property N As String
        ''' <summary>
        ''' L-Aspartic acid
        ''' </summary>
        ''' <returns></returns>
        Public Property D As String
        ''' <summary>
        ''' L-Cysteine
        ''' </summary>
        ''' <returns></returns>
        Public Property C As String
        ''' <summary>
        ''' L-Glutamic acid
        ''' </summary>
        ''' <returns></returns>
        Public Property E As String
        ''' <summary>
        ''' L-Glutamine
        ''' </summary>
        ''' <returns></returns>
        Public Property Q As String
        ''' <summary>
        ''' Glycine
        ''' </summary>
        ''' <returns></returns>
        Public Property G As String
        ''' <summary>
        ''' L-Histidine
        ''' </summary>
        ''' <returns></returns>
        Public Property H As String
        ''' <summary>
        ''' L-Isoleucine
        ''' </summary>
        ''' <returns></returns>
        Public Property I As String
        ''' <summary>
        ''' L-Leucine
        ''' </summary>
        ''' <returns></returns>
        Public Property L As String
        ''' <summary>
        ''' L-Lysine
        ''' </summary>
        ''' <returns></returns>
        Public Property K As String
        ''' <summary>
        ''' L-Methionine
        ''' </summary>
        ''' <returns></returns>
        Public Property M As String
        ''' <summary>
        ''' L-Phenylalanine
        ''' </summary>
        ''' <returns></returns>
        Public Property F As String
        ''' <summary>
        ''' L-Proline
        ''' </summary>
        ''' <returns></returns>
        Public Property P As String
        ''' <summary>
        ''' L-Serine
        ''' </summary>
        ''' <returns></returns>
        Public Property S As String
        ''' <summary>
        ''' L-Threonine
        ''' </summary>
        ''' <returns></returns>
        Public Property T As String
        ''' <summary>
        ''' L-Tryptophan
        ''' </summary>
        ''' <returns></returns>
        Public Property W As String
        ''' <summary>
        ''' L-Tyrosine
        ''' </summary>
        ''' <returns></returns>
        Public Property Y As String
        ''' <summary>
        ''' L-Valine
        ''' </summary>
        ''' <returns></returns>
        Public Property V As String
        ''' <summary>
        ''' L-Selenocysteine
        ''' </summary>
        ''' <returns></returns>
        Public Property U As String
        ''' <summary>
        ''' L-Pyrrolysine
        ''' </summary>
        ''' <returns></returns>
        Public Property O As String

        Shared ReadOnly aa As Dictionary(Of String, PropertyInfo)

        Shared Sub New()
            aa = DataFramework.Schema(Of AminoAcid)(PropertyAccess.Readable, True, True) _
                .Values _
                .Where(Function(p) p.Name.Length = 1) _
                .ToDictionary(Function(a)
                                  Return a.Name
                              End Function)
        End Sub

        Default Public ReadOnly Property Residue(compound As String) As String
            Get
                Return aa(compound).GetValue(Me)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Class
End Namespace
