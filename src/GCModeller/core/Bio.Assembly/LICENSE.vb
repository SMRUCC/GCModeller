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
