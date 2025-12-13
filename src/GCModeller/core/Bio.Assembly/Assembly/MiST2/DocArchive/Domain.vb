#Region "Microsoft.VisualBasic::603720ce45d10e45baa4bcf2a4e53bcd, core\Bio.Assembly\Assembly\MiST2\DocArchive\Domain.vb"

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


' Code Statistics:

'   Total Lines: 46
'    Code Lines: 31 (67.39%)
' Comment Lines: 11 (23.91%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 4 (8.70%)
'     File Size: 1.82 KB


'     Structure Domain
' 
'         Function: Load, ToString, TryParse
' 
' 
' /********************************************************************************/

#End Region

Imports System.Text

Namespace Assembly.MiST2

    ''' <summary>
    ''' The Microbial Signal Transduction database contains the signal transduction proteins 
    ''' for bacterial and archaeal genomes (2,457 complete and 5,181 draft). These are 
    ''' identified using various domain profiles that directly or indirectly implicate a 
    ''' particular protein in participating in signal transduction.
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure Domain
        Dim PfamId As String
        Dim Id As String
        Dim CommonName As String
        Dim Type As String
        Dim [Function] As String
        Dim Marker As Boolean

        Public Shared Function TryParse(strText As String) As Domain
            Dim Tokens As String() = strText.Split(CChar(","))
            Return New Domain With {
                .PfamId = Tokens(0),
                .Id = Tokens(1),
                .CommonName = Tokens(2),
                .Type = Tokens(3),
                .Function = Tokens(4),
                .Marker = String.Equals("-", Tokens(5))
            }
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("({0}) {1}", PfamId, CommonName)
        End Function

        ''' <summary>
        ''' 从内部的资源文件之中进行加载
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function Load() As Domain()
            Dim DbRecordText As String() = Strings.Split(Encoding.UTF8.GetString(My.Resources._Default.MiST2), vbCrLf).Skip(1).ToArray
            Dim LQuery As Domain() = (From line As String
                                      In DbRecordText
                                      Select Domain.TryParse(line)).ToArray
            Return LQuery
        End Function
    End Structure
End Namespace
