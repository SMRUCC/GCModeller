#Region "Microsoft.VisualBasic::a6c8956b6a2fd915a7f5c58871b72391, GCModeller\engine\BootstrapLoader\Definition\NucleicAcid.vb"

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
    '    Code Lines: 24
    ' Comment Lines: 16
    '   Blank Lines: 6
    '     File Size: 1.20 KB


    '     Class NucleicAcid
    ' 
    '         Properties: A, C, G, U
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Definitions

    Public Class NucleicAcid

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property A As String
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property U As String
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property G As String
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property C As String

        Default Public ReadOnly Property Base(compound As String) As String
            Get
                Select Case compound
                    Case "A" : Return A
                    Case "U", "T" : Return U
                    Case "G" : Return G
                    Case "C" : Return C
                    Case Else
                        Throw New NotImplementedException(compound)
                End Select
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Class
End Namespace
