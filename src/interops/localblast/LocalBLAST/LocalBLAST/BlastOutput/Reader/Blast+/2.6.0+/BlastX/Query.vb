#Region "Microsoft.VisualBasic::c76cb29d9bec81fe339f5048050b8489, LocalBLAST\LocalBLAST\BlastOutput\Reader\Blast+\2.6.0+\BlastX\Query.vb"

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

    '     Class Query
    ' 
    '         Properties: QueryLength, QueryName, Subjects
    ' 
    '         Function: ToString
    ' 
    '     Class Subject
    ' 
    '         Properties: Hits, SubjectLength, SubjectName
    ' 
    '         Function: FilteringSegments, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language

Namespace LocalBLAST.BLASTOutput.BlastPlus.BlastX.Components

    ''' <summary>
    ''' The blastx query input result data.
    ''' 
    ''' ```
    ''' + Query
    '''   + Subject
    '''      + Fragment
    '''         + HSP1
    '''         + HSP2
    '''         + HSP3
    '''      + Fragment
    '''      ...
    ''' ```
    ''' </summary>
    Public Class Query

        Public Property QueryName As String
        Public Property QueryLength As Integer

        ''' <summary>
        ''' The hits result in target protein sequence database.
        ''' </summary>
        ''' <returns></returns>
        Public Property Subjects As Subject()

        Public Overrides Function ToString() As String
            If Subjects.IsNullOrEmpty Then
                Return "***** No hits found *****"
            Else
                Return QueryName
            End If
        End Function
    End Class

    ''' <summary>
    ''' The hits result in target protein sequence database.
    ''' </summary>
    Public Class Subject

        Public Property SubjectName As String
        Public Property SubjectLength As Integer
        ''' <summary>
        ''' The blastx subject hit is consist with sevral fragment.
        ''' </summary>
        ''' <returns></returns>
        Public Property Hits As HitFragment()

        Public Overrides Function ToString() As String
            Return SubjectName
        End Function

        ''' <summary>
        ''' 这个函数过滤掉所有过短的比对片段
        ''' </summary>
        ''' <returns></returns>
        ''' <param name="p">0-1之间的数，用于表示长度的百分比</param>
        ''' <remarks></remarks>
        Public Function FilteringSegments(p As Double) As HitFragment()
            Dim LQuery = LinqAPI.Exec(Of HitFragment) _
 _
                () <= From hsp As HitFragment
                      In Hits
                      Where hsp.SubjectLength / SubjectLength >= p
                      Select hsp

            Return LQuery
        End Function
    End Class
End Namespace
