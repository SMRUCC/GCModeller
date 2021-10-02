#Region "Microsoft.VisualBasic::ed2fdb26306c20aa1b3074f094cc4d8a, HMP_client\metadata.vb"

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

    ' Class metadata
    ' 
    '     Properties: project_name, sample_body_site, sample_fecalcal, sample_id, study_full_name
    '                 subject_alcohol, subject_asthma, subject_cad, subject_diabetes, subject_education
    '                 subject_gender, subject_id, subject_mother, subject_occupation, subject_race
    '                 subject_rx, visits
    ' 
    '     Function: LoadTable
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Text
Imports csv = Microsoft.VisualBasic.Data.csv.IO.File

''' <summary>
''' 数据文件所对应的样本信息
''' </summary>
Public Class metadata

    Public Property sample_id As String
    Public Property subject_id As String
    Public Property sample_body_site As String
    Public Property subject_gender As String
    Public Property subject_race As String
    Public Property study_full_name As String
    Public Property project_name As String
    Public Property subject_alcohol As String
    Public Property subject_education As String
    Public Property subject_mother As String
    Public Property subject_rx As String
    Public Property subject_diabetes As String
    Public Property subject_occupation As String
    Public Property subject_asthma As String
    Public Property subject_cad As String
    Public Property sample_fecalcal As String

    ''' <summary>
    ''' 以visit起始的样本关联信息
    ''' </summary>
    ''' <returns></returns>
    Public Property visits As Dictionary(Of String, String)

    ''' <summary>
    ''' 从tsv表格文件之中加载资源数据
    ''' </summary>
    ''' <param name="tsv"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function LoadTable(tsv As String) As IEnumerable(Of metadata)
        Return csv.LoadTsv(tsv, Encodings.UTF8).AsDataSource(Of metadata)()
    End Function
End Class
