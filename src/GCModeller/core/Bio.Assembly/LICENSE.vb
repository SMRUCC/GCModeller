#Region "Microsoft.VisualBasic::cf8c7b5bc892e78d6f5456d165d6ddda, ..\GCModeller\core\Bio.Assembly\LICENSE.vb"

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

Imports Microsoft.VisualBasic.Scripting.MetaData

''' <summary>
''' ᶘ ᵒᴥᵒᶅ？？？？
''' </summary>
''' <remarks>
'''  _______           _______
''' く__,.ヘヽ.　　　　/　,ー､ 〉
'''　　＼ ', !-─‐-i　/　/´
''' 　 ／｀ｰ'　　　 L/／｀ヽ､
'''  /　 ／,　 /|　 ,　 ,　　　 ',
'''  ｲ 　/ /-‐/　ｉ　L_ ﾊ ヽ!　 i
'''   ﾚ ﾍ 7ｲ｀ﾄ　 ﾚ'ｧ-ﾄ､!ハ|　 |
'''  !,/7 'ゞ'　　 ´i__rﾊiソ| 　 |　　　
'''  |.从"　　_　　 ,,,, / |./ 　 |
'''  ﾚ'| i＞.､,,__　_,.イ / 　.i 　|
'''    ﾚ'| | / k_７_/ﾚ'ヽ,　ﾊ.　|
'''　　 | |/i 〈|/　 i　,.ﾍ |　i　|
'''　　　.|/ /　ｉ： 　 ﾍ!　　＼　|
''' 　 　 kヽ>､ﾊ 　 _,.ﾍ､ 　 /､!
'''　　　 !'〈//｀Ｔ´', ＼ ｀'7'ｰr'
'''　　　 ﾚ'ヽL__|___i,___,ンﾚ|ノ
'''　　 　　　ﾄ-,/　|___./
'''　　 　　　'ｰ'　　!_,.:
''' </remarks>
<Author("关倩倩", "amethyst.asuka@gcmodeller.org"), Author("谢桂纲", "xie.guigang@gcmodeller.org")>
Public Module LICENSE

    '''<summary>
    '''  Looks up a localized string similar to                     GNU GENERAL PUBLIC LICENSE
    '''                       Version 3, 29 June 2007
    '''
    ''' Copyright (C) 2007 Free Software Foundation, Inc. &lt;http://fsf.org/&gt;
    ''' Everyone is permitted to copy and distribute verbatim copies
    ''' of this license document, but changing it is not allowed.
    '''
    '''                            Preamble
    '''
    '''  The GNU General Public License is a free, copyleft license for
    '''software and other kinds of works.
    '''
    '''  The licenses for most software and other practical works are designed
    '''to take away yo [rest of string was truncated]&quot;;.
    '''</summary>
    Public ReadOnly Property GPL3 As String
        Get
            Return Microsoft.VisualBasic.LICENSE.GPL3
        End Get
    End Property

    ''' <summary>
    ''' https://github.com/SMRUCC/GCModeller.Core
    ''' </summary>
    Public Sub GithubRepository()
        Call Process.Start("https://github.com/SMRUCC/GCModeller.Core")
    End Sub
End Module
