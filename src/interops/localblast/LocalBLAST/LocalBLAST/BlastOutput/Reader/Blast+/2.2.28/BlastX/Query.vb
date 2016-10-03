#Region "Microsoft.VisualBasic::d7780c049713baab60de2c588a095087, ..\interops\localblast\LocalBLAST\LocalBLAST\BlastOutput\Reader\Blast+\2.2.28\BlastX\Query.vb"

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

Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace LocalBLAST.BLASTOutput.BlastPlus.BlastX.Components

    Public Class Query

        Dim __hits As HitFragment()

        Public Property QueryName As String
        Public Property QueryLength As Integer
        Public Property SubjectName As String
        Public Property SubjectLength As Integer
        Public Property Hits As HitFragment()
            Get
                Return __hits
            End Get
            Set(value As HitFragment())
                __hits = LinqAPI.Exec(Of HitFragment) <=
                    From x As HitFragment
                    In value
                    Select x.SetStack(Me)
            End Set
        End Property

        Public Overrides Function ToString() As String
            If Hits.IsNullOrEmpty Then
                Return String.Format("{0}  <===>  {1}  (HITS_NOT_FOUND)", QueryName, SubjectName)
            Else
                Return String.Format("{0}  <===>  {1}", QueryName, SubjectName)
            End If
        End Function

        ''' <summary>
        ''' 这个函数过滤掉所有过短的比对片段
        ''' </summary>
        ''' <returns></returns>
        ''' <param name="p">0-1之间的数，用于表示长度的百分比</param>
        ''' <remarks></remarks>
        Public Function FilteringSegments(p As Double) As HitFragment()
            Dim LQuery =
                LinqAPI.Exec(Of HitFragment) <= From hsp As HitFragment
                                                In Hits
                                                Where hsp.SubjectLength / SubjectLength >= p
                                                Select hsp
            Return LQuery
        End Function
    End Class
End Namespace
