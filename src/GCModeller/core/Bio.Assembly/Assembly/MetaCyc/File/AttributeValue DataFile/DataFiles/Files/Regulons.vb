#Region "Microsoft.VisualBasic::812c212122a3942b7268f00a94df7333, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Files\Regulons.vb"

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

    '   Total Lines: 23
    '    Code Lines: 12
    ' Comment Lines: 8
    '   Blank Lines: 3
    '     File Size: 944 B


    '     Class Regulons
    ' 
    '         Properties: AttributeList
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Assembly.MetaCyc.File.DataFiles

    ''' <summary>
    ''' This file lists all transcription factors in the PGDB and the genes that 
    ''' they regulate by binding upstream of the transcription unit containing 
    ''' those genes.
    ''' (本数据库文件中记录了所有的转录因子以及通过与包含这些基因的转录单元的上游区域
    ''' 进行结合而发挥调控作用的基因)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Regulons : Inherits DataFile(Of Slots.Regulon)

        Public Overrides ReadOnly Property AttributeList As String()
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0}  {1} frame object records.", DbProperty.ToString, FrameObjects.Count)
        End Function
    End Class
End Namespace
