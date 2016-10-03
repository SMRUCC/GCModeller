#Region "Microsoft.VisualBasic::0924b3d9761e27273458369d766f40bf, ..\GCModeller\analysis\Xfam\Pfam\MPAlignment\Algorithm\DomainEquals.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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

Imports SMRUCC.genomics.ProteinModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace ProteinDomainArchitecture.MPAlignment

    Public Class DomainEquals

        Public ReadOnly __high_Scoring_thresholds As Double
        ReadOnly __partEquals As Boolean

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="high_Scoring_thresholds"></param>
        ''' <param name="partEquals">位置得分是否满足一部分是高分的就行了？</param>
        Sub New(high_Scoring_thresholds As Double, partEquals As Boolean)
            __high_Scoring_thresholds = high_Scoring_thresholds
            __partEquals = partEquals
        End Sub

        Public Overrides Function ToString() As String
            Return New With {__high_Scoring_thresholds}.GetJson
        End Function

        Public Overloads Function Equals(a As DomainObject, b As DomainObject) As Boolean
            If Not String.Equals(a.Identifier, b.Identifier, StringComparison.OrdinalIgnoreCase) Then
                Return False
            End If

            Dim ps As Double() = {a.Location.Left, b.Location.Left}
            Dim leftCheck As Boolean
            Dim pn As Double

            If a.Location.Left = 0R AndAlso b.Location.Left = 0R Then
                leftCheck = True
            Else
                pn = ps.Min / ps.Max     'Max is 1 (when min = max)
                leftCheck = pn >= __high_Scoring_thresholds
            End If

            ps = {a.Location.Right, b.Location.Right}
            pn = ps.Min / ps.Max  'Max is 1 (when min = max)

            Dim rightCheck As Boolean = pn >= __high_Scoring_thresholds

            If leftCheck AndAlso rightCheck Then
                Return True
            Else
                If __partEquals Then
                    Return leftCheck OrElse rightCheck
                Else
                    Return False
                End If
            End If
        End Function
    End Class
End Namespace
