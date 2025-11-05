#Region "Microsoft.VisualBasic::998d4ef20c6e0923db866190e286380b, meme_suite\MEME.DocParser\TomTom\TOMText.vb"

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

'     Class TOMText
' 
'         Properties: evalue, OptimalOffset, Orientation, Overlap, pvalue
'                     Query, QueryConsensus, qvalue, Target, TargetConsensus
' 
'         Function: LoadDoc, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Data.Framework.DataImports

Namespace DocumentFormat.TomTOM

    Public Class TOMText

        ''' <summary>
        ''' #Query ID
        ''' </summary>
        ''' <returns></returns>
        <Column("#Query ID")> Public Property Query As String
        ''' <summary>
        ''' Target ID
        ''' </summary>
        ''' <returns></returns>
        <Column("Target ID")> Public Property Target As String
        <Column("Optimal offset")> Public Property OptimalOffset As Double
        <Column("p-value")> Public Property pvalue As Double
        <Column("E-value")> Public Property evalue As Double
        <Column("q-value")> Public Property qvalue As Double
        Public Property Overlap As Double
        <Column("Query consensus")> Public Property QueryConsensus As String
        <Column("Target consensus")> Public Property TargetConsensus As String
        Public Property Orientation As String

        Public Overrides Function ToString() As String
            Return $"{Query} => {Target}:   {pvalue}  {evalue}  {qvalue}"
        End Function

        Public Shared Function LoadDoc(path As String) As TOMText()
            Return path.Imports(Of TOMText)(delimiter:=vbTab)
        End Function
    End Class
End Namespace
